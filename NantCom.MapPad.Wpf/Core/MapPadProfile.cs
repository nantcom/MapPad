using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core
{
    /// <summary>
    /// Profiles
    /// </summary>
    public static class MapPadProfile
    {
        private static Dictionary<string, MappingProfile> _Profiles = new Dictionary<string, MappingProfile>();
        private static MappingProfile _DefaultProfile;

        /// <summary>
        /// Registers the profile
        /// </summary>
        /// <param name="profile"></param>
        public static void Register( MappingProfile profile )
        {
            if (string.IsNullOrEmpty( profile.ProcessName ))
            {
                _DefaultProfile = profile;
                MapPad.SetProfile(profile);
            }
            else
            {
                _Profiles.Add(profile.ProcessName, profile);
            }
        }

        /// <summary>
        /// Starts mapping game pad by process name
        /// </summary>
        /// <param name="token"></param>
        public static void StartProfileByProcessMapping( CancellationToken token )
        {
            ProcessMonitor.StartMonitorProcess(token, (s) =>
            {
                MappingProfile profile;
                if (_Profiles.TryGetValue( s, out profile ))
                {
                    MapPad.SetProfile(profile);
                }
                else
                {
                    if (_DefaultProfile != null)
                    {
                        MapPad.SetProfile(_DefaultProfile);
                    }
                }

            });
        }
    }
}
