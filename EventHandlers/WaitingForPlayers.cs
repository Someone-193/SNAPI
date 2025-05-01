using SnakeAPI.Events;
using SnakeAPI.Events.EventArgs;
using SnakeAPI.Features;
using UnityEngine;
namespace SnakeAPI.EventHandlers
{
    public class WaitingForPlayers
    {
        public static void OnWaitingForPlayers()
        {
            
        }
    }
    public class ActiveSnakeHandler : MonoBehaviour
    {
        public void Update()
        {
            foreach (SnakeContext context in SnakeContext.SavedContexts.Values)
            {
                bool wasPlaying = context.Playing;
                context.Playing = context.Timer.Elapsed.TotalSeconds < 1.5F;
                if (wasPlaying != context.Playing)
                {
                    if (context.Playing)
                        SnakePlayer.OnResumingSnake(new ResumingSnakeEventArgs(context));
                    else
                    {
                        SnakePlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                        context.Timer.Reset();
                    }
                }
            }
        }
    }
}