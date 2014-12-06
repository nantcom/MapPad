using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace NantCom.MapPad.Core.GamePadMappers
{
    public class DigitalButtonToMouseButtonMapper : GamePadMapper
    {
        protected static IMouseSimulator Simulator = (new InputSimulator()).Mouse;


        public MouseButtonId Target { get; set; }

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

        public DigitalButtonToMouseButtonMapper()
        {
            this.OnValue = 128;
            this.OffValue = 0;
        }

        protected override void OnJoystickUpdate()
        {
            if (this.CurrentValue == this.OnValue)
            {
                switch (this.Target)
                {
                    case MouseButtonId.Left:
                        DigitalButtonToMouseButtonMapper.Simulator.LeftButtonDown();
                        break;
                    case MouseButtonId.Right:
                        DigitalButtonToMouseButtonMapper.Simulator.RightButtonDown();
                        break;
                    case MouseButtonId.Mouse4:
                    case MouseButtonId.Mouse5:
                        DigitalButtonToMouseButtonMapper.Simulator.XButtonDown((int)this.Target);
                        break;
                }
            }

            if (this.CurrentValue == this.OffValue)
            {
                switch (this.Target)
                {
                    case MouseButtonId.Left:
                        DigitalButtonToMouseButtonMapper.Simulator.LeftButtonUp();
                        break;
                    case MouseButtonId.Right:
                        DigitalButtonToMouseButtonMapper.Simulator.RightButtonUp();
                        break;
                    case MouseButtonId.Mouse4:
                    case MouseButtonId.Mouse5:
                        DigitalButtonToMouseButtonMapper.Simulator.XButtonUp((int)this.Target);
                        break;
                }
            }
        }
    }
}
