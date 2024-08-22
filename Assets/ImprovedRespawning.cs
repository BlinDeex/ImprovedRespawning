using System;
using System.IO;
using System.Reflection;
using ImprovedRespawning.Assets.Hardcore;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets;
public class ImprovedRespawning : Mod
{
    private static Type interfaceType;
    public object ModConfigRef;
    public static ImprovedRespawning Instance { get; private set; }

    public override void Load()
    {
        Instance = this;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketDataType type = (PacketDataType)reader.ReadInt32();

        switch (type)
        {
            case PacketDataType.TurnToGhost:
                bool trueOrFalse = reader.ReadBoolean();
                Main.LocalPlayer.ghost = trueOrFalse;
                break;
            case PacketDataType.CleanseWorld:
                HardcoreModuleModSystem.Instance.CleanseWorld();
                break;
            case PacketDataType.CleansePlayer:
                string name = reader.ReadString();
                HardcoreModuleModSystem.Instance.CleansePlayer(name);
                break;
            case PacketDataType.TryAuth:
                string code = reader.ReadString();
                ImprovedRespawningModSystem.Instance.TryAddAdminUser(Main.player[whoAmI].name, code);
                break;
        }
    }

    public override void PostSetupContent()
    {
        interfaceType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.Interface");
        ModConfigRef = interfaceType?.GetField("modConfig", (BindingFlags)40)?.GetValue(null);
    }
}