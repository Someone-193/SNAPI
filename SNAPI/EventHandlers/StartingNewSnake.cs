namespace SNAPI.EventHandlers
{
    using Exiled.Permissions.Extensions;
    using SNAPI.Events.EventArgs;

    /// <summary>
    /// An EventHandler.
    /// </summary>
    public class StartingNewSnake
    {
        /// <summary>
        /// Called when a new SnakeGame starts.
        /// </summary>
        /// <param name="ev">The event args.</param>
        public static void OnStartNewSnake(StartingNewSnakeEventArgs ev)
        {
            if (Main.Instance.Config.AllowSnake || (!Main.Instance.Config.SettingsAffectAdmins && ev.Player.RemoteAdminAccess))
                return;
            if (ev.Player.CheckPermission(Main.Instance.Config.SnakePermission))
                return;

            ev.Context.ForceStop();
        }
    }
}