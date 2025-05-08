namespace SNAPI.Events.EventArgs
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using SNAPI.Features;

    /// <summary>
    /// The arguments for when a player switches axes.
    /// </summary>
    /// <param name="context">The SnakeContext this event happened in.</param>
    public class SwitchAxesEventArgs(SnakeContext context)
    {
        /// <summary>
        /// Gets the Player that is playing Snake.
        /// </summary>
        public Player Player { get; } = context.Player;
        
        /// <summary>
        /// Gets the keycard that this SnakeContext is on.
        /// </summary>
        public Keycard Keycard { get; } = context.Keycard;
        
        /// <summary>
        /// Gets the SnakeContext this happened in.
        /// </summary>
        public SnakeContext Context { get; } = context;
    }
}