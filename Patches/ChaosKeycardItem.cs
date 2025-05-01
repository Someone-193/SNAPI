using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Keycards.Snake;
using SNAPI.Events.EventArgs;
using SNAPI.Features;
using UnityEngine;
using SPlayer = SNAPI.Events.Handlers.SnakePlayer;
using static HarmonyLib.AccessTools;
namespace SNAPI.Patches
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
            
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_2) + 8;
            newInstructions.InsertRange(index, 
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, Method(typeof(ChaosKeycardItemTranspiler), nameof(OnSwitchedAxes))),
                ]);
            
            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;
            
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        public static readonly List<Vector2Int> StartSegments = [new(9, 6), new(8, 6), new(7, 6), new(6, 6), new(5, 6)];
        public static void OnMoved(ChaosKeycardItem keycard, SnakeNetworkMessage msg)
        {
            try
            {
                SnakeContext context = SnakeContext.Get(keycard.ItemSerial);
                if (context == null)
                {
                    Log.Debug("OnMoved failed to fire because SnakeContext could not be acquired.");
                    return;
                }
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
                    if (!isNew) SPlayer.OnScore(new ScoreEventArgs(context));
                    context.NextFoodPosition = msg.NextFoodPosition ?? throw new NullReferenceException("Next food position was null!");
                }

                if (isNew)
                {
                    context.Segments = new List<Vector2Int>(StartSegments);
                    SPlayer.OnStartingNewSnake(new StartingNewSnakeEventArgs(context));
                }
                
                if (msg.MoveOffset != Vector2Int.zero && !isNew)
                {
                    context.Direction = msg.MoveOffset;
                    Vector2Int next = ConfineToPlayableArea(context.Segments.Last() + context.Direction);
                    context.Segments.Insert(0, next);
                    if (!newFood) context.Segments.RemoveAt(context.Segments.Count - 1);
                    SPlayer.OnSnakeMove(new SnakeMoveEventArgs(context));
                }

                if (gameOver)
                {
                    SPlayer.OnGameOver(new GameOverEventArgs(context));
                    SPlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                    context.Timer.Reset();
                    context.TotalTimePlaying = TimeSpan.Zero;
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
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public static void OnSwitchedAxes(ChaosKeycardItem keycard, object obj)
        {
            try
            {
                Type T = obj.GetType();
                int x = (sbyte)(T.GetField("x")?.GetValue(obj) ?? 0);
                int y = (sbyte)(T.GetField("y")?.GetValue(obj) ?? 0);
                
                SnakeContext context = SnakeContext.Get(keycard.ItemSerial);
                if (context == null)
                {
                    Log.Debug("OnMoved failed to fire because SnakeContext could not be acquired.");
                    return;
                }
                context.Direction = new Vector2Int(x, y);
                
                SPlayer.OnSwitchAxes(new SwitchAxesEventArgs(context));
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public static Vector2Int ConfineToPlayableArea(Vector2Int input)
        {
            const int XSize = 18;
            const int YSize = 11;
            int x = input.x;
            int y = input.y;
            while (x < 0)
                x += XSize;
            while (y < 0)
                y += YSize;
            return new Vector2Int(x % XSize, y % YSize);
        }
    }
    // [HarmonyPatch(typeof(ChaosKeycardItem), nameof(ChaosKeycardItem.GetNewEngine))]
    // public class SnakeEnginePostfix
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(ChaosKeycardItem __instance)
    //     {
    //         // THANKS NORTHWOOD
    //         Log.Warn(__instance._snakeAreaSize); // (18, 11)
    //         Log.Warn(__instance._snakeMaxLength); // 100
    //         Log.Warn(__instance._snakeStartLength); // 5
    //     }
    // }
}