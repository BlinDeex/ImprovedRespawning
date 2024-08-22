using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImprovedRespawning.Assets.UI;
public class SelectBuffsEntry : UIElement
{
    private readonly Asset<Texture2D> buffsEntry =
        ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/BuffsEntry");
    private readonly Asset<Texture2D> buffsInnerArea =
        ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/BuffsEntryInnerArea");
    
    private const float MAX_RECTANGLE_HEIGHT = 270f;
    private const float ANIMATION_SPEED = 0.04f;
    private const Easings.EasingsList easingType = Easings.EasingsList.OutQuad;
    private readonly Color hoveringTitleColor = new(150, 150, 150, 200);
    private readonly Point listDims = new(500, 222);
    private readonly Color notHoveringTitleColor = new(100, 100, 100, 200);
    private BuffsList buffsList;
    private float currentAnimationValue;
    private Rectangle currentInnerAreaRec;
    private bool hoveringTitle;
    private bool innerAreaOpen;
    private bool listAppended;
    private MultilineTextBox searchBar;
    public SelectBuffsEntry()
    {
        SetPadding(0f);
        Width.Set(550f, 0f);
        Height.Set(30f, 0f);
        OverflowHidden = false;
    }
    
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Height.Set(30f + currentInnerAreaRec.Height, 0f);
        Rectangle targetRect = GetDimensions().ToRectangle();
        Rectangle rectangle = targetRect;
        rectangle.Height = 30;
        targetRect = rectangle;
        hoveringTitle = targetRect.Contains(Main.MouseScreen.ToPoint());
        Vector2 textPos = new(targetRect.X + 10, targetRect.Y + 8);
        Color targetTitleColor = hoveringTitle ? hoveringTitleColor : notHoveringTitleColor;
        spriteBatch.Draw(buffsEntry.Value, targetRect, targetTitleColor);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value,
            "Buffs you will always spawn with", textPos, Color.White, 0f, Vector2.Zero, new Vector2(0.8f, 0.8f));
        currentAnimationValue += innerAreaOpen ? ANIMATION_SPEED : -ANIMATION_SPEED;
        currentAnimationValue = MathHelper.Clamp(currentAnimationValue, 0f, 1f);
        float lerpedValue = Easings.RunEasingType(easingType, currentAnimationValue);
        int targetX = (int)MathHelper.Lerp(0f, MAX_RECTANGLE_HEIGHT, lerpedValue);
        currentInnerAreaRec = new Rectangle(targetRect.X, targetRect.Y + 30, targetRect.Width, targetX);
        spriteBatch.Draw(buffsInnerArea.Value, currentInnerAreaRec, Color.Gray);
        
        switch (Math.Abs(lerpedValue - 1f))
        {
            case < 0.001f when !listAppended:
                ConstructBuffList();
                break;
            case > 0.001f when listAppended:
                RemoveChild(buffsList);
                buffsList = null;
                listAppended = false;
                break;
        }
    }

    private void ConstructBuffList()
    {
        listAppended = true;
        buffsList = new BuffsList(this);
        buffsList.Height.Set(listDims.Y, 0f);
        buffsList.Width.Set(listDims.X, 0f);
        buffsList.Left.Set(37f, 0f);
        buffsList.Top.Set(74f, 0f);
        Append(buffsList);
        if (searchBar != null) return;
        searchBar = new MultilineTextBox(1, 476)
        {
            HAlign = 0.5f
        };
        searchBar.Top.Set(38f, 0f);
        Append(searchBar);
    }
    
    public override void LeftClick(UIMouseEvent evt)
    {
        if (hoveringTitle)
        {
            BuffsList list = buffsList;
            if (list?.DraggingHandle != true)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                innerAreaOpen = !innerAreaOpen;
            }
        }

        base.LeftClick(evt);
    }
    private sealed class BuffsList : UIGrid
    {
        private const float MAX_HANDLE_DRAG = 246f;
        private const float SCROLL_SENSITIVITY = 0.2f;

        private readonly Asset<Texture2D> handle =
            ModContent.Request<Texture2D>("ImprovedRespawning/Assets/Images/ScrollBarHandle");

        private readonly Color handleNotHoveringColor = new(200, 200, 200, 255);
        private readonly Point leftHandleOffset = new(-31, -40);
        private readonly Point rightHandleOffset = new(487, -40);
        private readonly GridScrollBar scrollbar;
        private readonly SelectBuffsEntry selectBuffsEntry;
        private bool clickedLeftMouse;
        private float currentHandleDrag;
        private bool lastFrameClicked;
        private string lastSearchBar = "";
        private Vector2 mouseDelta;
        private Vector2 mousePosLastFrame;
        private bool releasedLeftMouse;
        
        public bool DraggingHandle { get; private set; }

        public BuffsList(SelectBuffsEntry entry)
        {
            selectBuffsEntry = entry;
            ImprovedRespawningModSystem modSystem = ModContent.GetInstance<ImprovedRespawningModSystem>();
            ImprovedRespawningPlayer player = Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>();
            
            foreach (KeyValuePair<string, int> buff in modSystem.Buffs)
            {
                int value = buff.Value;
                BuffEntry newEntry = new(value, ToggleBuff, buff.Key, player.BuffsToSpawnWith.Contains(buff.Value));
                Add(newEntry);
            }

            foreach (KeyValuePair<string, int> validBuff in modSystem.ModdedBuffs)
            {
                int value2 = validBuff.Value;
                BuffEntry newEntry2 = new(value2, ToggleBuff, validBuff.Key, player.BuffsToSpawnWith.Contains(validBuff.Value));
                Add(newEntry2);
            }

            scrollbar = new GridScrollBar();
            SetScrollbar(scrollbar);
            Append(scrollbar);
        }

        private void UpdateSearch()
        {
            MultilineTextBox searchBar = selectBuffsEntry.searchBar;
            string currentSearchBar = searchBar?.MainText;
            bool changed = (currentSearchBar != "" && currentSearchBar != lastSearchBar) ||
                           (currentSearchBar == "" && lastSearchBar != "");
            lastSearchBar = currentSearchBar;
            ImprovedRespawningModSystem respawning = ModContent.GetInstance<ImprovedRespawningModSystem>();
            if (searchBar == null || !changed) return;
            Clear();
            ImprovedRespawningPlayer player = Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>();

            foreach ((string key, int value) in respawning.Buffs)
            {
                if (key.ContainsValue(currentSearchBar, StringComparison.OrdinalIgnoreCase))
                {
                    BuffEntry newEntry = new(value, ToggleBuff, key, player.BuffsToSpawnWith.Contains(value));
                    Add(newEntry);
                }
            }

            foreach ((string key, int value2) in respawning.ModdedBuffs)
            {
                if (key.ContainsValue(currentSearchBar, StringComparison.OrdinalIgnoreCase))
                {
                    BuffEntry newEntry2 = new(value2, ToggleBuff, key, player.BuffsToSpawnWith.Contains(value2));
                    Add(newEntry2);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CheckInput();
            Rectangle listRect = GetDimensions().ToRectangle();
            
            Rectangle targetLeftHandleRect = new(listRect.X + leftHandleOffset.X, listRect.Y - 40 + (int)currentHandleDrag, 20, 16);
            Rectangle targetRightHandleRect = new(listRect.X + rightHandleOffset.X, listRect.Y - 40 + (int)currentHandleDrag, 20, 16);
            
            bool hoverLeftHandle = targetLeftHandleRect.Contains(Main.MouseScreen.ToPoint());
            bool hoverRightHandle = targetRightHandleRect.Contains(Main.MouseScreen.ToPoint());
            
            Color targetLeftHandleColor = hoverLeftHandle ? Color.White : handleNotHoveringColor;
            Color targetRightHandleColor = hoverRightHandle ? Color.White : handleNotHoveringColor;
            
            spriteBatch.Draw(handle.Value, targetLeftHandleRect, targetLeftHandleColor);
            spriteBatch.Draw(handle.Value, targetRightHandleRect, targetRightHandleColor);
            
            if (clickedLeftMouse && (hoverLeftHandle || hoverRightHandle))
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                DraggingHandle = true;
            }

            if (releasedLeftMouse)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                DraggingHandle = false;
            }

            if (DraggingHandle)
            {
                currentHandleDrag += mouseDelta.Y;
                currentHandleDrag = MathHelper.Clamp(currentHandleDrag, 0f, MAX_HANDLE_DRAG);
            }

            scrollbar.CurrentOffset = currentHandleDrag;
            UpdateSearch();
        }

        private void CheckInput()
        {
            bool mouseLeft = Main.mouseLeft;
            if (mouseLeft && !lastFrameClicked && !clickedLeftMouse)
                clickedLeftMouse = true;
            else
                clickedLeftMouse = false;
            
            if (!mouseLeft && lastFrameClicked && !releasedLeftMouse)
                releasedLeftMouse = true;
            else
                releasedLeftMouse = false;
            
            lastFrameClicked = Main.mouseLeft;
            mouseDelta = Main.MouseScreen - mousePosLastFrame;
            mousePosLastFrame = Main.MouseScreen;
        }

        private static void ToggleBuff(int ID)
        {
            if (!Main.LocalPlayer.TryGetModPlayer(out ImprovedRespawningPlayer player))
            {
                Utilities.Log("couldnt get modplayer in ToggleBuff!", true);
                return;
            }

            if (!player.BuffsToSpawnWith.Remove(ID))
            {
                player.BuffsToSpawnWith.Add(ID);
            }
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            currentHandleDrag += -SCROLL_SENSITIVITY * evt.ScrollWheelValue;
            currentHandleDrag = MathHelper.Clamp(currentHandleDrag, 0f, MAX_HANDLE_DRAG);
        }

        private class GridScrollBar : UIScrollbar
        {
            public float CurrentOffset { get; set; }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                ViewPosition = CurrentOffset / MAX_HANDLE_DRAG * (MaxViewSize - ViewSize);
            }
        }
    }
}

