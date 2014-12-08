using Loopback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace NantCom.MapPad.Core
{
    public static class MetroAppSetup
    {
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Enable Loopback access for NantCom MapPad Package
        /// </summary>
        public static void Setup()
        {
            if (IsAdministrator() == false)
            {
                // Restart program and run as admin
                var exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                startInfo.Arguments = "setupapp";

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception)
                {
                }

                return;
            }

            var loopUtil = new LoopUtil();
            loopUtil.LoadApps();

            var targetApp = (from app in loopUtil.Apps
                              where app.appContainerName.Equals("NantCom.NCMapPad_5wwphy6k5b6bt", StringComparison.InvariantCultureIgnoreCase)
                              select app).FirstOrDefault();

            if (targetApp != null)
            {
                targetApp.LoopUtil = true;
                loopUtil.SaveLoopbackState();
            } 
        }
    }
}
