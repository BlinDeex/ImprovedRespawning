using ImprovedRespawning.Assets.MainClasses;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Commands;

public class GetIRAuthCommand : ModCommand
{
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (ImprovedRespawningModSystem.Instance.AuthCode == "")
        {
            ImprovedRespawningModSystem.Instance.RegenerateAuthCode();
        }
        
        ImprovedRespawningModSystem.Instance.PrintAuthMessage();
    }

    public override string Command => "GetIRAuth";
    public override string Usage => "/GetIRAuth";
    public override CommandType Type => CommandType.Console;
}