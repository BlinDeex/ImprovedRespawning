//#define DEBUGUI

using ImprovedRespawning.Assets.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImprovedRespawning.Assets.Hardcore;

public class BasicUIElement : UIPanel
{
	#if DEBUGUI
	private Texture2D whitePixel = ModContent
		.Request<Texture2D>("ImprovedRespawning/Assets/Images/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
	#endif
	
	private bool dragging;
	
	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		#if DEBUGUI
		CalculatedStyle dimensions = GetDimensions();
		Rectangle destRect = new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width,
			(int)dimensions.Height);
		spriteBatch.Draw(whitePixel, destRect, Color.Gray);
		#endif
	}
	
	public override void LeftMouseDown(UIMouseEvent evt)
	{
		if (!ModContent.GetInstance<ImprovedRespawningConfig>().EnableHardcoreUI) return;
		base.LeftMouseDown(evt);
		dragging = true;
	}

	public override void LeftMouseUp(UIMouseEvent evt)
	{
		base.LeftMouseUp(evt);
		dragging = false;
		Recalculate();
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		
		if (ContainsPoint(Main.MouseScreen))
		{
			Main.LocalPlayer.mouseInterface = true;
		}

		if (dragging)
		{
			Vector2 mouseDelta = Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>().MouseDelta;
			Left.Set(Left.Pixels + mouseDelta.X, 1f);
			Top.Set(Top.Pixels + mouseDelta.Y, 0f);
			Recalculate();
		}
		
		var parentSpace = Parent.GetDimensions().ToRectangle();
		if (GetDimensions().ToRectangle().Intersects(parentSpace)) return;
		Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
		Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
		Recalculate();
	}
}