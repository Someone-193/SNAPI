using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Keycards.Snake;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Item = Exiled.API.Features.Items.Item;
using Player = Exiled.API.Features.Player;
namespace SNAPI.Features
{
    public class SnakeContext(ushort serial)
    {
        public Player Player { get; } = Item.Get(serial).Owner;
        public Keycard Keycard { get; } = (Keycard)Item.Get(serial);
        public bool Playing { get; internal set; } = false;
        internal bool Initialized { get; set; } = false;
        internal bool Started { get; set; } = false;
        internal bool Stopping { get; set; } = false;
        public TimeSpan TotalTimePlaying { get; internal set; }
        public int Score { get; internal set; }
        public int TotalLength => Score + 5;
        public Vector2Int CurrentHeadPosition { get; internal set; } = new();
        public Vector2Int Direction { get; internal set; } = new(9, 6);
        public Vector2Int? NextFoodPosition { get; internal set; } = null;
        public List<Vector2Int> Segments { get; internal set; } = [];
        public int StoppingAttempts { get; set; } = 0;
        internal readonly Stopwatch Timer = new();
        public SnakeEngine Engine { get; } = ChaosKeycardItem.SnakeSessions[serial]; 
        internal static Dictionary<ushort, SnakeContext> SavedContexts { get; } = new();
        public static SnakeContext Get(ushort serial)
        {
            if (serial == 0) 
                return null;

            if (Item.Get(serial)?.Type != ItemType.KeycardChaosInsurgency && Pickup.Get(serial)?.Type != ItemType.KeycardChaosInsurgency)
                return null;

            ChaosKeycardItem.GetEngineForSerial(serial);
            
            if (SavedContexts.TryGetValue(serial, out SnakeContext context))
                return context;
            
            context = new SnakeContext(serial);
            context.Timer.Reset();
            SavedContexts.Add(serial, context);
            return context;
        }
        public void ForceStop()
        {
            if (Player.CurrentItem != Keycard) return;
            Stopping = true;
            Player.CurrentItem = null;
            Timing.CallDelayed(0F, () =>
            {
                Player.CurrentItem = Keycard;
                Stopping = false;
            });
        }
        public void Reset()
        {
            if (Player.CurrentItem != Keycard) return;
            Player.RemoveHeldItem();
            Item i = Player.AddItem(ItemType.KeycardChaosInsurgency);
            Player.CurrentItem = i;
            Dispose();
        }
        public void Dispose()
        {
            SavedContexts.Remove(Keycard.Serial);
        }
    }
}