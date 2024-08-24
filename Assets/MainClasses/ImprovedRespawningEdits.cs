using System;
using System.Collections.Generic;
using System.Reflection;
using ImprovedRespawning.Assets.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImprovedRespawning.Assets.MainClasses;

public class ImprovedRespawningEdits : ModSystem
{
    private readonly Asset<Texture2D> lostWorldInnerBackground =
        ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/LostWorldInnerBackground");
    
    private readonly Asset<Texture2D> whitePixel =
        ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/WhitePixel");
    
    private readonly Asset<Texture2D> worldListItemReplacement =
        ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/WorldListItemReplacement");
    
    public override void PostSetupContent()
    {
        ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();
        
        On_Player.CheckSpawn += (orig, i, i1) =>
            Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>().KingBedSpawn || orig.Invoke(i, i1);
        
        IL_Player.UpdateDead += delegate(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After, x => x.MatchLdcI4(3600));
            c.EmitDelegate((int _) => int.MaxValue);
            c.GotoNext(MoveType.After, x => x.MatchLdcI4(3600));
            c.EmitDelegate((int _) => int.MaxValue);

        };
        
        On_Player.DropTombstone += delegate(On_Player.orig_DropTombstone orig, Player self, long owned,
            NetworkText text, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            if (!config.DisableTombstones)
                orig.Invoke(self, owned, text, direction);
        };
        
        EditCalamity();
        if (ImprovedRespawningModSystem.Instance.CalamityLoaded)
        {
            GetCalamitySpecialCaseBosses();
        }
        
