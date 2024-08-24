using ImprovedRespawning.Assets.Hardcore;
using ImprovedRespawning.Assets.MainClasses;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class CleansePlayerCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        if (!config.LetPlayersUseCheatCommands)
        {
            caller.Reply("Config does not allow to use this command!");
            return;
        }
        
        HardcoreModuleModSystem.Instance.CleansePlayer(caller.Player.name);
    }

    public override string Command => "CleanseMe";
    public override string Usage => "/CleanseMe";
    public override CommandType Type => CommandType.Chat;
}