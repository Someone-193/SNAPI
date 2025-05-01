using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using SNAPI.Features;
using UnityEngine;
namespace SNAPI.EventHandlers
{
    public class ItemRemoved
    {
        public static void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            Timing.CallDelayed(0, () =>
            {
                SnakeContext context = SnakeContext.Get(ev.Item.Serial);
                context?.Dispose();
            });
        }
    }
}