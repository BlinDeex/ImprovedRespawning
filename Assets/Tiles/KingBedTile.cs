using ImprovedRespawning.Assets.Items;
using ImprovedRespawning.Assets.MainClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ImprovedRespawning.Assets.Tiles;

public class KingBedTile : ModTile
{
    public const int NEXT_STYLE_HEIGHT = 38;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSleptIn[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.IsValidSpawnPoint[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
        DustType = 304;
        AdjTiles = [79];
        TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
        TileObjectData.newTile.CoordinateHeights =
        [16, 18];
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
        TileObjectData.addTile(Type);
        AddMapEntry(new Color(235, 192, 52), Language.GetText("ItemName.Bed"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth,
        ref int frameHeight, ref int extraY)
    {
        width = 2;
        height = 2;
    }

    public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
    {
        info.VisualOffset.Y += 4f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        Tile tile = Main.tile[i, j];
        int spawnX = i - tile.TileFrameX / 18 + (tile.TileFrameX >= 72 ? 5 : 2);
        int spawnY = j + 2;
        if (tile.TileFrameY % 38 != 0) spawnY--;
        if (!Player.IsHoveringOverABottomSideOfABed(i, j))
        {
            if (!player.IsWithinSnappngRangeToTile(i, j, 96)) return true;
            player.GamepadEnableGrappleCooldown();
            player.sleeping.StartSleeping(player, i, j);
        }
        else if (player.SpawnX == spawnX && player.SpawnY == spawnY)
        {
            player.RemoveSpawn();
            Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
        }
        else
        {
            Main.LocalPlayer.GetModPlayer<ImprovedRespawningPlayer>().KingBedSpawn = true;
            player.ChangeSpawn(spawnX, spawnY);
            Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
        }

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (Player.IsHoveringOverABottomSideOfABed(i, j))
        {
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<KingBed>();
            return;
        }

        if (!player.IsWithinSnappngRangeToTile(i, j, 96)) return;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = 5013;
    }
}