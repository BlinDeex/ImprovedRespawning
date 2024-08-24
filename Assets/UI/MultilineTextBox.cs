using ImprovedRespawning.Assets.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImprovedRespawning.Assets.UI;

public class MultilineTextBox : UIElement
{
    private readonly Texture2D tex = ModContent
        .Request<Texture2D>("ImprovedRespawning/Assets/Images/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
    
    private readonly bool drawBackground;
    private readonly int maxLineLength;
    private readonly int maxLines;
    private readonly Color maxWarningColor = Color.DarkRed;
    private readonly bool readonlyTextBox;
    private readonly DynamicSpriteFont targetFont = FontAssets.MouseText.Value;
    private readonly Color textBoxColor = new(80, 80, 80, 255);
    private readonly Color textColor = Color.Black;
    private readonly Vector2 textOffset = new(10f, 3f);
    private readonly float titleScale;
    private bool clickedLeftMouse;
    private int currentTickingTimer;
    private float currentWarninglerp;
    private bool hasFocus;
    private bool hovering;
    private bool lastFrameClicked;
    private bool releasedLeftMouse;
    private bool symbolVisible;
    private bool textBoxFull;

    public MultilineTextBox(int maxLines, int textBoxWidth, string title = "", float titleScale = 1f,
        bool readonlyTextBox = false, bool drawBackground = true)
    {
        this.drawBackground = drawBackground;
        this.readonlyTextBox = readonlyTextBox;
        Height.Set(maxLines * 22f, 0f);
        if (maxLines == 1) Height.Set(24f, 0f);
        this.titleScale = titleScale;
        this.title = title;
        this.maxLines = maxLines;
        Width.Set(textBoxWidth, 0f);
        maxLineLength = textBoxWidth - 24;
        SetPadding(0f);
        Width.Set(textBoxWidth, 0f);
    }

    private string title;

    public string MainText { get; private set; } = "";

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
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        CheckInput();
        if (clickedLeftMouse) LeftClick();
        Rectangle targetRect = GetDimensions().ToRectangle();
        Color targetTextBoxColor = textBoxColor;
        if (hovering) targetTextBoxColor *= 1.2f;
        if (drawBackground) spriteBatch.Draw(tex, targetRect, targetTextBoxColor);
        if (!string.IsNullOrEmpty(title))
        {
            float titleLength = targetFont.MeasureString(title).X * titleScale;
            float num = targetFont.MeasureString(title).Y * titleScale;
            float XTitleoffset = targetRect.Width / 2f - titleLength / 2f;
            float YTitleoffset = -num + 4f;
            Vector2 titleOffset = new Vector2(XTitleoffset, YTitleoffset);
            ChatManager.DrawColorCodedString(spriteBatch, targetFont, title,
                targetRect.Location.ToVector2() + titleOffset, textColor, 0f, Vector2.Zero,
                new Vector2(titleScale, titleScale));
        }

        string targetText = Utilities.FormatText(targetFont, MainText, maxLines, maxLineLength, out int linesUsed,
            out textBoxFull, out float lastLineWidth);
        Color targetTextColor = Color.Lerp(textColor, maxWarningColor, currentWarninglerp);
        targetTextColor *= hovering ? 1.2f : 1f;
        Vector2 targetMainTextPos = new Vector2(targetRect.X + textOffset.X, targetRect.Y + textOffset.Y);
        ChatManager.DrawColorCodedString(spriteBatch, targetFont, targetText, targetMainTextPos, targetTextColor, 0f,
            Vector2.Zero, Vector2.One);
        if (readonlyTextBox) return;
        Input();
        currentWarninglerp -= 0.02f;
        hovering = targetRect.Contains(Main.MouseScreen.ToPoint());
        if (!hasFocus || !symbolVisible) return;
        int Yoffset = (linesUsed - 1) * 20;
        Vector2 offset = new Vector2(lastLineWidth, Yoffset);
        ChatManager.DrawColorCodedString(spriteBatch, targetFont, "|", targetMainTextPos + offset, targetTextColor, 0f,
            Vector2.Zero, Vector2.One);
    }

    private void Input()
    {
        if (!hasFocus) return;
        if (Utilities.KeyTyped(Keys.Escape))
        {
            hasFocus = false;
            return;
        }

        HandleTextInput();
        currentTickingTimer++;
        if (currentTickingTimer < 30) return;
        symbolVisible = !symbolVisible;
        currentTickingTimer = 0;
    }

    private void LeftClick()
    {
        if (!hovering && hasFocus)
        {
            hasFocus = false;
            return;
        }

        if (hovering && !hasFocus)
        {
            hasFocus = true;
            currentTickingTimer = 0;
            symbolVisible = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private void HandleTextInput()
    {
        PlayerInput.WritingText = true;
        Main.instance.HandleIME();
        string newText = Main.GetInputText(MainText);
        if (newText != MainText) SoundEngine.PlaySound(SoundID.MenuTick);
        if (newText.Length > MainText.Length && textBoxFull)
        {
            currentWarninglerp += 0.2f;
            currentWarninglerp = MathHelper.Clamp(currentWarninglerp, 1f, 0f);
            return;
        }

        MainText = newText;
    }
}