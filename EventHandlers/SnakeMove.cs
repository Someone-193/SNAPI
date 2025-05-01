using System;
using System.Collections.Generic;
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
        public static readonly Dictionary<ushort, double> Cooldowns = new();
        public static readonly Dictionary<ushort, double> SavedDurations = new();
        public static void OnSnakeMove(SnakeMoveEventArgs ev)
        {
            if (Cooldowns.TryGetValue(ev.Keycard.Serial, out double cooldownRemaining))
            {
                ev.Context.ForceStop();
                ShowUnequipHint(ev, cooldownRemaining);
                return;
            }
            if (!SavedDurations.TryGetValue(ev.Keycard.Serial, out double ignoredPrevious))
                ignoredPrevious = 0;
            if (ev.Context.TotalTimePlaying.TotalSeconds > Main.Instance.Config.MaxPlaytime + ignoredPrevious)
            {
                Cooldowns.Add(ev.Keycard.Serial, Main.Instance.Config.CooldownTime);
                ev.Context.ForceStop();
                ShowUnequipHint(ev, 0);
            }
        }
        public static void ShowUnequipHint(SnakeMoveEventArgs ev, double cooldownRemaining)
        {
            string timeRemaining = $"{Math.Round((float)(Main.Instance.Config.MaxPlaytime - cooldownRemaining), 1)}";
#if EXILED
                ev.Player.ShowHint(Main.Instance.Config.ForceUnequipMessage.Replace("{0}", timeRemaining), Main.Instance.Config.MessageDuration);
#elif HSM
            ev.Player.GetPlayerUi().CommonHint.ShowItemHint(Main.Instance.Config.ForceUnequipMessage.Replace("{0}", timeRemaining), Main.Instance.Config.MessageDuration);
#elif RUEI
                DisplayCore core = DisplayCore.Get(ev.Player.ReferenceHub);
                core.SetElemTemp(Main.Instance.Config.ForceUnequipMessage.Replace("{0}", timeRemaining), Ruetility.FunctionalToScaledPosition(0), TimeSpan.FromSeconds(Main.Instance.Config.MessageDuration), new TimedElemRef<SetElement>());
#endif
        }
    }
}