internal class BuffEntry : UIElement
{
    private readonly Color disabledColor = new(50, 50, 50, 255);
    private readonly Color enabledColor = new(200, 200, 200, 255);
    private readonly string hoverText;
    private readonly int ID;
    private readonly Action<int> toggleAction;
    private bool hovering;
    private bool isEnabled;

    public BuffEntry(int ID, Action<int> toggleAction, string hoverText, bool enabled)
    {
        isEnabled = enabled;
        this.hoverText = hoverText;
        this.ID = ID;
        this.toggleAction = toggleAction;
        Height.Set(32f, 0f);
        Width.Set(32f, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Texture2D targetTex = TextureAssets.Buff[ID].Value;
        Rectangle targetRect = GetDimensions().ToRectangle();
        Color targetColor = isEnabled ? enabledColor : disabledColor;
        targetColor *= hovering ? 1.5f : 1f;
        spriteBatch.Draw(targetTex, targetRect, targetColor);
        if (hovering) Main.instance.MouseText(hoverText);
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        hovering = true;
        base.MouseOver(evt);
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        hovering = false;
        base.MouseOut(evt);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        toggleAction(ID);
        SoundEngine.PlaySound(SoundID.MenuTick);
        isEnabled = !isEnabled;
        base.LeftClick(evt);
    }
}