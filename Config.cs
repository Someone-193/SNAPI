using System.ComponentModel;
using Exiled.API.Interfaces;
namespace SNAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("If enabled, causes SNAPI to not subscribe to any events. Only useful for advanced developers")]
        public bool APIMode { get; set; } = false;
        [Description("If disabled, automatically unequips a players keycard when they try to play snake")]
        public bool AllowSnake { get; set; } = true;
        [Description("A float which determines how long a player can play snake on a keycard before forcing them to unequip")]
        public float MaxPlaytime { get; set; } = 60F;
        [Description("How long a player cannot play snake after they hit their MaxPlaytime")]
        public float CooldownTime { get; set; } = 180F;
        public bool UseCooldown { get; set; } = true;
        [Description("The text shown to a player when being forced off Snake from exceeding MaxPlaytime or being on cooldown, {0} represents cooldown remaining")]
        public string ForceUnequipMessage { get; set; } = "<b>Your free trial has expired!</b>\nWait {0} more seconds before your <b>FREE</b> tier membership refreshes!";
        [Description("How long a player sees the ForceUnequipMessage when being forced off Snake")]
        public float MessageDuration { get; set; } = 5F;
        [Description("SNAPI can't force Snake to never activate unfortunately, but it can incentivize it by resetting players progress after trying to play snake this amount of times while on cooldown")]
        public int MaxAllowedAttempts { get; set; } = 5;
        [Description("If enabled, all settings pertaining to snake will apply to staff members as well")]
        public bool SettingsAffectAdmins { get; set; } = false;
        [Description("If enabled, allows all admins to use the SNAPI command which soft-disconnects clients which may violate VSR, use with caution.")]
        public bool NoVSRViolatingCommand { get; set; } = true;
    }
}