        //Test();
    }

    private static void GetCalamitySpecialCaseBosses()
    {
        Mod calamity = ImprovedRespawningModSystem.Instance.CalamityMod;
        List<int> calBosses = [];
        
        if (calamity.TryFind("EbonianPaladin", out ModNPC ebonianPaladin))
        {
            calBosses.Add(ebonianPaladin.Type);
        }
        else
        {
            const string message = $"{nameof(GetCalamitySpecialCaseBosses)}: Failed to get {nameof(ebonianPaladin)} ModNPC";
            Utilities.LogMessage(message, LogType.Warning);
        }
        
        if (calamity.TryFind("CrimulanPaladin", out ModNPC crimulanPaladin))
        {
            calBosses.Add(crimulanPaladin.Type);
        }
        else
        {
            const string message = $"{nameof(GetCalamitySpecialCaseBosses)}: Failed to get {nameof(crimulanPaladin)} ModNPC";
            Utilities.LogMessage(message, LogType.Warning);
        }
        
        if (calamity.TryFind("SplitEbonianPaladin", out ModNPC splitEbonianPaladin))
        {
            calBosses.Add(splitEbonianPaladin.Type);
        }
        else
        {
            const string message = $"{nameof(GetCalamitySpecialCaseBosses)}: Failed to get {nameof(splitEbonianPaladin)} ModNPC";
            Utilities.LogMessage(message, LogType.Warning);
        }
        
        if (calamity.TryFind("SplitCrimulanPaladin", out ModNPC splitCrimulanPaladin))
        {
            calBosses.Add(splitCrimulanPaladin.Type);
        }
        else
        {
            const string message = $"{nameof(GetCalamitySpecialCaseBosses)}: Failed to get {nameof(splitCrimulanPaladin)} ModNPC";
            Utilities.LogMessage(message, LogType.Warning);
        }

        ImprovedRespawningModSystem.Instance.calBosses = calBosses.ToArray();
    }
    
    private void Test()
    {
        /*
        On_UIPanel.DrawSelf += (orig, self, batch) =>
        {
            if (self is not UIWorldListItem uiWorldListItem)
            {
                orig.Invoke(self, batch);
                return;
            }

            Type type = typeof(UIPanel);
            FieldInfo _needsTextureLoadingField = type.GetField("_needsTextureLoading", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _backgroundTextureField = type.GetField("_backgroundTexture", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _borderTextureField = type.GetField("_borderTexture", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo BackgroundColorField = type.GetField("BackgroundColor", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo BorderColorField = type.GetField("BorderColor", BindingFlags.Public | BindingFlags.Instance);
            bool _needsTextureLoading = (bool)_needsTextureLoadingField!.GetValue(self)!;
            var _backgroundTexture = (Asset<Texture2D>)_backgroundTextureField!.GetValue(self);
            var _borderTexture = (Asset<Texture2D>)_borderTextureField!.GetValue(self);
            Color BackgroundColor = (Color)BackgroundColorField!.GetValue(self)!;
            Color BorderColor = (Color)BorderColorField!.GetValue(self)!;

            MethodInfo LoadTextures = type.GetMethod("LoadTextures", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo DrawPanel = type.GetMethod("DrawPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (_needsTextureLoading) 
            {
                _needsTextureLoadingField.SetValue(self, false);
                LoadTextures!.Invoke(self, null);
            }

            if (_backgroundTexture != null)
            {
                DrawPanel!.Invoke(self, [batch, lostWorldInnerBackground.Value, BackgroundColor]);
            }
            
            if (_borderTexture != null)
            {
                DrawPanel!.Invoke(self, [batch, _borderTexture.Value, BorderColor]);
            }
            
        };
        */
        On_UIPanel.DrawPanel += (orig, self, batch, texture, color) =>
        {
            if (self is not UIWorldListItem uiWorldListItem)
            {
                orig.Invoke(self, batch, texture, color);
                return;
            }

            CalculatedStyle dimensions = uiWorldListItem.GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = 660;
            int height = 96;
            int lineWidth = 2;
            batch.Draw(worldListItemReplacement.Value, new Rectangle(point.X, point.Y, width, height), color);
        };
        /*
        IL_UIPanel.DrawPanel += il =>
        {
            ILCursor c = new(il);
            for (int i = 0; i < 9; i++)
            {
                if (c.TryGotoNext(MoveType.Before, x => x.MatchLdarg1())) continue;
                Mod.Logger.Error("respawn Failed DrawPanel ldarg1!");
                return;
            }
            // each draw instruction count top to bottom
            // 21 top left corner 25 top right corner 25 bottom left corner 29 bottom right corner
            // 24 * * * corner lines
            // 27 background draw
            c.RemoveRange(27);
        };
        */
    }
    
    private FieldInfo damnBosses;
    
    private void EditCalamity()
    {
        if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity))
        {
            const string message = "Calamity was not found";
            Utilities.LogMessage(message, LogType.Debug);
            return;
        }

        ImprovedRespawningModSystem.Instance.CalamityMod = calamity;
        ImprovedRespawningModSystem.Instance.CalamityLoaded = true;

        if (!calamity.TryFind("CalamityPlayer", out ModPlayer calamityPlayer))
        {
            const string message = "CalamityPlayer was not found!";
            Utilities.LogMessage(message, LogType.Error);
            return;
        }

        Type type = calamityPlayer.GetType();
        MethodInfo method = type.GetMethod("UpdateDead", (BindingFlags)52);
        MethodInfo killPlayer = type.GetMethod("KillPlayer", (BindingFlags)52);
        damnBosses = type.GetField("areThereAnyDamnBosses", (BindingFlags)24);
        
        if (method == null)
        {
            const string message = "UpdateDead was not found!";
            Utilities.LogMessage(message, LogType.Error);
            return;
        }

        if (damnBosses == null)
        {
            const string message = "damnBosses was not found!";
            Utilities.LogMessage(message, LogType.Error);
            return;
        }
        
        MonoModHooks.Modify(method, CalamityUpdateDeadPatch);
        MonoModHooks.Modify(killPlayer, CalamityKillPlayerPatch);
    }

    
    private void CalamityUpdateDeadPatch(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.AfterLabel, x => x.MatchLdsfld(damnBosses)))
        {
            const string message = $"{nameof(CalamityUpdateDeadPatch)}: Failed at MatchLdsfld damnBosses!";
            Utilities.LogMessage(message, LogType.Error);
            return;
        }

        c.EmitRet();
        
        const string message2 = $"{nameof(CalamityUpdateDeadPatch)}: Successfully applied";
        Utilities.LogMessage(message2, LogType.Debug);
    }

    private void CalamityKillPlayerPatch(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.After, x => x.MatchLdcI4(600)))
        {
            const string message = $"{nameof(CalamityKillPlayerPatch)}: Failed at MatchLdcI4 600";
            Utilities.LogMessage(message, LogType.Error);
            return;
        }
        
        c.Index -= 3;
        c.RemoveRange(16);
        const string message2 = $"{nameof(CalamityKillPlayerPatch)}: CalamityKillPlayerPatch successfully applied";
        Utilities.LogMessage(message2, LogType.Debug);
    }
}