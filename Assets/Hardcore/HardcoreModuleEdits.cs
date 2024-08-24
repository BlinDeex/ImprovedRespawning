using ImprovedRespawning.Assets.MainClasses;
using ImprovedRespawning.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Hardcore;

public class HardcoreModuleEdits : ModSystem
{
    public override void Load()
    {
        On_Player.KillMe += KillMeDetour;
        
        On_Player.Spawn += SpawnDetour;
    }

    private void SpawnDetour(On_Player.orig_Spawn orig, Player self, PlayerSpawnContext context)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            orig.Invoke(self, context);
            if (self != Main.LocalPlayer) return;
            if (Utilities.TryGetPlayerToSpawnOn(out Player player))
            {
                Main.LocalPlayer.position = player.position;
            }
            return;
        }

        if (HardcoreModuleModSystem.Instance.TotalWorldDeathsReached)
        {
            HardcoreModuleModSystem.Instance.TurnTargetIntoGhost(self.whoAmI);
            DelayedChatMessage message = new()
            {
                //TODO: broadcasts to everyone and there is no target player param so idk
                message = Localization.WorldHasExceededDeaths.Value,
                color = Color.Red,
                ticksDelayed = 60
            };
            HardcoreModuleModSystem.Instance.AddDelayedMessage(message);
        }

        if (HardcoreModuleModSystem.Instance.DeadPlayers.Contains(self.name))
        {
            HardcoreModuleModSystem.Instance.TurnTargetIntoGhost(self.whoAmI);
            DelayedChatMessage message = new()
            {
                message = Localization.PlayerHasExceededDeaths.Value,
                color = Color.Red,
                ticksDelayed = 60
            };
            HardcoreModuleModSystem.Instance.AddDelayedMessage(message);
        }
            
        // if not a server
        //if(Main.netMode == NetmodeID.SinglePlayer) orig.Invoke(self, context);
            
        orig.Invoke(self, context);
    }

    private void KillMeDetour(On_Player.orig_KillMe orig, Player self, PlayerDeathReason source, double dmg, int direction, bool pvp)
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        
        bool wasPvPDeath = source.SourcePlayerIndex > -1 && Main.LocalPlayer.whoAmI != source.SourcePlayerIndex;

        if (!config.EnablePvPDeaths && wasPvPDeath)
        {
            orig.Invoke(self,source,dmg,direction,pvp);
            return;
        }
        
        if (config.EnableMaximumLivesPerWorld)
        {
            HardcoreModuleModSystem.Instance.IncreaseWorldDeaths(self.name);
        }

        if (config.EnableMaximumLivesPerPlayer)
        {
            HardcoreModuleModSystem.Instance.IncreasePlayerDeaths(self.whoAmI, self.name);
        }
            
        orig.Invoke(self,source,dmg,direction,pvp);
    }
}