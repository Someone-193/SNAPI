using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Interfaces;
using SNAPI.Features;
using UnityEngine;
namespace SNAPI.Events.EventArgs
{
    public class ResumingSnakeEventArgs : IPlayerEvent
    {
        public ResumingSnakeEventArgs(SnakeContext context)
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