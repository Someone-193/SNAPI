using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Keycards.Snake;
using UnityEngine;
namespace SNAPI.Features
{
    public class SnakeContext(ushort serial)
    {
        public Player Player { get; } = Item.Get(serial).Owner;
        public Keycard Keycard { get; } = (Keycard)Item.Get(serial);
        public bool Playing { get; internal set; } = false;
        public TimeSpan TotalTimePlaying { get; internal set; }
        public int Score => Engine.Score;
        public int Length => Engine.CurLength;
        public Vector2Int CurrentPosition { get; internal set; } = new();
        public Vector2Int Direction { get; internal set; } = new(9, 6);
        public Vector2Int? NextFoodPosition { get; internal set; } = null;
        public List<Vector2Int> Segments { get; internal set; } = [];
        internal readonly Stopwatch Timer = new();
        public SnakeEngine Engine { get; } = ChaosKeycardItem.SnakeSessions[serial]; 
        internal static Dictionary<ushort, SnakeContext> SavedContexts { get; } = new();
        public static SnakeContext Get(ushort serial)
        {
            if (serial == 0) 
                return null;
            
            if (Item.Get(serial) is not Keycard {Type: ItemType.KeycardChaosInsurgency})
            {
                Log.Error($"{nameof(Get)}: SnakeContext.Get(); was used with a non Chaos keycard serial");
                return null;
            }

            ChaosKeycardItem.GetEngineForSerial(serial);
            
            if (SavedContexts.TryGetValue(serial, out SnakeContext context))
                return context;
            
            context = new SnakeContext(serial);
            context.Timer.Reset();
            SavedContexts.Add(serial, context);
            return context;
        }
        ~SnakeContext()
        {
            SavedContexts.Remove(Keycard.Serial);
        }
    }
}