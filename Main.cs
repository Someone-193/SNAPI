using Exiled.API.Features;
using HarmonyLib;
namespace SnakeControl
{
    public class Main : Plugin<Config>
    {
        public override string Author => "@Someone";
        public override string Name => "SnakeControl";
        public override string Prefix => "SnkCtrl";
        public static Main Instance { get; private set; }
        private Harmony Harmony;
        public override void OnEnabled()
        {
            Instance = this;
            Harmony = new Harmony("SnakeControl");
            Harmony.PatchAll();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            Harmony.UnpatchAll("SnakeControl");
            Instance = null;
        }
    }
}