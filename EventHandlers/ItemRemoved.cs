namespace SNAPI.EventHandlers
{
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using SNAPI.Features;

    /// <summary>
    /// An EventHandler.
    /// </summary>
    public class ItemRemoved
    {
        /// <summary>
        /// Called whenever a player loses an item by any means.
        /// </summary>
        /// <param name="ev">The EventArgs.</param>
        public static void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            Timing.CallDelayed(0, () =>
            {
                SnakeContext? context = SnakeContext.Get(ev.Item.Serial);
                context?.Dispose();
            });
        }
    }
}