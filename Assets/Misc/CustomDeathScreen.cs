using System.Text;
using ImprovedRespawning.Assets.MainClasses;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Misc;

public class CustomDeathScreen : ModSystem
{

    public override void Load() => On_Main.DrawInterface_35_YouDied += DeathScreenRework;

    private static void DeathScreenRework(On_Main.orig_DrawInterface_35_YouDied orig)
    {
        if (!Main.LocalPlayer.dead) return;

        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        ImprovedRespawningModSystem modSystem = ModContent.GetInstance<ImprovedRespawningModSystem>();
        ImprovedRespawningPlayer player = Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>();

        string youHaveBeenSlainText = Lang.inter[38].Value;

        float targetSlainTextX =
            Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(youHaveBeenSlainText).X / 2f;
        float targetSlainTextY = Main.screenHeight / 2f - 80f;

        int currentYOffset = (int)targetSlainTextY;

        Vector2 targetSlainTextPos = new(targetSlainTextX, targetSlainTextY);
        Color targetTextAlpha = Main.player[Main.myPlayer].GetDeathAlpha(Color.Transparent);

        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, youHaveBeenSlainText, targetSlainTextPos,
            targetTextAlpha, 0f, Vector2.Zero, 1f, 0, 0f);

        currentYOffset += 60;

        if (Main.player[Main.myPlayer].lostCoins > 0)
        {
            string coinsText = Language.GetTextValue("Game.DroppedCoins", Main.LocalPlayer.lostCoinString);
            float targetCoinsLostTextX =
                Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(coinsText).X / 2f * 0.7f;
            Vector2 targetCoinsLostPos = new(targetCoinsLostTextX, currentYOffset);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, coinsText, targetCoinsLostPos, targetTextAlpha,
                0f, Vector2.Zero, 0.7f, 0, 0f);
            currentYOffset += 50;
        }


        if (config.EnableMaxLivesPerBoss && player.TimesDiedToSameBoss >= config.MaxLivesPerBoss)
        {
            float targetPermDeathX = Main.screenWidth / 2f -
                                     FontAssets.DeathText.Value.MeasureString(Localization.PermDeathText.Value).X / 2f * 0.7f;
            Vector2 targetPermDeathPos = new(targetPermDeathX, currentYOffset);
            Color targetPermDeathColor = new(168, 50, 50, targetTextAlpha.A);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, Localization.PermDeathText.Value, targetPermDeathPos,
                targetPermDeathColor, 0f, Vector2.Zero, 0.7f, 0, 0f);
            return;
        }

        int num = Main.LocalPlayer.respawnTimer / 60;
        int minutes = num / 60;
        int seconds = num % 60 + 1;
        StringBuilder tillRespawn = new(Localization.RespawnInText.Value + " ");
        if (minutes > 0) tillRespawn.Append(Localization.RespawnInMinutesText.Format(minutes));
        tillRespawn.Append(Localization.RespawnInSecondsText.Format(seconds));

        string tillRespawnText = tillRespawn.ToString();

        float targetTillRespawnX = Main.screenWidth / 2f -
                                   FontAssets.DeathText.Value.MeasureString(tillRespawnText).X / 2f * 0.7f;

        Vector2 targetTillRespawnPos = new(targetTillRespawnX, currentYOffset);

        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, tillRespawnText, targetTillRespawnPos,
            targetTextAlpha, 0f, Vector2.Zero, 0.7f, 0, 0f);

        if (!config.EnableMaxLivesPerBoss) return;
        if (!modSystem.BossActive) return;
        
        string livesUsedText = Localization.YouDiedToThisBossText.Format(player.TimesDiedToSameBoss, config.MaxLivesPerBoss);
        currentYOffset += 50;
        float targetLivesUsedX =
            Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(livesUsedText).X / 2f * 0.7f;
        Vector2 targetLivesUsedPos = new(targetLivesUsedX, currentYOffset);
        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, livesUsedText, targetLivesUsedPos, targetTextAlpha,
            0f, Vector2.Zero, 0.7f, 0, 0f);

        if (player.TimesDiedToSameBoss != config.MaxLivesPerBoss - 1) return;

        currentYOffset += 50;

        float targetLastChanceX = Main.screenWidth / 2f -
                                  FontAssets.DeathText.Value.MeasureString(Localization.LastChanceText.Value).X / 2f * 0.7f;
        Vector2 targetLastChancePos = new(targetLastChanceX, currentYOffset);
        Color targetColor = new(131, 50, 168, targetTextAlpha.A);
        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, Localization.LastChanceText.Value, targetLastChancePos,
            targetColor, 0f, Vector2.Zero, 0.7f, 0, 0f);
    }
}