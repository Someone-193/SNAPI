using System;
using Exiled.API.Features;
using SNAPI.Events.EventArgs;
using SNAPI.Events.Handlers;
using SNAPI.Features;
using UnityEngine;
namespace SNAPI.EventHandlers
{
    public class WaitingForPlayers
    {
        public static void OnWaitingForPlayers()
        {
            if (!Server.Host.GameObject.GetComponent<ActiveSnakeHandler>()) Server.Host.GameObject.AddComponent<ActiveSnakeHandler>();
        }
    }
    public class ActiveSnakeHandler : MonoBehaviour
    {
        public void Update()
        {
            foreach (SnakeContext context in SnakeContext.SavedContexts.Values)
            {
                bool wasPlaying = context.Playing;
                bool skip = context.TotalTimePlaying.TotalSeconds < 0.6F;
                context.Playing = context.Timer.Elapsed.TotalSeconds < 0.6F || skip;
                // Need to figure out when detecting pause or known playing.
                if (context.Playing) context.TotalTimePlaying += TimeSpan.FromSeconds(Time.deltaTime);
                if (wasPlaying && !context.Playing && !skip)
                {
                    SnakePlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                    context.Timer.Reset();
                }
            }
        }
    }
}