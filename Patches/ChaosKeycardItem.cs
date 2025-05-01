using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Keycards.Snake;
using Mirror;
using UnityEngine;
using static HarmonyLib.AccessTools;
namespace SnakeControl.Patches
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
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, Method(typeof(ChaosKeycardItemTranspiler), nameof(Method1))),
                ]);
            
            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;
            
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        public static void Method1(SnakeNetworkMessage msg)
        {
            Log.Warn($"Flags: {msg.Flags.ToString()}");
            Log.Warn($"Move Offset: {msg.MoveOffset.ToString()}");
            Log.Warn($"Next Food Position: {msg.NextFoodPosition?.ToString() ?? "NULL"}");
            string txt = "Segments: ";
            foreach (Vector2Int segment in msg.Segments)
            {
                txt += $"{segment.ToString()}, ";
            }
            if (txt.Length > 1) txt = txt.Remove(txt.Length - 2, 2);
            Log.Warn(txt);
        }
    }
}