using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core
{


    public static class ProcessMonitor
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Gets current process
        /// </summary>
        /// <returns></returns>
        public static Process GetCurrentProcess()
        {
            IntPtr hWnd = ProcessMonitor.GetForegroundWindow();
            if (hWnd == null)
            {
                return null;
            }

            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            if (processId != default(uint))
            {
                return Process.GetProcessById((int)processId);
            }

            return null;
        }

        /// <summary>
        /// Get list of process
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SimpleProcess> GetProcesses()
        {
            Func<Process, byte[]> getIcon = (p) =>
            {
                try
                {
                    var ico = Icon.ExtractAssociatedIcon(p.MainModule.FileName);
                    var bmp = ico.ToBitmap();

                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);

                    var str = ms.ToArray();
                    ms.Dispose();
                    bmp.Dispose();
                    ico.Dispose();

                    return str;
                }
                catch (Exception)
                {
                    return null;
                }
            };

            return from p in Process.GetProcesses()
                   where p.ProcessName.Equals( "conhost", StringComparison.InvariantCultureIgnoreCase ) == false &&
                         p.ProcessName.Equals( "svchost", StringComparison.InvariantCultureIgnoreCase ) == false && 
                         p.ProcessName.Equals( "system", StringComparison.InvariantCultureIgnoreCase ) == false && 
                         p.ProcessName.Equals( "idle", StringComparison.InvariantCultureIgnoreCase ) == false
                    
                   let icon = getIcon(p)

                    where icon != null
                   select new SimpleProcess()
                   {
                       Name = p.ProcessName,
                       Title = p.MainWindowTitle ,
                       Icon = icon
                   };
        }

        /// <summary>
        /// Starts monitoring process
        /// </summary>
        /// <param name="token"></param>
        public static void StartMonitorProcess( CancellationToken token, Action<string> callback )
        {
            Task.Run(() =>
            {
                string name = string.Empty;
                while (token.IsCancellationRequested == false)
                {
                    var current = ProcessMonitor.GetCurrentProcess();
                    if (current != null)
                    {
                        if ( name != current.ProcessName )
                        {
                            name = current.ProcessName;
                            callback(name);
                        }

                        current.Dispose();
                    }
                    Thread.Sleep( 3000 );
                }

            });
        }
    }
}
