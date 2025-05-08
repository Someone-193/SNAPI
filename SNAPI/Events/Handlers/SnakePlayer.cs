namespace SNAPI.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors
    
    using Exiled.Events.Features;
    using SNAPI.Events.EventArgs;

    public class SnakePlayer
    {
        /// <summary>
        /// Invoked after a player begins a new game of Snake.
        /// </summary>
        public static Event<StartingNewSnakeEventArgs> StartingNewSnake { get; set; } = new();
        
        /// <summary>
        /// Invoked after a player stops playing Snake mid-game.
        /// </summary>
        public static Event<PausingSnakeEventArgs> PausingSnake { get; set; } = new();
        
        /// <summary>
        /// Invoked after a player continues a stopped game of snake.
        /// </summary>
        public static Event<ResumingSnakeEventArgs> ResumingSnake { get; set; } = new();
        
        /// <summary>
        /// Invoked after every time a Snake instance updates.
        /// </summary>
        public static Event<SnakeMoveEventArgs> SnakeMove { get; set; } = new();
        
        /// <summary>
        /// Invoked after a player scores.
        /// </summary>
        public static Event<ScoreEventArgs> Score { get; set; } = new();
        
        /// <summary>
        /// Invoked after a player loses.
        /// </summary>
        public static Event<GameOverEventArgs> GameOver { get; set; } = new();
        
        /// <summary>
        /// Invoked after a player attempts to change axes.
        /// </summary>
        public static Event<SwitchAxesEventArgs> SwitchAxes { get; set; } = new();
        
        /// <summary>
        /// Called after a player begins a new game of Snake.
        /// </summary>
        /// <param name="ev">The <see cref="StartingNewSnakeEventArgs"/> instance.</param>
        public static void OnStartingNewSnake(StartingNewSnakeEventArgs ev) => StartingNewSnake.InvokeSafely(ev);
        
        /// <summary>
        /// Called after a player stops playing Snake mid-game.
        /// </summary>
        /// <param name="ev">The <see cref="PausingSnakeEventArgs"/> instance.</param>
        public static void OnPausingSnake(PausingSnakeEventArgs ev) => PausingSnake.InvokeSafely(ev);
        
        /// <summary>
        /// Called after a player continues a stopped game of snake.
        /// </summary>
        /// <param name="ev">The <see cref="ResumingSnakeEventArgs"/> instance.</param>
        public static void OnResumingSnake(ResumingSnakeEventArgs ev) => ResumingSnake.InvokeSafely(ev);
        
        /// <summary>
        /// Called after every time a Snake instance updates.
        /// </summary>
        /// <param name="ev">The <see cref="SnakeMoveEventArgs"/> instance.</param>
        public static void OnSnakeMove(SnakeMoveEventArgs ev) => SnakeMove.InvokeSafely(ev);
        
        /// <summary>
        /// Called after a player scores.
        /// </summary>
        /// <param name="ev">The <see cref="ScoreEventArgs"/> instance.</param>
        public static void OnScore(ScoreEventArgs ev) => Score.InvokeSafely(ev);
        
        /// <summary>
        /// Called after a player loses.
        /// </summary>
        /// <param name="ev">The <see cref="GameOverEventArgs"/> instance.</param>
        public static void OnGameOver(GameOverEventArgs ev) => GameOver.InvokeSafely(ev);
        
        /// <summary>
        /// Called after a player attempts to change axes.
        /// </summary>
        /// <param name="ev">The <see cref="SwitchAxesEventArgs"/> instance.</param>
        public static void OnSwitchAxes(SwitchAxesEventArgs ev) => SwitchAxes.InvokeSafely(ev);
    }
}