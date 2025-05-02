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
        private static readonly HashSet<Player> Confirmation = [];
        
        /// <summary>
        /// Gets the Command's name.
        /// </summary>
        public string Command => "ForceSnake";
        
        /// <summary>
        /// Gets the Command's description.
        /// </summary>
        public string Description => "LIKELY VIOLATES VSR, DO NOT USE EXCESSIVELY.\n\nForces a player to play snake by soft-dc'ing them if they're playing. /ForceSnake <RA ID>";

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
            if (!int.TryParse(arguments.At(1), out int id))
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
            
            if (Confirmation.Add(user))
            {
                response = "<color=red>THIS COMMAND WILL LIKELY VIOLATE VSR, ARE YOU SURE YOU WANT TO USE IT?</color>\nrerun the command to confirm.";
                return true;
            }
            
            Confirmation.Remove(user);
            target.Connection.Send(SnakeNetworkMessage.NewGameOver());
            response = "Done!";
            return true;
        }
    }
}