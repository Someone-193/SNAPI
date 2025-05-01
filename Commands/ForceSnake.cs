using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using InventorySystem.Items.Keycards.Snake;
using SnakeAPI.Features;
namespace SnakeAPI.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSnake : ICommand
    {
        public string Command => "ForceSnake";
        public string Description => "LIKELY VIOLATES VSR, DO NOT USE EXCESSIVELY.\n\nForces a player to play snake by soft-dc'ing them if they're playing. /ForceSnake <RA ID>";
        public string[] Aliases => [];
        private static readonly HashSet<Player> Confirmation = [];
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