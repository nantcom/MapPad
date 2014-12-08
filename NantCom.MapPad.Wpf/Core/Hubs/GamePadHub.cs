using Microsoft.AspNet.SignalR;
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

        private static CancellationTokenSource _Cancel;

        /// <summary>
        /// Starts the game pad.
        /// </summary>
        public static void StartGamePad()
        {
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
            if (_Cancel != null)
            {
                _Cancel.Cancel();
            }

            GamePadHub.CurrentContext.Clients.All.gamepadDisconnected();
        }

        private static void Initialize()
        {
            Gamepad.DataReceived += (s, e) =>
            {
                foreach (var item in e.JoystickUpdate)
                {
                    GamePadHub.CurrentContext.Clients.All.gamepadStatus(item);
                }
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
