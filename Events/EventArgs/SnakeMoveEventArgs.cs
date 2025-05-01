using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Interfaces;
using SnakeAPI.Features;
using UnityEngine;
namespace SnakeAPI.Events.EventArgs
{
    public class SnakeMoveEventArgs : IPlayerEvent
    {
        public SnakeMoveEventArgs(SnakeContext context)
        {
            Player = context.Player;
            Keycard = context.Keycard;
            Context = context;
        }
        public Player Player { get; }
        public Keycard Keycard { get; }
        public SnakeContext Context { get; }
    }
}