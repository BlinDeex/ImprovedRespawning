using ImprovedRespawning.Assets.MainClasses;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class ReviveCommand : ModCommand
{
    public override string Command => "Revive";

    public override string Usage => "/Revive";

    public override string Description => "Clears respawn timer instantly";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        
        if (!caller.Player.dead)
        {
            caller.Reply("You are not dead!");
            return;
        }

        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();

        if (!config.LetPlayersUseCheatCommands)
        {
            caller.Reply("Config does not allow to use this command!");
            return;
        }

        caller.Player.respawnTimer = 0;
        caller.Reply("You have been revived!");
    }
}