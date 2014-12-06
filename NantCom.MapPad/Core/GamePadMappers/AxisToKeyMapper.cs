using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core.GamePadMappers
{

    public class AxisToKeyMapper : GamePadMapper
    {
        /// <summary>
        /// Gets or sets the target of mapping
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public ScanCode Target { get; set; }

        private bool _IsKeyPressing;

        private void PressKey()
        {
            if (_IsKeyPressing == false)
            {
                SimulateInput.KeyDown(this.Target);
                _IsKeyPressing = true;

                Debug.WriteLine("Press:" + this.Target);
            }
        }

        private void ReleaseKey()
        {
            if (_IsKeyPressing == true)
            {
                SimulateInput.KeyUp(this.Target);
                _IsKeyPressing = false;

                Debug.WriteLine("Release:" + this.Target);
            }
        }

        protected override void OnJoystickUpdate()
        {
            if (this.Range == null)
            {
                Debug.Fail("Range was not Set for: " + this.Target);
                return;
            }

            if (this.ValidZone == null)
            {
                this.DeadZoneFactor = 0.3; // use default dead zone
            }

            if (this.ValidZone.IsInRange(this.CurrentValue))
            {
                this.PressKey();
            }
            else
            {
                this.ReleaseKey();
            }
        }
    }

}
