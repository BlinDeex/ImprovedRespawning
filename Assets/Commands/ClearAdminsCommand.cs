using ImprovedRespawning.Assets.MainClasses;
using ImprovedRespawning.Assets.Misc;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class ClearAdminsCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ImprovedRespawningModSystem.Instance.ClearAdmins();
        string message = Localization.ClearedAdmins.Value;
        Utilities.LogMessage(message, LogType.Important);
    }

    public override string Command => "ClearAdmins";
    public override string Usage => "/ClearAdmins";
    public override CommandType Type => CommandType.Console;
}