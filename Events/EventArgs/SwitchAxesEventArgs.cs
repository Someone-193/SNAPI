using Exiled.API.Features;
using Exiled.API.Features.Items;
using SnakeAPI.Features;
namespace SnakeAPI.Events.EventArgs
{
    public class SwitchAxesEventArgs
    {
        public SwitchAxesEventArgs(SnakeContext context)
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