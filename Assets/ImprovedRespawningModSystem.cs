using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImprovedRespawning.Assets.Hardcore;
using ImprovedRespawning.Assets.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets;

public class ImprovedRespawningModSystem : ModSystem
{
    public readonly Dictionary<string, int> Buffs = new();
    public readonly Dictionary<string, int> ModdedBuffs = new();
    
    private SelectBuffsEntry entry;
    private int gameTicks;
    private bool lastFrameBossActive;
    internal bool BossActive { get; private set; }
    internal bool BossDisappeared { get; private set; }
    internal bool EventActive { get; private set; }

    public static ImprovedRespawningModSystem Instance { get; private set; }

    private readonly List<string> adminUsers = [];
    
    public string AuthCode { get; private set; } = "";
    
    public bool CalamityLoaded;

    public Mod CalamityMod;

    public int[] calBosses = [];

    public bool IsUserAdmin(string username)
    {
        return adminUsers.Contains(username);
    }
    
    /// <summary>
    /// run on server only
    /// </summary>
    /// <param name="username"></param>
    /// <param name="code"></param>
    public void TryAddAdminUser(string username, string code)
    {
        if (!Main.dedServ)
        {
            Utilities.Log("AddAdminUser called not on server!", true);
            return;
        }
        
        int targetUserID = Main.player.Where(x => x.name == username).Select(x => x.whoAmI).First();
        
        if (AuthCode != code)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Invalid code!"), Color.Red, targetUserID);
            return;
        }
        
        adminUsers.Add(username);
        ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Successfully added user as admin"), Color.Green, targetUserID);
    }

    public bool AreThereAnyAdmins()
    {
        return adminUsers.Count > 0;
    }

    public void ClearAdmins()
    {
        adminUsers.Clear();
    }

    public void PrintAuthMessage()
    {
        Utilities.ConsoleMessage($"auth code: {AuthCode}, use command /irauth {AuthCode} to get admin permissions,\nif theres one or more admins, config changes will only be accepted from them. use /ClearAdmins command to clear all admins");
    }

    public void RegenerateAuthCode()
    {
        int number = Main.rand.Next(100000, 1000000);
        AuthCode =  number.ToString();
    }

    public override void Load()
    {
        Instance = this;
    }

    public override void PostUpdateNPCs()
    {
        gameTicks++;
        if (gameTicks % 20 != 0) return;
        CheckNPCS();
        EventActive = Main.invasionType != 0;
    }

    public override void ClearWorld()
    {
        if (!Main.dedServ) return;
        adminUsers.Clear();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        InjectDropDown();
    }

    public override void PostSetupContent()
    {
        List<int> buffIds = new List<int>(BuffID.Count);

        for (int i = 0; i < BuffID.Count; i++)
        {
            if (!Main.buffNoSave[i] && TextureAssets.Buff[i] != null) buffIds.Add(i);
        }

        foreach (int buffID in buffIds)
        {
            try
            {
                Buffs.TryAdd(Lang.GetBuffName(buffID), buffID);
            }
            catch
            {
                // ignored
            }
        }

        int moddedBuffsCount = BuffLoader.BuffCount;
        for (int j = 0; j < moddedBuffsCount; j++)
        {
            ModBuff modBuff = BuffLoader.GetBuff(j);
            if (modBuff != null) ModdedBuffs.TryAdd(modBuff.Name, modBuff.Type);
        }
    }

    private void InjectDropDown()
    {
        object UIModConfigRef = ModContent.GetInstance<ImprovedRespawning>().ModConfigRef;
        UIList uiConfigUiList = (UIList)UIModConfigRef.GetType().GetField("mainConfigList", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(UIModConfigRef);
        if (uiConfigUiList == null) return;
        if (uiConfigUiList.Count != 37) return; //TODO: find better way
        entry = new SelectBuffsEntry();
        uiConfigUiList.Add(entry);
    }

    private void CheckNPCS()
    {
        lastFrameBossActive = BossActive;
        BossActive = Utilities.AnyBossNPCS();
        BossDisappeared = lastFrameBossActive && !BossActive;
    }
}