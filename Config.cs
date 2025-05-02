namespace SNAPI
{
#pragma warning disable SA1623
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    /// <summary>
    /// The main config class.
    /// </summary>
    public class Config : IConfig
    {
        /// <summary>
        /// Gets a value indicating whether the plugin is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether Debug is enabled.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets a value indicating whether this project acts as a plugin or an API.
        /// </summary>
        [Description("If enabled, causes SNAPI to not subscribe to any events. Only useful for advanced developers")]
        public bool APIMode { get; set; } = false;
        
        /// <summary>
        /// Gets a value indicating whether Snake can be played.
        /// </summary>
        [Description("If disabled, automatically unequips a players keycard when they try to play snake")]
        public bool AllowSnake { get; set; } = true;
        
        /// <summary>
        /// Gets a value indicating how long a player can play before being kicked off Snake.
        /// </summary>
        [Description("A float which determines how long a player can play snake on a keycard before forcing them to unequip")]
        public float MaxPlaytime { get; set; } = 60F;
        
        /// <summary>
        /// Gets a value indicating how long it takes until a player can play snake again.
        /// </summary>
        [Description("How long a player cannot play snake after they hit their MaxPlaytime")]
        public float CooldownTime { get; set; } = 180F;
        
        /// <summary>
        /// Gets a value indicating whether a player can play snake again after a certain time.
        /// </summary>
        public bool UseCooldown { get; set; } = true;
        
        /// <summary>
        /// Gets a value indicating the message sent to players when they are forcibly kicked off Snake.
        /// </summary>
        [Description("The text shown to a player when being forced off Snake from exceeding MaxPlaytime or being on cooldown, {0} represents cooldown remaining")]
        public string ForceUnequipMessage { get; set; } = "<b>Your free trial has expired!</b>\nWait {0} more seconds before your <b>FREE</b> tier membership refreshes!";
        
        /// <summary>
        /// Gets a value indicating how long the ForceUnequipMessage hint is shown.
        /// </summary>
        [Description("How long a player sees the ForceUnequipMessage when being forced off Snake")]
        public float MessageDuration { get; set; } = 5F;
        
        /// <summary>
        /// Gets a value indicating how many times a player can try to play snake while on cooldown before their progress is reset.
        /// </summary>
        [Description("SNAPI can't force Snake to never activate unfortunately, but it can incentivize it by resetting players progress after trying to play snake this amount of times while on cooldown")]
        public int MaxAllowedAttempts { get; set; } = 5;
        
        /// <summary>
        /// Gets a value indicating whether admins are affected by this plugin.
        /// </summary>
        [Description("If enabled, all settings pertaining to snake will apply to staff members as well")]
        public bool SettingsAffectAdmins { get; set; } = false;
        
        /// <summary>
        /// Gets a value indicating whether a command which can violate VSR gets registerd when this plugin is enabled.
        /// </summary>
        [Description("If enabled, allows all admins to use the SNAPI command which soft-disconnects clients which may violate VSR, use with caution.")]
        public bool NoVSRViolatingCommand { get; set; } = true;
    }
}