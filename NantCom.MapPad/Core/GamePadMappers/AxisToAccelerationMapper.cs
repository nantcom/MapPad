using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core.GamePadMappers
{
    /// <summary>
    /// Base class which handles Axis hold to Acceleration
    /// (for example: when holding the Thumb Stick 25% to the left will generate acceleration factor of 0.75)
    /// </summary>
    public abstract class AxisToAccelerationMapper : GamePadMapper
    {
        private bool _IsRunning;
        private void UpdateLoop()
        {
            Task.Run(() =>
            {
                var doubleMid = (double)this.Range.Mid;
                while (_IsRunning)
                {
                    if (this.MiddleDeadZone.IsInRange(this.CurrentValue))
                    {
                        // stick was now in dead zone
                        _IsRunning = false;
                        return;
                    }

                    // calculate acceleration
                    var acceleration = 1d - (this.CurrentValue / doubleMid);
                    this.OnAccelerationChanged(acceleration);

                    Thread.Sleep(10);
                }
            });
        }

        protected override void OnJoystickUpdate()
        {
            if (this.Range == null)
            {
                this.Range = GamePadMapper.AxisRange;
            }

            if (this.MiddleDeadZone == null)
            {
                this.DeadZoneFactor = 0.001;
            }

            double acceleration = 0;
            if (this.MiddleDeadZone.IsInRange(this.CurrentValue) == false)
            {
                var doubleMid = (double)this.Range.Mid;
                acceleration = 1d - (this.CurrentValue / doubleMid);
            }
            this.OnAccelerationChanged(acceleration);
        }

        protected abstract void OnAccelerationChanged(double acceleration);
    }
}
