namespace SNAPI.Commands
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using InventorySystem.Items.Keycards.Snake;
    using SNAPI.Features;

    /// <summary>
    /// A Command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSnake : ICommand
    {
        /// <summary>
        /// Gets the Command's name.
        /// </summary>
        public string Command => "ForceSnake";
        
        /// <summary>
        /// Gets the Command's description.
        /// </summary>
        public string Description => "Forces a player to play snake by soft-dc'ing them if they're playing. Usage: ForceSnake <RA ID>";

        /// <summary>
        /// Gets the Command's aliases.
        /// </summary>
        public string[] Aliases => [];
        
        /// <summary>
        /// Called when the command is called.
        /// </summary>
        /// <param name="arguments">The arguments given.</param>
        /// <param name="sender">The ICommandSender that fired the command.</param>
        /// <param name="response">The response from the command.</param>
        /// <returns>A bool indicating whether the command fired successfully or not.</returns>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player user = Player.Get(sender);
            if (arguments.Count != 1)
            {
                response = "Usage: ForceSnake <RA ID>";
                return false;
            }
            
            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = "Please specify a valid ID.";
                return false;
            }
            
            Player target = Player.Get(id);
            if (!SnakeContext.Get(target.CurrentItem?.Serial ?? 0)?.Playing ?? true)
            {
                response = "The target is not playing Snake right now, try again later";
                return true;
            }
            
            target.Connection.Send(SnakeNetworkMessage.NewGameOver());
            response = "Done!";
            return true;
        }
    }
}