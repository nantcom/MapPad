using SharpDX.DirectInput;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core
{    
    /// <summary>
    /// 
    /// </summary>
    public class JoystickUpdateEventArgs : EventArgs 
    {        
        /// <summary>
        /// Gets or sets the joystick update.
        /// </summary>
        /// <value>
        /// The joystick update.
        /// </value>
        public JoystickUpdate[] JoystickUpdate { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="JoystickUpdateEventArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public JoystickUpdateEventArgs( JoystickUpdate[] data )
        {
            this.JoystickUpdate = data;
        }
    }
    
    /// <summary>
    /// Class which handle interfacing with gamepad
    /// </summary>
    public static class Gamepad
    {
        public static event EventHandler<JoystickUpdateEventArgs> DataReceived = delegate { };
        
        private static bool _IsRunning;

        /// <summary>
        /// Gets a value indicating whether Gampad is already accquired.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is polling; otherwise, <c>false</c>.
        /// </value>
        public static bool IsPolling
        {
            get
            {
                return _IsRunning;
            }
        }

        public static DeviceInstance CurrentDevice
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts polling default gamepad data.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        public static DeviceInstance StartPolling( CancellationToken cancelToken )
        {
            if (_IsRunning)
            {
                throw new InvalidOperationException("Already Polling");
            }

            //http://www.codeproject.com/Articles/839229/OoB-Moving-webcam-with-joystick-and-servos-Arduino
            var di = new DirectInput();
            var devices = from d in di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                            where d.Type == DeviceType.Gamepad
                            select d;

            var device = devices.FirstOrDefault();
            if (device == null)
            {
                return null;
            }

            var joystick = new Joystick(di, device.InstanceGuid);
            joystick.Properties.BufferSize = 128;
            joystick.Acquire();

            _IsRunning = true;

            Task.Run(() =>
            {
                using( di )
                using (joystick)
                {
                    while (cancelToken.IsCancellationRequested == false)
                    {
                        var data = joystick.GetBufferedData();
                        if (data.Length > 0)
                        {
                            Gamepad.DataReceived(null, new JoystickUpdateEventArgs(data));
                        }
                        Thread.Sleep(20);
                    }
                }

                _IsRunning = false;
                Gamepad.CurrentDevice = null;
            });

            Gamepad.CurrentDevice = device;

            return device;
        }

    }
}
