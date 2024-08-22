using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImprovedRespawning.Assets.Hardcore;

[Autoload(Side = ModSide.Client)]
public class HardcoreModuleInterfaceLayer : ModSystem
{
    private UserInterface HardcoreUI;

    private HardcoreModuleUI HardcoreDisplay;

    public override void Load() 
    {
        HardcoreDisplay = new HardcoreModuleUI();
        HardcoreUI = new UserInterface();
        HardcoreUI.SetState(HardcoreDisplay);
    }

    public override void UpdateUI(GameTime gameTime) 
    {
        HardcoreUI?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) 
    {
        int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (resourceBarIndex != -1) 
        {
            layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                "ImprovedRespawning: Hardcore UI", delegate 
                {
                    HardcoreUI.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}