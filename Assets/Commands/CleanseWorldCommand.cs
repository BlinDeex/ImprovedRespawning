using ImprovedRespawning.Assets.Hardcore;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class CleansWorldCommand : ModCommand
{
    public override string Command => "CleanseWorld";

    public override string Usage => "/CleanseWorld";

    public override string Description => "Resets this world total death count";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        if (!config.LetPlayersUseCheatCommands)
        {
            caller.Reply("Config does not allow to use this command!");
            return;
        }

        HardcoreModuleModSystem.Instance.CleanseWorld();
    }
}