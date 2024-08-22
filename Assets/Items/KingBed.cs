using System.Collections.Generic;
using ImprovedRespawning.Assets.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets.Items;

public class KingBed : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<KingBedTile>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 2000;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine ttl = new TooltipLine(Mod, "", "Bed so nice you dont even need a house for it!");
        tooltips.Add(ttl);
    }
    
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Wood, 50).AddIngredient(ItemID.Silk, 20).AddIngredient(ItemID.GoldBar, 8)
            .AddTile(TileID.WorkBenches).Register();
        CreateRecipe().AddIngredient(ItemID.Wood, 50).AddIngredient(ItemID.Silk, 20)
            .AddIngredient(ItemID.PlatinumBar, 8).AddTile(TileID.WorkBenches).Register();
    }
}