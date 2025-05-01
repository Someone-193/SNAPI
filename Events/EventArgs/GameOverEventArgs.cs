using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Interfaces;
using SnakeAPI.Features;
namespace SnakeAPI.Events.EventArgs
{
    public class GameOverEventArgs : IPlayerEvent
    {
        public GameOverEventArgs(SnakeContext context)
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