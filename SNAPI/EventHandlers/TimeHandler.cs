namespace SNAPI.EventHandlers
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using SNAPI.Events.EventArgs;
    using SNAPI.Events.Handlers;
    using SNAPI.Features;
    using UnityEngine;

    /// <summary>
    /// A MonoBehaviour to track time.
    /// </summary>
    public class TimeHandler : MonoBehaviour
    {
        /// <summary>
        /// Called every tick.
        /// </summary>
        public void Update()
        {
            try
            {
                foreach (SnakeContext context in SnakeContext.SavedContexts.Values.ToArray())
                {
                    bool skip = context.TotalTimePlaying.TotalSeconds < 0.6F;
                    bool overrideValue = context.Timer.Elapsed.TotalSeconds > 0.6F;
                    if (context.Playing) 
                        context.TotalTimePlaying += TimeSpan.FromSeconds(Time.deltaTime);
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
                    if (Main.Instance.Config.UseCooldown) 
                        value -= Time.deltaTime;
                    SnakeMove.Cooldowns[key] = value;
                    if (value > 0) 
                        continue;
                    SnakeMove.Cooldowns.Remove(key);
                    SnakeContext? context = SnakeContext.Get(key.CurrentItem?.Serial ?? 0);
                    if (context == null) 
                        continue;
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