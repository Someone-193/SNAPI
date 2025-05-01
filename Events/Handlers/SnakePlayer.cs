using Exiled.Events.Features;
using SnakeAPI.Events.EventArgs;
namespace SnakeAPI.Events
{
    public class SnakePlayer
    {
        public static Event<StartingNewSnakeEventArgs> StartingNewSnake { get; set; } = new();
        
        public static Event<PausingSnakeEventArgs> PausingSnake { get; set; } = new();
        
        public static Event<ResumingSnakeEventArgs> ResumingSnake { get; set; } = new();
        
        public static Event<SnakeMoveEventArgs> SnakeMove { get; set; } = new();
        
        public static Event<ScoreEventArgs> Score { get; set; } = new();
        
        public static Event<GameOverEventArgs> GameOver { get; set; } = new();
        
        public static Event<SwitchAxesEventArgs> SwitchAxes { get; set; } = new();
        
        public static void OnStartingNewSnake(StartingNewSnakeEventArgs ev) => StartingNewSnake.InvokeSafely(ev);
        
        public static void OnPausingSnake(PausingSnakeEventArgs ev) => PausingSnake.InvokeSafely(ev);
        
        public static void OnResumingSnake(ResumingSnakeEventArgs ev) => ResumingSnake.InvokeSafely(ev);
        
        public static void OnSnakeMove(SnakeMoveEventArgs ev) => SnakeMove.InvokeSafely(ev);
        
        public static void OnScore(ScoreEventArgs ev) => Score.InvokeSafely(ev);
        
        public static void OnGameOver(GameOverEventArgs ev) => GameOver.InvokeSafely(ev);
        
        public static void OnSwitchAxes(SwitchAxesEventArgs ev) => SwitchAxes.InvokeSafely(ev);
    }
}