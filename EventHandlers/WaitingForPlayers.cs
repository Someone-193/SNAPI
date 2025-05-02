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
    /// An EventHandler.
    /// </summary>
    public class WaitingForPlayers
    {
        /// <summary>
        /// Called when WaitingForPlayers.
        /// </summary>
        public static void OnWaitingForPlayers()
        {
            if (!Server.Host.GameObject.GetComponent<TimeHandler>()) 
                Server.Host.GameObject.AddComponent<TimeHandler>();
        }
    }
}