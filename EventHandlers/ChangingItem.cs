namespace SNAPI.EventHandlers
{
    using Exiled.Events.EventArgs.Player;
    using SNAPI.Events.EventArgs;
    using SNAPI.Events.Handlers;
    using SNAPI.Features;

    /// <summary>
    /// An EventHandler.
    /// </summary>
    public class ChangingItem
    {
        /// <summary>
        /// Called whenever a player changes their held item.
        /// </summary>
        /// <param name="ev">The EventArgs.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Player.CurrentItem == null) 
                return;
            SnakeContext? context = SnakeContext.Get(ev.Player.CurrentItem.Serial);
            if (context is not { Playing: true }) 
                return;

            context.Playing = false;
            context.Timer.Reset();

            SnakePlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
        }
    }
}