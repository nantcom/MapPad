using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace NantCom.MapPad.Core.GamePadMappers
{
    public abstract class AxisToMouseMapper : AxisToAccelerationMapper
    {
        /// <summary>
        /// Gets or sets the speed of mouse movement, default is 5
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public double Speed { get; set; }

        public AxisToMouseMapper()
        {
            this.Speed = 5;
        }
        
        protected static IMouseSimulator Simulator = (new InputSimulator()).Mouse;

        protected static int XDelta;
        protected static int YDelta;

        protected static bool _IsRunning;

        protected static void StartMouseLoop()
        {
            if (_IsRunning)
            {
                return;
            }
            _IsRunning = true;

            Task.Run(() =>
            {
                while (_IsRunning)
                {
                    if (XDelta == 0 && YDelta == 0)
                    {
                        _IsRunning = false;
                        return;
                    }
                    AxisToMouseMapper.Simulator.MoveMouseBy(XDelta, YDelta);
                        
                    Thread.Sleep(10);
                    Debug.WriteLine( XDelta + "," + YDelta);
                }
            });
        }
    }

    public class AxisToMouseXMapper : AxisToMouseMapper
    {   
        protected override void OnAccelerationChanged(double acceleration)
        {
            AxisToMouseMapper.XDelta = (int)(this.Speed * -acceleration);
            AxisToMouseMapper.StartMouseLoop();
        }
    }

    public class AxisToMouseYMapper : AxisToMouseMapper
    {
        protected override void OnAccelerationChanged(double acceleration)
        {
            AxisToMouseMapper.YDelta = (int)(this.Speed * -acceleration);
            AxisToMouseMapper.StartMouseLoop();
        }
    }
}
