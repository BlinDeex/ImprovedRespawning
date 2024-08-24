using ImprovedRespawning.Assets.MainClasses;
using ImprovedRespawning.Assets.Misc;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class RegenerateAuthCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ImprovedRespawningModSystem.Instance.RegenerateAuthCode();
        Utilities.LogMessage(Localization.RegenAuth.Value, LogType.Important);
    }

    public override string Command => "RegenIRAuth";
    public override string Usage => "RegenIRAuth";
    public override CommandType Type => CommandType.Console;
}