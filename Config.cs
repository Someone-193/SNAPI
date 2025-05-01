using System.ComponentModel;
using Exiled.API.Interfaces;
namespace SNAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("If disabled, automatically unequips a players keycard when they try to play snake")]
        public bool AllowSnake { get; set; } = true;
        [Description("A float which determines how long a player can play snake on a keycard before forcing them to unequip")]
        public float MaxPlaytime { get; set; } = 60F;
        [Description("How long a player cannot play snake after they hit their MaxPlaytime")]
        public float CooldownTime { get; set; } = 180F;
        [Description("The text shown to a player when being forced off Snake from exceeding MaxPlaytime or being on cooldown, {0} represents cooldown remaining")]
        public string ForceUnequipMessage { get; set; } = "<b>Your free trial has expired!</b>\nWait {0} more seconds before trying again!";
        [Description("How long a player sees the ForceUnequipMessage when being forced off Snake")]
        public float MessageDuration { get; set; } = 10F;
        [Description("If enabled, all settings pertaining to snake will apply to staff members as well")]
        public bool SettingsAffectAdmins { get; set; } = false;
        [Description("If enabled, allows all admins to use the SNAPI command which soft-disconnects clients which may violate VSR, use with caution.")]
        public bool NoVSRViolatingCommand { get; set; } = true;
    }
}