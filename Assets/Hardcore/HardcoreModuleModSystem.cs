using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImprovedRespawning.Assets.Hardcore;

public enum PacketDataType
{
    TurnToGhost,
    CleanseWorld,
    CleansePlayer,
    TryAuth
}

public class DelayedChatMessage
{
    public Color color;
    public string message;
    public int ticksDelayed;
}

public class HardcoreModuleModSystem : ModSystem
{
    public static HardcoreModuleModSystem Instance { get; private set; }

    public override void Load()
    {
        Instance = this;
    }

    public int TotalWorldDeaths;
    public bool TotalWorldDeathsReached;
    public Dictionary<string,int> PlayerDeaths = [];
    public HashSet<string> DeadPlayers = [];

    private List<DelayedChatMessage> delayedChatMessages = [];

    public override void SaveWorldData(TagCompound tag)
    {
        SaveHardcoreWorldData(tag);
    }

    public void AddDelayedMessage(DelayedChatMessage message)
    {
        delayedChatMessages.Add(message);
    }

    public override void ClearWorld()
    {
        TotalWorldDeaths = 0;
        TotalWorldDeathsReached = false;
        PlayerDeaths = [];
        DeadPlayers = [];
        delayedChatMessages = [];
    }

    public override void PostUpdateEverything()
    {
        foreach (DelayedChatMessage message in delayedChatMessages)
        {
            message.ticksDelayed--;
            if (message.ticksDelayed > 0) continue;
            
            Utilities.BroadcastOrNewText(message.message, message.color);
        }

        delayedChatMessages.RemoveAll(x => x.ticksDelayed <= 0);
    }
    
    
    /// <summary>
    /// handles any netmode
    /// </summary>
    private void TurnEveryoneIntoGhost(bool isGhost = true)
    {
        if (Main.dedServ)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((int)PacketDataType.TurnToGhost);
            packet.Write(isGhost);
            packet.Send();
            return;
        }

