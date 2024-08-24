using System;
using System.Collections.Generic;
using System.Linq;
using ImprovedRespawning.Assets.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImprovedRespawning.Assets.MainClasses;

public class ImprovedRespawningPlayer : ModPlayer
{
    private readonly Dictionary<int, int> buffsOnDeath = new();

    private bool respawnOnBossDeath;

    public int TimesDiedToSameBoss { get; set; }

    public bool KingBedSpawn { get; set; }

    public List<int> BuffsToSpawnWith { get; set; } = [];

    public override void LoadData(TagCompound tag)
    {
        try
        {
            KingBedSpawn = tag.ContainsKey("KingBedSpawn") && tag.GetBool("KingBedSpawn");
            BuffsToSpawnWith = tag.ContainsKey("BuffsToSpawnWith") ? tag.GetList<int>("BuffsToSpawnWith").ToList() : [];
        }
        catch(Exception e)
        {
            string message = $"{nameof(LoadData)}: Failure! {e.Message}";
            Utilities.LogMessage(message, LogType.Error);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("KingBedSpawn", KingBedSpawn);
        tag.Add("BuffsToSpawnWith", BuffsToSpawnWith);
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        respawnOnBossDeath = config.RespawnOnBossDeath;
        ImprovedRespawningModSystem modSystem = ModContent.GetInstance<ImprovedRespawningModSystem>();
        
        if (modSystem.BossActive) TimesDiedToSameBoss += 1;
        
        if (config.KeepBuffsOnDeath)
        {
            buffsOnDeath.Clear();
            float buffMultiplier = config.KeptBuffsLengthMultiplier;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffType = Player.buffType[i];
                if (!Main.debuff[buffType] && !Main.persistentBuff[buffType] && !buffsOnDeath.ContainsKey(buffType))
                    buffsOnDeath.Add(buffType, (int)(Player.buffTime[i] * buffMultiplier));
            }
        }

        Player.respawnTimer = CalculateRespawnTimer();
    }

    private int CalculateRespawnTimer()
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        ImprovedRespawningModSystem modSystem = ModContent.GetInstance<ImprovedRespawningModSystem>();
        
        if (config.EnableMaxLivesPerBoss && TimesDiedToSameBoss >= config.MaxLivesPerBoss)
        {
            return int.MaxValue;
        }
        
        if (config.InstantRespawn)
        {
            return 0;
        }
        
        float targetRespawnTimer = config.BaseRespawnTimer;
        int additionalPlayersActive = 0;
        
        for (int i = 0; i < 255; i++)
        {
            Player player = Main.player[i];
            if (player != Main.LocalPlayer && player.active) additionalPlayersActive++;
        }

        if (additionalPlayersActive > 0 && (modSystem.BossActive || modSystem.EventActive))
            targetRespawnTimer += additionalPlayersActive * config.AdditionalSecondsPerPlayer;
        
        targetRespawnTimer += TimesDiedToSameBoss * config.BossAdditionPerDeath;
        targetRespawnTimer *= config.BaseScalar;
        if (Main.hardMode) targetRespawnTimer *= config.HardmodeScalar;
        if (Main.expertMode) targetRespawnTimer *= config.ExpertModeScalar;
        if (modSystem.BossActive) targetRespawnTimer *= config.BossAliveScalar;
        if (Main.masterMode) targetRespawnTimer *= config.MasterModeScalar;
        if (modSystem.EventActive) targetRespawnTimer *= config.EventModeScalar;
        targetRespawnTimer *= 60f;
        return (int)targetRespawnTimer;
    }

    public override void OnRespawn()
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        float targetMana = config.ManaOnRespawn / 100f;
        float targetHealth = config.HealthOnRespawn / 100f;
        Player.statMana = (int)(targetMana * Player.statManaMax2);
        Player.statLife = (int)(targetHealth * Player.statLifeMax2);
        
        if (config.KeepBuffsOnDeath) foreach (var kvp in buffsOnDeath) Player.AddBuff(kvp.Key, kvp.Value, false);

        if (BuffsToSpawnWith.Count == 0 || !config.EnableBuffsOnSpawn) return;
        
        int targetLength = config.BuffsOnSpawnLength * 60;
        
        foreach (int buffID in BuffsToSpawnWith)
        {
            try
            {
                Player.AddBuff(buffID, targetLength, false);
            }
            catch
            {
                string message = $"{nameof(OnRespawn)}: Failed to apply buff! ID: {buffID} Name: {BuffLoader.GetBuff(buffID).Name}";
                Utilities.LogMessage(message, LogType.Important);
            }
        }
    }

    public override void PreUpdate()
    {
        CheckBoss();
        MouseCalc();
    }

    public Vector2 MouseDelta { get; private set; }
    private Vector2 lastFrameMousePos = Vector2.Zero;
    
    private void MouseCalc()
    {
        MouseDelta = Main.MouseScreen - lastFrameMousePos;
        lastFrameMousePos = Main.MouseScreen;
    }

    private void CheckBoss()
    {
        bool bossDisappeared = ModContent.GetInstance<ImprovedRespawningModSystem>().BossDisappeared;
        if (bossDisappeared) TimesDiedToSameBoss = 0;
        if (!Player.dead) return;
        if (!bossDisappeared) return;
        Player.respawnTimer = respawnOnBossDeath ? 0 : CalculateRespawnTimer();
    }
}