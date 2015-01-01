using Microsoft.AspNet.SignalR;
using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core.Hubs
{
    public class GamePadHub : Hub
    {
        public bool IsGamepadConnected()
        {
            return Gamepad.IsPolling;
        }

        public string GetGamePadDeviceName()
        {
            if (Gamepad.CurrentDevice == null)
            {
                return "(not connected)";
            }
            return Gamepad.CurrentDevice.ProductName;
        }

        public void SetProfile( MappingProfile p )
        {
            MapPad.SetProfile(p);
        }

        public void ClearMappings()
        {
            
        }

        /// <summary>
        /// Get process list from local machine
        /// </summary>
        /// <returns></returns>
        public List<SimpleProcess> GetProcesses()
        {
            return ProcessMonitor.GetProcesses().ToList();
        }

        private static CancellationTokenSource _Cancel;

        public static event Action DeviceError = delegate { };

        /// <summary>
        /// Starts the game pad.
        /// </summary>
        public static void StartGamePad()
        {
            MapPad.IsEnabled = true;

            if (Gamepad.IsPolling == false)
            {
                _Cancel = new CancellationTokenSource();
                var device = Gamepad.StartPolling(_Cancel.Token);

                if (device != null)
                {
                    GamePadHub.CurrentContext.Clients.All.gamepadConnected(device);
                }
            }
        }

        /// <summary>
        /// Stops the game pad.
        /// </summary>
        public static void StopGamePad()
        {
            MapPad.IsEnabled = false;
            if (_Cancel != null)
            {
                _Cancel.Cancel();
            }

            GamePadHub.CurrentContext.Clients.All.gamepadDisconnected();
        }

        private static void Initialize()
        {
            Gamepad.DeviceError += () =>
            {
                GamePadHub.DeviceError();
                GamePadHub.CurrentContext.Clients.All.gamepadDisconnected();
            };

            Gamepad.DataReceived += (s, e) =>
            {
                foreach (var item in e.JoystickUpdate)
                {
                    GamePadHub.CurrentContext.Clients.All.gamepadStatus(item);
                }

                GamePadHub.CurrentContext.Clients.All.gamepadConnected(Gamepad.CurrentDevice);
            };
        }

        private static IHubContext _CurrentContext;

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public static IHubContext CurrentContext
        {
            get
            {
                if (_CurrentContext == null)
                {
                    _CurrentContext = GlobalHost.ConnectionManager.GetHubContext<GamePadHub>();
                    GamePadHub.Initialize();
                }

                return _CurrentContext;
            }
        }
    }
}
