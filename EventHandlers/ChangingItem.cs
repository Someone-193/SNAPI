using Exiled.Events.EventArgs.Player;
using SNAPI.Events.EventArgs;
using SNAPI.Events.Handlers;
using SNAPI.Features;
namespace SNAPI.EventHandlers
{
    public class ChangingItem
    {
        public static void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Player.CurrentItem == null) return;
            SnakeContext context = SnakeContext.Get(ev.Player.CurrentItem.Serial);
            if (context == null) return;

            context.Playing = false;
            context.Timer.Reset();

            SnakePlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
        }
    }
}