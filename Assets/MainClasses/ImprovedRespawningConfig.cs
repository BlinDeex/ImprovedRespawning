using System.ComponentModel;
using ImprovedRespawning.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace ImprovedRespawning.Assets.MainClasses;

[BackgroundColor(0, 0, 0, 200)]
public class ImprovedRespawningConfig : ModConfig
{
    public override ConfigScope Mode => 0;


    [BackgroundColor(200, 200, 100, 200)]
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.ServerOnly")]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.LetClientsModifyConfig.Tooltip")]
    [DefaultValue(true)]
    public bool LetClientsModifyConfig { get; set; }
    
    [BackgroundColor(200, 200, 100, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.LetPlayersUseCheatCommands.Tooltip")]
    public bool LetPlayersUseCheatCommands { get; set; }


    [BackgroundColor(100, 100, 255, 200)]
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.BaseRespawnTimer")]
    [Range(1, 500)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.BaseRespawnTimer.Tooltip")]
    [DefaultValue(15)]
    public int BaseRespawnTimer { get; set; }


    [BackgroundColor(100, 100, 255, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.AdditionalSecondsPerPlayer.Tooltip")]
    [DefaultValue(0)]
    public int AdditionalSecondsPerPlayer { get; set; }


    [BackgroundColor(255, 100, 128, 200)]
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.DifficultyModes")]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(255, 100, 128, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.HardmodeScalar.Tooltip")]
    [DefaultValue(1f)]
    public float HardmodeScalar { get; set; }


    [BackgroundColor(255, 100, 128, 200)]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(255, 100, 128, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.ExpertModeScalar.Tooltip")]
    [DefaultValue(1f)]
    public float ExpertModeScalar { get; set; }


    [BackgroundColor(255, 100, 128, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.MasterModeScalar.Tooltip")]
    [DefaultValue(1f)]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(255, 100, 128, 200)]
    public float MasterModeScalar { get; set; }


    [BackgroundColor(200, 200, 200, 200)]
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.MiscScalars")]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(200, 200, 200, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.BaseScalar.Tooltip")]
    [DefaultValue(1f)]
    public float BaseScalar { get; set; }


    [BackgroundColor(200, 200, 200, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EventModeScalar.Tooltip")]
    [DefaultValue(1f)]
    [SliderColor(200, 200, 200, 200)]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    public float EventModeScalar { get; set; }


    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.BossSettings")]
    [BackgroundColor(255, 128, 100, 200)]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(255, 128, 100, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.BossAliveScalar.Tooltip")]
    [DefaultValue(1f)]
    public float BossAliveScalar { get; set; }


    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.BossAdditionPerDeath.Tooltip")]
    [BackgroundColor(255, 128, 100, 200)]
    [DefaultValue(0)]
    [Range(0, 250)]
    [Increment(.1f)]
    public int BossAdditionPerDeath { get; set; }


    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnableMaxLivesPerBoss.Tooltip")]
    [BackgroundColor(255, 128, 100, 200)]
    [DefaultValue(false)]
    public bool EnableMaxLivesPerBoss { get; set; }


    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.MaxLivesPerBoss.Tooltip")]
    [BackgroundColor(255, 128, 100, 200)]
    [Range(1, 250)]
    [Increment(.1f)]
    [DefaultValue(1)]
    public int MaxLivesPerBoss { get; set; }


    [BackgroundColor(255, 128, 100, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.RespawnOnBossDeath.Tooltip")]
    public bool RespawnOnBossDeath { get; set; }
    
    [BackgroundColor(255, 128, 100, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.RespawnOnPlayer.Tooltip")]
    public bool RespawnOnPlayer { get; set; }


    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.OnRespawn")]
    [BackgroundColor(150, 255, 150, 200)]
    [Range(1, 100)]
    [Slider]
    [SliderColor(150, 255, 150, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.HealthOnRespawn.Tooltip")]
    [DefaultValue(50)]
    public int HealthOnRespawn { get; set; }


    [BackgroundColor(150, 255, 150, 200)]
    [Range(0, 100)]
    [Slider]
    [SliderColor(150, 255, 150, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.ManaOnRespawn.Tooltip")]
    [DefaultValue(50)]
    public int ManaOnRespawn { get; set; }


    [BackgroundColor(150, 255, 150, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.KeepBuffsOnDeath.Tooltip")]
    public bool KeepBuffsOnDeath { get; set; }


    [BackgroundColor(150, 255, 150, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.KeptBuffsLengthMultiplier.Tooltip")]
    [DefaultValue(1f)]
    [Range(0.1f, 10f)]
    [Increment(.1f)]
    [SliderColor(150, 255, 150, 200)]
    public float KeptBuffsLengthMultiplier { get; set; }


    [BackgroundColor(150, 255, 150, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnableBuffsOnSpawn.Tooltip")]
    [DefaultValue(false)]
    public bool EnableBuffsOnSpawn { get; set; }
    
    [BackgroundColor(150, 255, 150, 200)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.BuffsOnSpawnLength.Tooltip")]
    [DefaultValue(1)]
    [Range(1, 500)]
    public int BuffsOnSpawnLength { get; set; }

    
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.Hardcore")]
    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnableMaximumLivesPerPlayer.Tooltip")]
    public bool EnableMaximumLivesPerPlayer { get; set; }


    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(1)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.MaximumLivesPerPlayer.Tooltip")]
    public int MaximumLivesPerPlayer { get; set; }


    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnableMaximumLivesPerWorld.Tooltip")]
    public bool EnableMaximumLivesPerWorld { get; set; }


    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(1)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.MaximumLivesPerWorld.Tooltip")]
    public int MaximumLivesPerWorld { get; set; }
    
    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnablePvPDeaths.Tooltip")]
    public bool EnablePvPDeaths { get; set; }
    
    [BackgroundColor(255, 0, 0, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.EnableHardcoreUI.Tooltip")]
    public bool EnableHardcoreUI { get; set; }
    

    [BackgroundColor(200, 200, 200, 200)]
    [Header("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.Header.Other")]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.InstantRespawn.Tooltip")]
    public bool InstantRespawn { get; set; }


    [BackgroundColor(200, 200, 200, 200)]
    [DefaultValue(false)]
    [TooltipKey("$Mods.ImprovedRespawning.Configs.ImprovedRespawningConfig.DisableTombstones.Tooltip")]
    public bool DisableTombstones { get; set; }


    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
    {
        bool areThereAnyAdmins = ImprovedRespawningModSystem.Instance.AreThereAnyAdmins();

        if (areThereAnyAdmins)
        {
            string playerName = Main.player[whoAmI].name;
            bool isAdmin = ImprovedRespawningModSystem.Instance.IsUserAdmin(playerName);
            if(!isAdmin) message = NetworkText.FromLiteral(Localization.RejectConfigChangesNotAnAdmin.Value);
            return isAdmin;
        }
        
        if (Netplay.Clients[whoAmI].Socket.GetRemoteAddress().IsLocalHost()) return true;
        
        if (!LetClientsModifyConfig)
        {
            message = NetworkText.FromLiteral(Localization.RejectConfigChangesOnlyHost.Value);
            return false;
        }

        bool containsServerOnlyChange = false;
        ImprovedRespawningConfig pendingClientConfig = (ImprovedRespawningConfig)pendingConfig;
        if (pendingClientConfig.LetClientsModifyConfig != LetClientsModifyConfig) containsServerOnlyChange = true;
        if (pendingClientConfig.LetPlayersUseCheatCommands != LetPlayersUseCheatCommands) containsServerOnlyChange = true;
        if (!containsServerOnlyChange) return true;
        
        message = NetworkText.FromLiteral(Localization.RejectConfigChangesServerOnlySetting.Value);
        return false;
    }
}