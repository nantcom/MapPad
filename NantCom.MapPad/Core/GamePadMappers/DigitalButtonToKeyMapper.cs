using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core.GamePadMappers
{

    public class DigitalButtonToKeyMapper : GamePadMapper
    {
        /// <summary>
        /// Gets or sets the target of mapping
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public ScanCode Target { get; set; }

        /// <summary>
        /// Gets or sets the value when button is pressed
        /// </summary>
        /// <value>
        /// The target value.
        /// </value>
        public int OnValue { get; set; }

        /// <summary>
        /// Gets or sets the value when button is depressed.
        /// </summary>
        /// <value>
        /// The off value.
        /// </value>
        public int OffValue { get; set; }

        public DigitalButtonToKeyMapper()
        {
            this.OnValue = 128;
            this.OffValue = 0;
        }

        protected override void OnJoystickUpdate()
        {
            if (this.CurrentValue == this.OnValue)
            {
                SimulateInput.KeyDown(this.Target);
            }

            if (this.CurrentValue == this.OffValue)
            {
                SimulateInput.KeyUp(this.Target);
            }
        }
    }

}
