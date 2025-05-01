using System;
using System.Collections.Generic;
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
            if (!Server.Host.GameObject.GetComponent<TimeHandler>()) Server.Host.GameObject.AddComponent<TimeHandler>();
        }
    }
    public class TimeHandler : MonoBehaviour
    {
        public void Update()
        {
            foreach (SnakeContext context in SnakeContext.SavedContexts.Values)
            {
                bool skip = context.TotalTimePlaying.TotalSeconds < 0.6F;
                bool overrideValue = context.Timer.Elapsed.TotalSeconds > 0.6F;
                if (context.Playing) context.TotalTimePlaying += TimeSpan.FromSeconds(Time.deltaTime);
                if (context.Playing && overrideValue && !skip)
                {
                    SnakePlayer.OnPausingSnake(new PausingSnakeEventArgs(context));
                    context.Playing = false;
                    context.Timer.Reset();
                }
            }
            foreach (KeyValuePair<ushort, double> kvp in SnakeMove.Cooldowns)
            {
                SnakeMove.Cooldowns[kvp.Key] = kvp.Value - Time.deltaTime;
                if (SnakeMove.Cooldowns[kvp.Key] > 0) continue;
                SnakeMove.Cooldowns.Remove(kvp.Key);
                SnakeContext context = SnakeContext.Get(kvp.Key);
                if (context == null) continue;
                SnakeMove.SavedDurations.Add(kvp.Key, context.TotalTimePlaying.TotalSeconds);
            }
        }
    }
}