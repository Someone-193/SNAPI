using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Keycards.Snake;
using Mirror;
using SnakeAPI.Events;
using SnakeAPI.Events.EventArgs;
using SnakeAPI.Features;
using UnityEngine;
using SPlayer = SnakeAPI.Events.SnakePlayer;
using static HarmonyLib.AccessTools;
namespace SnakeAPI.Patches
{
    [HarmonyPatch(typeof(ChaosKeycardItem), nameof(ChaosKeycardItem.ServerProcessCustomCmd))]
    public class ChaosKeycardItemTranspiler
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            // int index = newInstructions.FindIndex(instruction => instruction.Calls(Constructor(typeof(SnakeNetworkMessage), [typeof(NetworkReader)])))
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) - 1;
            newInstructions.InsertRange(index, 
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, Method(typeof(ChaosKeycardItemTranspiler), nameof(OnMoved))),
                ]);
            
            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;
            
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        public static void OnMoved(ChaosKeycardItem keycard, SnakeNetworkMessage msg)
        {
            SnakeContext context = SnakeContext.Get(keycard.ItemSerial);
            bool isNew = msg.HasFlag(SnakeNetworkMessage.SyncFlags.GameReset);
            bool gameOver = msg.HasFlag(SnakeNetworkMessage.SyncFlags.GameOver);
            bool newFood = msg.HasFlag(SnakeNetworkMessage.SyncFlags.HasNewFood);
            if (!context.Timer.IsRunning && !isNew)
            {
                SPlayer.OnResumingSnake(new ResumingSnakeEventArgs(context));
                context.Playing = true;
            }
            context.Timer.Restart();

            if (newFood)
            {
                SPlayer.OnScore(new ScoreEventArgs(context));
                context.NextFoodPosition = msg.NextFoodPosition ?? throw new NullReferenceException("Next food position was null!");
            }
            
            if (msg.MoveOffset != Vector2Int.zero)
            {
                context.Direction = msg.MoveOffset;
                Vector2Int next = ConfineToPlayableArea(context.Segments.Last() + context.Direction);
                context.Segments.Insert(0, next);
                if (!newFood) context.Segments.RemoveAt(context.Segments.Count - 1);
                SPlayer.OnSnakeMove(new SnakeMoveEventArgs(context));
            }
            
            if (isNew)
                SPlayer.OnStartingNewSnake(new StartingNewSnakeEventArgs(context));

            if (gameOver)
            {
                SPlayer.OnGameOver(new GameOverEventArgs(context));
                SPlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                context.Timer.Reset();
                context.Playing = false;
            }

            // good to use to figure out how these messages work
            
            // Log.Warn($"Flags: {msg.Flags.ToString()}");
            // Log.Warn($"Move Offset: {msg.MoveOffset.ToString()}");
            // Log.Warn($"Next Food Position: {msg.NextFoodPosition?.ToString() ?? "NULL"}");
            // if (msg.Segments == null)
            // {
            //     Log.Warn("No Segments");
            //     return;
            // }
            // string txt = "Segments: ";
            // foreach (Vector2Int segment in msg.Segments)
            // {
            //     txt += $"{segment.ToString()}, ";
            // }
            // if (txt.Length > 1) txt = txt.Remove(txt.Length - 2, 2);
            // Log.Warn(txt);
        }
        internal static Vector2Int ConfineToPlayableArea(Vector2Int input)
        {
            return Vector2Int.zero;
            const int XSize = 0;
            const int YSize = 0;
            int x = input.x;
            int y = input.y;
            while (x < 0)
                x += XSize;
            while (y < 0)
                y += YSize;
            return new Vector2Int(x % XSize, y % YSize);
        }
    }
    [HarmonyPatch(typeof(ChaosKeycardItem), nameof(ChaosKeycardItem.GetNewEngine))]
    public class SnakeEnginePostfix
    {
        [HarmonyPostfix]
        public static void Postfix(ChaosKeycardItem __instance)
        {
            Log.Warn(__instance._snakeAreaSize);
            Log.Warn(__instance._snakeMaxLength);
            Log.Warn(__instance._snakeStartLength);
        }
    }
}