using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ImprovedRespawning.Assets;

public static class Utilities
{
    public const string VERSION = "1.1.1v";
    public static bool KeyTyped(Keys key)
    {
        return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
    }
    
    public static void BroadcastOrNewText(string message, Color color)
    {
        if (Main.dedServ)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            return;
        }
        
        Main.NewText(message, color);
    }

    public static bool AnyBossNPCS()
    {
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.IsABoss()) return true;
        }

        return false;
    }
    
    /// <summary>
    /// should only be called on multiplayer clients, handles all conditions if its appropriate to spawn on player
    /// </summary>
    /// <returns>alive and closest to the boss player</returns>
    public static bool TryGetPlayerToSpawnOn(out Player player)
    {
        player = null;

        if (!ModContent.GetInstance<ImprovedRespawningConfig>().RespawnOnPlayer)
        {
            return false;
        }
        
        if (!ImprovedRespawningModSystem.Instance.BossActive || Main.netMode == NetmodeID.SinglePlayer)
        {
            return false;
        }
        
        if (Main.dedServ)
        {
            Log("TryGetPlayerToSpawnOn called on server!", true);
            BroadcastOrNewText("TryGetPlayerToSpawnOn called on server!", Color.Red);
            return false;
        }

        float smallestDistToBoss = float.MaxValue;

        NPC currentBoss = Main.npc.First(x => x.IsABoss());
        Vector2 bossPos = currentBoss.position;
        
        foreach (var potentialPlayer in Main.ActivePlayers)
        {
            if (potentialPlayer.dead) continue;
            float distToBoss = Vector2.DistanceSquared(bossPos, potentialPlayer.position);
            
            if (!(distToBoss < smallestDistToBoss)) continue;
            
            smallestDistToBoss = distToBoss;
            player = potentialPlayer;
        }

        return player != null;
    }

    private static bool IsABoss(this NPC npc)
    {
        if (npc is null || !npc.active)
            return false;
        if (npc.boss && npc.type != NPCID.MartianSaucerCore)
            return true;
        if (npc.type is NPCID.EaterofWorldsBody or NPCID.EaterofWorldsHead or NPCID.EaterofWorldsTail)
            return true;

        return ImprovedRespawningModSystem.Instance.calBosses.Contains(npc.type);
    }

    public static bool ContainsValue(this string source, string toCheck, StringComparison comp)
    {
        return source != null && source.Contains(toCheck, comp);
    }

    /// <summary>
    /// logging for dedicated server
    /// </summary>
    public static void ConsoleMessage(string message, bool isFailure = false)
    {
        Mod mod = ImprovedRespawning.Instance;
        Console.ForegroundColor = isFailure ? ConsoleColor.Red : ConsoleColor.DarkCyan;
        StringBuilder sb = new();
        sb.Append("[ImprovedRespawning ");
        sb.Append(VERSION);
        sb.Append("] ");
        sb.Append(message);
        mod.Logger.Info(sb.ToString());
        Console.WriteLine(sb.ToString());
        
        Console.ResetColor();
    }

    public static void Log(string message, bool isFailure)
    {
        Mod mod = ModContent.GetInstance<ImprovedRespawning>();
        if (isFailure)
        {
            mod.Logger.Error($"RESPAWNERROR: {message}");
            return;
        }
        
        mod.Logger.Info($"RESPAWNLOG: {message}");
    }
    
    /// <summary>
    /// formats given string into box shaped area which is calculated from <paramref name="maxLines"/> and <paramref name="maxLineLength"/>
    /// </summary>
    /// <returns></returns>
    public static string FormatText(DynamicSpriteFont currentFont, string targetText, int maxLines, float maxLineLength,
        out int linesUsed, out bool fullyFilled, out float lastLineWidth)
    {
        StringBuilder formattedText = new StringBuilder();
        StringBuilder workspaceText = new StringBuilder(targetText);
        StringBuilder currentLine = new StringBuilder();
        StringBuilder currentWord = new StringBuilder();
        List<string> words = [];
        int currentLines = 1;
        bool lastLine = currentLines == maxLines;
        fullyFilled = false;
        for (int i = 0; i < workspaceText.Length; i++)
        {
            currentWord.Append(workspaceText[i]);
            if (workspaceText[i] == ' ' || i == workspaceText.Length - 1)
            {
                words.Add(currentWord.ToString());
                currentWord.Clear();
            }
        }

        foreach (string word in words)
        {
            float wordLength = currentFont.MeasureString(word).X;
            float currentLineAvailableSpace = maxLineLength - currentFont.MeasureString(currentLine.ToString()).X;
            if (wordLength > maxLineLength)
            {
                StringBuilder longWord = new StringBuilder(word);
                StringBuilder fitsInCurrentLine = new StringBuilder();
                if (fullyFilled) break;
                for (int j = 0; j < longWord.Length; j++)
                {
                    fitsInCurrentLine.Append(longWord[j]);
                    float currentLength = currentFont.MeasureString(fitsInCurrentLine.ToString()).X;
                    if (fullyFilled) break;
                    if (currentLength >= currentLineAvailableSpace - 4f)
                    {
                        if (lastLine)
                        {
                            currentLine.Append(fitsInCurrentLine);
                            formattedText.Append(currentLine);
                            lastLineWidth = currentFont.MeasureString(currentLine.ToString()).X;
                            fullyFilled = true;
                            break;
                        }

                        currentLine.Append(fitsInCurrentLine);
                        formattedText.Append(currentLine);
                        formattedText.AppendLine();
                        currentLine.Clear();
                        currentLines++;
                        lastLine = currentLines == maxLines;
                        fitsInCurrentLine.Clear();
                        currentLineAvailableSpace = maxLineLength;
                    }

                    if (j == longWord.Length - 1) currentLine.Append(fitsInCurrentLine);
                }
            }
            else if (wordLength <= currentLineAvailableSpace)
            {
                currentLine.Append(word);
            }
            else if (wordLength > currentLineAvailableSpace)
            {
                if (!lastLine)
                {
                    formattedText.Append(currentLine);
                    formattedText.AppendLine();
                    currentLine.Clear();
                    currentLines++;
                    lastLine = currentLines == maxLines;
                    currentLine.Append(word);
                }
                else
                {
                    float leftLineLength = currentFont.MeasureString(currentLine.ToString()).X;
                    StringBuilder lastWord = new StringBuilder(word);
                    while (lastWord.Length > 1)
                    {
                        lastWord.Remove(lastWord.Length - 1, 1);
                        if (currentFont.MeasureString(lastWord.ToString()).X <= leftLineLength)
                        {
                            currentLine.Append(lastWord);
                            formattedText.Append(currentLine);
                            fullyFilled = true;
                            break;
                        }
                    }
                }
            }
        }

        if (currentLine.Length > 0 && !fullyFilled) formattedText.Append(currentLine);
        linesUsed = currentLines;
        lastLineWidth = currentFont.MeasureString(currentLine.ToString()).X;
        return formattedText.ToString();
    }
}