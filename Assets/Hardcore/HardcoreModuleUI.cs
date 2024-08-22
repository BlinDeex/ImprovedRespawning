using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImprovedRespawning.Assets.Hardcore;

public class HardcoreModuleUI : UIState
{
		private UIText worldDeathsText;
		private UIText playerDeathsText;
		private UIElement area;
		private const int PRIMARY_TOP_OFFSET = 18;
		private const int SECONDARY_TOP_OFFSET = PRIMARY_TOP_OFFSET - 18;
		private DynamicSpriteFont font = FontAssets.MouseText.Value;
		private int leftOffset = -420;
		private float font_size = 0.8f;
		private const int AREA_INITIAL_WIDTH = 120;
		
		public override void OnInitialize()
		{
			area = new BasicUIElement();
			area.Left.Set(leftOffset, 1f);
			area.Top.Set(0, 0f);
			area.Width.Set(100, 0f);
			area.Height.Set(42, 0f);
			area.SetPadding(0);
			area.MarginBottom = 0;
			area.MarginLeft = 0;
			area.MarginRight = 0;
			area.MarginTop = 0;
			

			worldDeathsText = new UIText("FAILED TO INIT", font_size);
			worldDeathsText.Width.Set(138, 0f);
			worldDeathsText.Height.Set(34, 0f);
			worldDeathsText.Top.Set(-34, 0f);
			worldDeathsText.Left.Set(0, 0f);
			worldDeathsText.SetPadding(0);
			
			playerDeathsText = new UIText("FAILED TO INIT", font_size);
			playerDeathsText.Width.Set(138, 0f);
			playerDeathsText.Height.Set(34, 0f);
			playerDeathsText.Top.Set(-10, 0f);
			playerDeathsText.Left.Set(0, 0f);
			playerDeathsText.SetPadding(0);
			
			
			area.Append(worldDeathsText);
			area.Append(playerDeathsText);
			Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) 
		{
			ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();

			if (!config.EnableMaximumLivesPerWorld && !config.EnableMaximumLivesPerPlayer) return;
			if (!config.EnableHardcoreUI) return;
			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{

			ImprovedRespawningConfig config = ModContent.GetInstance<ImprovedRespawningConfig>();

			if (!config.EnableMaximumLivesPerWorld && !config.EnableMaximumLivesPerPlayer) return;

			int worldDeaths = HardcoreModuleModSystem.Instance.TotalWorldDeaths;
			int maxWorldDeaths = config.MaximumLivesPerWorld;
			int thisPlayerDeaths = 0;
			int maxPlayerDeaths = config.MaximumLivesPerPlayer;
			
			if (HardcoreModuleModSystem.Instance.PlayerDeaths.TryGetValue(Main.LocalPlayer.name, out int death))
			{
				thisPlayerDeaths = death;
			}

			if (config.EnableMaximumLivesPerPlayer && !config.EnableMaximumLivesPerWorld)
			{
				playerDeathsText.Top.Set(PRIMARY_TOP_OFFSET, 0);
			}
			else if (config.EnableMaximumLivesPerWorld && !config.EnableMaximumLivesPerPlayer)
			{
				worldDeathsText.Top.Set(PRIMARY_TOP_OFFSET,0);
			}
			else
			{
				playerDeathsText.Top.Set(SECONDARY_TOP_OFFSET,0);
				worldDeathsText.Top.Set(PRIMARY_TOP_OFFSET,0);
			}
			
			worldDeathsText.SetText(config.EnableMaximumLivesPerWorld ? $"World: {worldDeaths} / {maxWorldDeaths}" : "");
			playerDeathsText.SetText(config.EnableMaximumLivesPerPlayer ? $"Player: {thisPlayerDeaths} / {maxPlayerDeaths}" : "");
			float deathsTextOffset = -50 + font.MeasureString(worldDeathsText.Text).X / 2f * font_size;
			float playerDeathsOffset = -50 + font.MeasureString(playerDeathsText.Text).X / 2f * font_size;
			worldDeathsText.Left.Set(deathsTextOffset,0);
			playerDeathsText.Left.Set(playerDeathsOffset,0);
			base.Update(gameTime);
		}
}