using System.ComponentModel;
using Exiled.API.Interfaces;
namespace SNAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
        [Description("If enabled, allows all admins to use the SNAPI command which soft-disconnects clients which may violate VSR, use with caution.")]
        public bool NoVSRViolatingCommand { get; set; } = true;
    }
}