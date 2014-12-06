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
                
        /// <summary>
        /// Starts polling default gamepad data.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        public static DeviceInstance StartPolling( CancellationToken cancelToken )
        {
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
            });

            return device;
        }

    }
}
