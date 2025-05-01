using System;
using System.Collections.Generic;
using System.Linq;
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
            try
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
                
                foreach (Player key in SnakeMove.Cooldowns.Keys.ToArray())
                {
                    double value = SnakeMove.Cooldowns[key];
                    if (Main.Instance.Config.UseCooldown) value -= Time.deltaTime;
                    SnakeMove.Cooldowns[key] = value;
                    if (value > 0) continue;
                    SnakeMove.Cooldowns.Remove(key);
                    SnakeContext context = SnakeContext.Get(key.CurrentItem?.Serial ?? 0);
                    if (context == null) continue;
                    SnakeMove.SavedDurations[key] = context.TotalTimePlaying.TotalSeconds;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}