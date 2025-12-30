namespace SNAPI.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using SNAPI.Events.EventArgs;
#if HSM
    using HintServiceMeow.UI.Extension;
#elif RUEI
    using RueI;
    using RueI.Displays;
    using RueI.Displays.Scheduling;
    using RueI.Elements;
    using RueI.Extensions;
#endif

    /// <summary>
    /// An EventHandler.
    /// </summary>
    public class SnakeMove
    {
        /// <summary>
        /// A Dictionary of Players and their cooldowns.
        /// </summary>
        public static readonly Dictionary<Player, double> Cooldowns = new();
        
        /// <summary>
        /// A dictionary of Players and their time playing snake to be ignored.
        /// </summary>
        public static readonly Dictionary<Player, double> SavedDurations = new();
        
#if RUEI
        private static TimedElemRef<SetElement> Curr { get; set; } = null!;
#endif

        /// <summary>
        /// Called whenever a Snake game updates.
        /// </summary>
        /// <param name="ev">The EventArgs.</param>
        public static void OnSnakeMove(SnakeMoveEventArgs ev)
        {
            if (ev.Player.RemoteAdminAccess && !Main.Instance.Config.SettingsAffectAdmins) 
                return;
            if (ev.Player.CheckPermission(Main.Instance.Config.SnakePermission))
                return;

            if (Cooldowns.TryGetValue(ev.Player, out double cooldownRemaining))
            {
                ev.Context.StoppingAttempts++;
                if (ev.Context.StoppingAttempts >= Main.Instance.Config.MaxAllowedAttempts)
                {
                    ev.Context.Reset();
                }
                else
                {
                    ev.Context.ForceStop();
                    ShowUnequipHint(ev, cooldownRemaining, ev.Context.StoppingAttempts);
                }
                
                return;
            }
            
            if (!SavedDurations.TryGetValue(ev.Player, out double ignoredPrevious))
                ignoredPrevious = 0;
            if (ev.Context.TotalTimePlaying.TotalSeconds > Main.Instance.Config.MaxPlaytime + ignoredPrevious)
            {
                Cooldowns.Add(ev.Player, Main.Instance.Config.CooldownTime);
                ev.Context.ForceStop();
                ShowUnequipHint(ev, 0, 0);
            }
        }

        /// <summary>
        /// Shows a player a hint representing how much time it will take until they can play Snake again.
        /// </summary>
        /// <param name="ev">The <see cref="SnakeMoveEventArgs"/> in OnSnakeMove.</param>
        /// <param name="cooldownRemaining">The time remaining until the Player can play Snake again.</param>
        /// <param name="stoppingAttempts">The amount of times the Player has tried to play Snake while on cooldown.</param>
        public static void ShowUnequipHint(SnakeMoveEventArgs ev, double cooldownRemaining, int stoppingAttempts)
        {
            if (cooldownRemaining == 0) 
                cooldownRemaining = Main.Instance.Config.CooldownTime;
            string timeRemaining = $"{Math.Round(cooldownRemaining, 1)}";
            string message = Main.Instance.Config.ForceUnequipMessage.Replace("{0}", timeRemaining);
            if (stoppingAttempts > 0)
            {
                int remaining = Main.Instance.Config.MaxAllowedAttempts - stoppingAttempts;
                message += $"\nStop using Snake or your game will be reset. {remaining} attempt{(remaining > 1 ? "s" : string.Empty)} left";
            }
#if EXILED
            ev.Player.ShowHint(message, Main.Instance.Config.MessageDuration);
#elif HSM
            ev.Player.GetPlayerUi().CommonHint.ShowItemHint(message, Main.Instance.Config.MessageDuration);
#elif RUEI
            DisplayCore core = DisplayCore.Get(ev.Player.ReferenceHub);
            if (Curr != null)
            {
                Element? element = core.GetElement(Curr);
                if (element != null)
                {
                    element.Enabled = false;
                    core.Update();
                }
            }

            Curr = new TimedElemRef<SetElement>();
            core.SetElemTemp(message, 500, TimeSpan.FromSeconds(Main.Instance.Config.MessageDuration), Curr);
#endif
        }
    }
}