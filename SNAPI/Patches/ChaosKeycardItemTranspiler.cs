namespace SNAPI.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Keycards.Snake;
    using SNAPI.Events.EventArgs;
    using SNAPI.Features;
    using UnityEngine;
    using static HarmonyLib.AccessTools;
    using SPlayer = SNAPI.Events.Handlers.SnakePlayer;

    /// <summary>
    /// The class for the transpiler for InventorySystem.Items.Keycards.ChaosKeycardItem.ServerProcessCustomCmd.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Items.Keycards.ChaosKeycardItem), nameof(InventorySystem.Items.Keycards.ChaosKeycardItem.ServerProcessCustomCmd))]
    public class ChaosKeycardItemTranspiler
    {
        /// <summary>
        /// The segments of the Snake that are spawned in by default.
        /// </summary>
        public static readonly List<Vector2Int> StartSegments = [new(9, 5), new(8, 5), new(7, 5), new(6, 5), new(5, 5)];
        
        /// <summary>
        /// A transpiler for InventorySystem.Items.Keycards.ChaosKeycardItem.ServerProcessCustomCmd.
        /// </summary>
        /// <param name="instructions">The instructions to edit.</param>
        /// <returns>The edited instructions.</returns>
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1);
            newInstructions.InsertRange(
                index - 1, 
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, Method(typeof(ChaosKeycardItemTranspiler), nameof(OnMoved))),
                ]);
            
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_2);
            newInstructions.InsertRange(
                index + 8, 
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, Method(typeof(ChaosKeycardItemTranspiler), nameof(OnSwitchedAxes))),
                ]);
            
            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;
            
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        
        /// <summary>
        /// The method that fires when a SnakeNetworkMessage is sent.
        /// </summary>
        /// <param name="keycard">The <see cref="Exiled.API.Features.Items.Keycard"/> the player is holding.</param>
        /// <param name="msg">The <see cref="SnakeNetworkMessage"/> sent.</param>
        /// <exception cref="NullReferenceException">Thrown if SnakeNetworkMessage::NextFoodPosition is null despite SnakeNetworkMessage::SyncFlags containing SyncFlags.HasNewFood.</exception>
        public static void OnMoved(InventorySystem.Items.Keycards.ChaosKeycardItem keycard, SnakeNetworkMessage msg)
        {
            try
            {
                // good to use to figure out how these messages work

                // Log.Warn($"Flags: {msg.Flags.ToString()}");
                // Log.Warn($"Move Offset: {msg.MoveOffset.ToString()}");
                // Log.Warn($"Next Food Position: {msg.NextFoodPosition?.ToString() ?? "NULL"}");
                // if (msg.Segments == null)
                // {
                //     Log.Warn("No Segments");
                //     goto skip;
                // }
                // string txt = "Segments: ";
                // foreach (Vector2Int segment in msg.Segments)
                // {
                //     txt += $"{segment.ToString()}, ";
                // }
                // if (txt.Length > 1) txt = txt.Remove(txt.Length - 2, 2);
                // Log.Warn(txt);
                // skip:
                SnakeContext? context = SnakeContext.Get(keycard.ItemSerial);
                if (context == null)
                {
                    Log.Debug("OnMoved failed to fire because SnakeContext could not be acquired.");
                    return;
                }
                
                bool isNew = msg.HasFlag(SnakeNetworkMessage.SyncFlags.GameReset);
                bool gameOver = msg.HasFlag(SnakeNetworkMessage.SyncFlags.GameOver);
                bool newFood = msg.HasFlag(SnakeNetworkMessage.SyncFlags.HasNewFood);

                if (newFood)
                {
                    if (!isNew)
                    {
                        context.Score++;
                        SPlayer.OnScore(new ScoreEventArgs(context));
                    }
                    
                    context.NextFoodPosition = msg.NextFoodPosition ?? throw new NullReferenceException("Next food position was null!");
                }

                if (!context.Timer.IsRunning && context is { Initialized: true, Started: true, Stopping: false })
                {
                    SPlayer.OnResumingSnake(new ResumingSnakeEventArgs(context));
                    context.Playing = true;
                }
                
                context.Timer.Restart();

                if (context is { Initialized: true, Playing: false, Started: false } && (isNew || msg.MoveOffset != Vector2Int.zero))
                {   
                    context.Segments = new List<Vector2Int>(StartSegments);
                    context.Playing = true;
                    context.Timer.Restart();
                    context.Started = true;
                    SPlayer.OnStartingNewSnake(new StartingNewSnakeEventArgs(context));
                }
                
                if (msg.MoveOffset != Vector2Int.zero)
                {
                    if (!context.Initialized)
                    {
                        context.Initialized = true;
                        return;
                    }
                    
                    context.Direction = msg.MoveOffset;
                    Vector2Int next = ConfineToPlayableArea(context.Segments.First() + context.Direction);
                    context.Segments.Insert(0, next);
                    context.CurrentHeadPosition = next;
                    if (!newFood) 
                        context.Segments.RemoveAt(context.Segments.Count - 1);
                    SPlayer.OnSnakeMove(new SnakeMoveEventArgs(context));
                }
                
                if (gameOver)
                {
                    SPlayer.OnGameOver(new GameOverEventArgs(context));
                    SPlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                    context.Timer.Reset();
                    context.TotalTimePlaying = TimeSpan.Zero;
                    context.Playing = false;
                    context.Score = 0;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        
        /// <summary>
        /// Called when a player switches their direction.
        /// </summary>
        /// <param name="keycard">The <see cref="Exiled.API.Features.Items.Keycard"/> the player is holding.</param>
        /// <param name="obj">A strange backing class in the <see cref="InventorySystem.Items.Keycards.ChaosKeycardItem"/> which contains both an x sbyte and a y sbyte.</param>
        public static void OnSwitchedAxes(InventorySystem.Items.Keycards.ChaosKeycardItem keycard, object obj)
        {
            try
            {
                Type t = obj.GetType();
                int x = (sbyte)(t.GetField("x")?.GetValue(obj) ?? 0);
                int y = (sbyte)(t.GetField("y")?.GetValue(obj) ?? 0);
                
                SnakeContext? context = SnakeContext.Get(keycard.ItemSerial);
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
        
        /// <summary>
        /// A method to determine where segments are despite wrapping around the screen.
        /// </summary>
        /// <param name="input">The raw position of a segment.</param>
        /// <returns>The position of the segment on the screen.</returns>
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