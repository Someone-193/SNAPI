using SNAPI.Events.EventArgs;
namespace SNAPI.EventHandlers
{
    public class StartingNewSnake
    {
        public static void OnStartNewSnake(StartingNewSnakeEventArgs ev)
        {
            if (!Main.Instance.Config.AllowSnake && (Main.Instance.Config.SettingsAffectAdmins || !ev.Player.RemoteAdminAccess)) ev.Context.ForceStop();
        }
    }
}