using System;
using System.Collections.Generic;
using Exiled.API.Features;
using SNAPI.Events.EventArgs;
using UnityEngine;
#if HSM
using HintServiceMeow.UI.Extension;
#elif RUEI
using RueI;
using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;
using RueI.Extensions;
#endif
namespace SNAPI.EventHandlers
{
    public class SnakeMove
    {
        public static readonly Dictionary<Player, double> Cooldowns = new();
        public static readonly Dictionary<Player, double> SavedDurations = new();
        public static void OnSnakeMove(SnakeMoveEventArgs ev)
        {
            if (ev.Player.RemoteAdminAccess && !Main.Instance.Config.SettingsAffectAdmins) return;
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
#if RUEI
        private static TimedElemRef<SetElement> Curr { get; set; }
#endif
        public static void ShowUnequipHint(SnakeMoveEventArgs ev, double cooldownRemaining, int stoppingAttempts)
        {
            if (cooldownRemaining == 0) cooldownRemaining = Main.Instance.Config.CooldownTime;
            string timeRemaining = $"{Math.Round(cooldownRemaining, 1)}";
            string message = Main.Instance.Config.ForceUnequipMessage.Replace("{0}", timeRemaining);
            if (stoppingAttempts > 0)
            {
                int remaining = Main.Instance.Config.MaxAllowedAttempts - stoppingAttempts;
                message += $"\nStop using Snake or your game will be reset. {remaining} attempt{(remaining > 1 ? "s" : "")} left";
            }
#if EXILED
                ev.Player.ShowHint(message, Main.Instance.Config.MessageDuration);
#elif HSM
            ev.Player.GetPlayerUi().CommonHint.ShowItemHint(message, Main.Instance.Config.MessageDuration);
#elif RUEI
                DisplayCore core = DisplayCore.Get(ev.Player.ReferenceHub);
                if (Curr != null)
                {
                    Element element = core.GetElement(Curr);
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