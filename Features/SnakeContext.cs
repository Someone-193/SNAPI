namespace SNAPI.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Keycards.Snake;
    using LabApi.Features.Wrappers;
    using MEC;
    using UnityEngine;
    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// A class holding all the relevant details of a Snake game.
    /// </summary>
    /// <param name="serial">The serial of the Keycard the Snake game is running on.</param>
    public class SnakeContext(ushort serial)
    {
        /// <summary>
        /// Gets the player who holds the keycard with the current SnakeContext. Note that dropped keycards reset SnakeContexts.
        /// </summary>
        public Player Player { get; } = Item.Get(serial).Owner;
        
        /// <summary>
        /// Gets the Keycard this SnakeContext is running on.
        /// </summary>
        public Keycard Keycard { get; } = (Keycard)Item.Get(serial);
        
        /// <summary>
        /// Gets a value indicating whether the Player is playing Snake on this SnakeContext when called.
        /// </summary>
        public bool Playing { get; internal set; }
        
        /// <summary>
        /// Gets a value representing how much time the Player has been playing Snake in total.
        /// </summary>
        public TimeSpan TotalTimePlaying { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating how much Score the player has iin this SnakeContext.
        /// </summary>
        public int Score { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating how long the Snake is in this SnakeContext.
        /// </summary>
        public int TotalLength => Score + 5;
        
        /// <summary>
        /// Gets a value indicating the current position of the Snake's head in this SnakeContext.
        /// </summary>
        public Vector2Int CurrentHeadPosition { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating the direction the Snake is facing in this SnakeContext.
        /// </summary>
        public Vector2Int Direction { get; internal set; } = new(9, 6);
        
        /// <summary>
        /// Gets a value indicating the location of the next piece of food in this SnakeContext.
        /// </summary>
        public Vector2Int? NextFoodPosition { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating the locations of all segments of the Snake in this SnakeContext.
        /// </summary>
        public List<Vector2Int> Segments { get; internal set; } = [];
        
        /// <summary>
        /// Gets or Sets the amount of times a player can try to continue playing snake while on cooldown before forcibly resetting their game.
        /// </summary>
        public int StoppingAttempts { get; set; }
        
        /// <summary>
        /// Gets a value indicating the SnakeEngine tied to this SnakeContext's Keycard's serial.
        /// </summary>
        public SnakeEngine Engine { get; } = ChaosKeycardItem.SnakeSessions[serial]; 
        
        /// <summary>
        /// Gets a value of all the stored item serials and their respective SnakeContexts.
        /// </summary>
        internal static Dictionary<ushort, SnakeContext> SavedContexts { get; } = new();
        
        /// <summary>
        /// Gets a value that is a Timer to track how much time has elapsed since the last SnakeNetworkMessage.
        /// </summary>
        internal Stopwatch Timer { get; } = new();
        
        /// <summary>
        /// Gets or Sets a value indicating whether this SnakeContext has detected the default SnakeNetworkMessages when acquired a chaos keycard.
        /// </summary>
        internal bool Initialized { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether this player has started playing since receiving their keycard.
        /// </summary>
        internal bool Started { get; set; }
        
        /// <summary>
        /// Gets or Sets a value indicating whether this SnakeContext is about to forcibly stop a Snake game.
        /// </summary>
        internal bool Stopping { get; set; }

        /// <summary>
        /// The designated method for acquiring SnakeContexts.
        /// </summary>
        /// <param name="serial">The serial of the keycard of the SnakeContext.</param>
        /// <returns>A SnakeContext if given a chaos keycard's serial. Null otherwise.</returns>
        public static SnakeContext? Get(ushort serial)
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
        
        /// <summary>
        /// Forcibly stops a Snake game by un-equipping and re-equipping their currently held keycard.
        /// </summary>
        public void ForceStop()
        {
            if (Player.CurrentItem != Keycard) 
                return;
            Stopping = true;
            Player.CurrentItem = null;
            Timing.CallDelayed(0F, () =>
            {
                Player.CurrentItem = Keycard;
                Stopping = false;
            });
        }
        
        /// <summary>
        /// Forcibly resets a Snake game by destroying their currently held keycard, giving a new one, then force-equipping it.
        /// </summary>
        public void Reset()
        {
            if (Player.CurrentItem != Keycard) 
                return;
            Player.RemoveHeldItem();
            Item i = Player.AddItem(ItemType.KeycardChaosInsurgency);
            Player.CurrentItem = i;
            Dispose();
        }
        
        /// <summary>
        /// Removes this SnakeContext from an internal dictionary. Typically called when a player loses their keycard (and thus resets the Snake game on it).
        /// </summary>
        public void Dispose()
        {
            SavedContexts.Remove(Keycard.Serial);
        }
    }
}