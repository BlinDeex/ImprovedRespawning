using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class ClearAdminsCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ImprovedRespawningModSystem.Instance.ClearAdmins();
        Utilities.ConsoleMessage("Admins successfully cleared");
    }

    public override string Command => "ClearAdmins";
    public override string Usage => "/ClearAdmins";
    public override CommandType Type => CommandType.Console;
}