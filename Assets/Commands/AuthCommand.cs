using ImprovedRespawning.Assets.Hardcore;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class AuthCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length != 1)
        {
            Main.NewText($"Expected only one argument (the code), got {args.Length} instead", Color.Red);
            return;
        }

        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            Main.NewText("Command only works in multiplayer!", Color.Red);
            return;
        }
        
        Main.NewText(input);
        
        ModPacket packet = Mod.GetPacket();
        packet.Write((int)PacketDataType.TryAuth);
        packet.Write(args[0]);
        packet.Send();
    }

    public override string Command => "IRAuth";
    public override string Usage => "/IRAuth <code you see in the console>";
    public override CommandType Type => CommandType.Chat;
}