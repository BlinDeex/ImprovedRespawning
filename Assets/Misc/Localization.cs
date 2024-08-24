using System;
using System.Text;
using Terraria.Localization;
using Terraria.ModLoader;
using ImprovedRespawning.Assets.MainClasses;

namespace ImprovedRespawning.Assets.Misc;

public static class Localization
{
    private const string YOU_DIED_HUD_PREFIX = "You_Died";
    private const string LOGGING_PREFIX = "Logging";
    
    
    #region You_Died_HUD
    
    public static LocalizedText RespawnInText { get; set; }
    public static LocalizedText RespawnInMinutesText { get; set; }
    public static LocalizedText RespawnInSecondsText { get; set; }
    public static LocalizedText LastChanceText { get; set; }
    public static LocalizedText YouDiedToThisBossText { get; set; }
    public static LocalizedText PermDeathText { get; set; }
    
    #endregion

    #region Logging

    public static LocalizedText AuthCodeMessage { get; set; }
    public static LocalizedText PlayerKilledIncreasingWorldDeaths { get; set; }
    public static LocalizedText WorldHasExceededDeaths { get; set; }
    public static LocalizedText PlayerHasExceededDeaths { get; set; }
    public static LocalizedText RejectConfigChangesNotAnAdmin { get; set; }
    public static LocalizedText RejectConfigChangesOnlyHost { get; set; }
    public static LocalizedText RejectConfigChangesServerOnlySetting { get; set; }
    public static LocalizedText ClearedAdmins { get; set; }
    public static LocalizedText AddedAdmin { get; set; }
    public static LocalizedText CodeInvalid { get; set; }
    
    public static LocalizedText RegenAuth { get; set; }

    #endregion
    
    public static void LoadLocalization()
    {
        Mod mod = MainClasses.ImprovedRespawning.Instance;

        RespawnInText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(RespawnInText)));
        RespawnInMinutesText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(RespawnInMinutesText)));
        RespawnInSecondsText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(RespawnInSecondsText)));
        LastChanceText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(LastChanceText)));
        YouDiedToThisBossText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(YouDiedToThisBossText)));
        PermDeathText = mod.GetLocalization(GetLocalizationPath(LocalizationType.You_Died, nameof(PermDeathText)));
        
        AuthCodeMessage = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(AuthCodeMessage)));
        PlayerKilledIncreasingWorldDeaths = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(PlayerKilledIncreasingWorldDeaths)));
        WorldHasExceededDeaths = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(WorldHasExceededDeaths)));
        RejectConfigChangesNotAnAdmin = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(RejectConfigChangesNotAnAdmin)));
        RejectConfigChangesOnlyHost = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(RejectConfigChangesOnlyHost)));
        RejectConfigChangesServerOnlySetting = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(RejectConfigChangesServerOnlySetting)));
        PlayerHasExceededDeaths = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(PlayerHasExceededDeaths)));
        ClearedAdmins = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(ClearedAdmins)));
        AddedAdmin = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(AddedAdmin)));
        CodeInvalid = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(CodeInvalid)));
        RegenAuth = mod.GetLocalization(GetLocalizationPath(LocalizationType.Logging, nameof(RegenAuth)));
    }

    private static string GetLocalizationPath(LocalizationType type, string name)
    {
        StringBuilder finalPath = new();
        string prefix = type switch
        {
            LocalizationType.You_Died => YOU_DIED_HUD_PREFIX,
            LocalizationType.Logging => LOGGING_PREFIX,
            _ => "Unknown"
        };
        finalPath.Append(prefix);
        finalPath.Append('.');
        finalPath.Append(name);
        return finalPath.ToString();
    }
}