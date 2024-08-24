using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImprovedRespawning.Assets.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static ImprovedRespawning.Assets.Misc.Constants;

namespace ImprovedRespawning.Assets.Misc;

public static class Utilities
{
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
            const string message = $"{nameof(TryGetPlayerToSpawnOn)}: RespawnOnPlayer config value is false";
            LogMessage(message, LogType.Debug);
            return false;
        }
        
        if (!ImprovedRespawningModSystem.Instance.BossActive || Main.netMode == NetmodeID.SinglePlayer)
        {
            const string message = $"{nameof(TryGetPlayerToSpawnOn)}: No boss active or single player";
            LogMessage(message, LogType.Debug);
            return false;
        }
        
        if (Main.dedServ)
        {
            const string message = $"{nameof(TryGetPlayerToSpawnOn)}: somehow called on server";
            LogMessage(message, LogType.Warning);
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
    /// Logs message into server console, client and server logs where appropriate
    /// </summary>
    public static void LogMessage(string message, LogType logType)
    {
        
        StringBuilder sb = new();
        sb.Append($"[Improved Respawning {VERSION}] ");
        string logPrefix = logType switch
        {
            LogType.Debug => "Debug: ",
            LogType.Log => "Log: ",
            LogType.Warning => "Warning: ",
            LogType.Error => "Error: ",
            LogType.Important => ""
        };
        sb.Append(logPrefix);
        sb.Append(message);
        
        string finalMessage = sb.ToString();

        if (Main.netMode == NetmodeID.Server)
        {
            ConsoleColor targetColor = logType switch
            {
                LogType.Debug => DEBUG_COLOR,
                LogType.Log => LOG_COLOR,
                LogType.Warning => WARNING_COLOR,
                LogType.Error => ERROR_COLOR,
                LogType.Important => LOG_COLOR,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType,
                    "strictly speaking possible to happen but cmon")
            };
            Console.ForegroundColor = targetColor;
            Console.WriteLine(finalMessage);
            Console.ResetColor();
        }

        Mod mod = MainClasses.ImprovedRespawning.Instance;

        switch (logType)
        {
            case LogType.Debug:
                mod.Logger.Debug(finalMessage);
                break;
            case LogType.Log:
                mod.Logger.Info(finalMessage);
                break;
            case LogType.Warning:
                mod.Logger.Warn(finalMessage);
                break;
            case LogType.Error:
                mod.Logger.Error(finalMessage);
                break;
            case LogType.Important:
                mod.Logger.Info(finalMessage);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
        }
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