        Main.LocalPlayer.ghost = isGhost;
    }
    
    /// <summary>
    /// handles any netmode
    /// </summary>
    public void TurnTargetIntoGhost(int whoAmI, bool isGhost = true)
    {
        if (Main.dedServ)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((int)PacketDataType.TurnToGhost);
            packet.Write(isGhost);
            packet.Send(whoAmI);
            return;
        }

        Main.LocalPlayer.ghost = isGhost;
    }
    
    public void IncreaseWorldDeaths(string playerWhoDied)
    {
        TotalWorldDeaths++;
        
        if (Main.netMode == NetmodeID.MultiplayerClient) return;
        
        string message = $"{playerWhoDied} has been killed! Increasing world deaths to {TotalWorldDeaths}";
        if (TotalWorldDeaths < ModContent.GetInstance<ImprovedRespawningConfig>().MaximumLivesPerWorld)
        {
            Utilities.BroadcastOrNewText(message, Color.OrangeRed);
            return;
        }
        
        TotalWorldDeathsReached = true;
        message = "Maximum world deaths have been reached!";
        Utilities.BroadcastOrNewText(message, Color.Red);
        TurnEveryoneIntoGhost();
    }

    public void CleanseWorld()
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((int)PacketDataType.CleanseWorld);
                packet.Send();
                return;
            }
            case NetmodeID.Server:
            case NetmodeID.SinglePlayer:
            {
                TotalWorldDeaths = 0;
                TotalWorldDeathsReached = false;
                string message = "World deaths have been reset!";
                Utilities.BroadcastOrNewText(message, Color.Green);
                foreach (Player player in Main.ActivePlayers)
                {
                    if (DeadPlayers.Contains(player.name)) continue;
                    TurnTargetIntoGhost(player.whoAmI, false);
                }
                break;
            }
        }
    }

    public void CleansePlayer(string name)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((int)PacketDataType.CleansePlayer);
                packet.Write(name);
                packet.Send();
                return;
            }
            case NetmodeID.Server:
            case NetmodeID.SinglePlayer:
            {
                DeadPlayers.Remove(name);
                PlayerDeaths.Remove(name);

                string message = $"{name} deaths have been reset!";
                Utilities.BroadcastOrNewText(message, Color.Green);
                TurnTargetIntoGhost(Main.player.First(x => x.name == name).whoAmI, false);
                break;
            }
        }
    }

    public void IncreasePlayerDeaths(int whoAmI, string playerWhoDied)
    {
        if (!PlayerDeaths.TryAdd(playerWhoDied, 1))
        {
            PlayerDeaths[playerWhoDied]++;
        }
        
        if (Main.netMode == NetmodeID.MultiplayerClient) return;
        
        if (TotalWorldDeathsReached) return;

        int playerDeaths = PlayerDeaths[playerWhoDied];
        string message = $"{playerWhoDied} has been killed! Increasing his deaths to {playerDeaths}";
        
        Utilities.BroadcastOrNewText(message, Color.OrangeRed);

        if (playerDeaths < ModContent.GetInstance<ImprovedRespawningConfig>().MaximumLivesPerPlayer) return;

        string message2 = $"{playerWhoDied} reached {playerDeaths} deaths and became a ghost!";
        
        Utilities.BroadcastOrNewText(message2, Color.Red);
        
        TurnTargetIntoGhost(whoAmI);
        DeadPlayers.Add(playerWhoDied);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        LoadHardcoreWorldData(tag);
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(TotalWorldDeaths);
        writer.Write(TotalWorldDeathsReached);
        string playerDeaths = JsonConvert.SerializeObject(PlayerDeaths);
        writer.Write(playerDeaths);
        string deadPlayers = JsonConvert.SerializeObject(DeadPlayers);
        writer.Write(deadPlayers);
    }

    public override void NetReceive(BinaryReader reader)
    {
        int totalWorldDeaths = reader.ReadInt32();
        bool totalWorldDeathsReached = reader.ReadBoolean();
        string playerDeaths = reader.ReadString();
        string deadPlayers = reader.ReadString();
        
        TotalWorldDeaths = totalWorldDeaths;
        TotalWorldDeathsReached = totalWorldDeathsReached;
        PlayerDeaths = JsonConvert.DeserializeObject<Dictionary<string, int>>(playerDeaths);
        DeadPlayers = JsonConvert.DeserializeObject<HashSet<string>>(deadPlayers);
    }

    private void SaveHardcoreWorldData(TagCompound tag)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) return;
        tag.Add(nameof(TotalWorldDeaths), TotalWorldDeaths);
        tag.Add(nameof(TotalWorldDeathsReached), TotalWorldDeathsReached);
        string playerDeathsData = JsonConvert.SerializeObject(PlayerDeaths);
        tag.Add(nameof(PlayerDeaths), playerDeathsData);
        string deadPlayersData = JsonConvert.SerializeObject(DeadPlayers);
        tag.Add(nameof(DeadPlayers), deadPlayersData);
    }

    private void LoadHardcoreWorldData(TagCompound tag)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) return;
        string error = string.Empty;
        
        if (!tag.TryGet(nameof(TotalWorldDeaths), out int totalDeaths)) error = "Failed to get TotalWorldDeaths!";
        if (!tag.TryGet(nameof(TotalWorldDeathsReached), out bool totalDeathsReached)) error = "Failed to get WorldTotalDeathsReached!";
        if (!tag.TryGet(nameof(PlayerDeaths), out string playerDeaths)) error = "Failed to get PlayerDeaths!";
        if (!tag.TryGet(nameof(DeadPlayers), out string deadPlayers)) error = "Failed to get DeadPlayers!";

        if (error != string.Empty)
        {
            Utilities.BroadcastOrNewText(error, Color.Red);
            return;
        }

        PlayerDeaths = JsonConvert.DeserializeObject<Dictionary<string, int>>(playerDeaths);
        TotalWorldDeaths = totalDeaths;
        TotalWorldDeathsReached = totalDeathsReached;
        DeadPlayers = JsonConvert.DeserializeObject<HashSet<string>>(deadPlayers);
    }
}