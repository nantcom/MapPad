using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using InputRange = NantCom.MapPad.Types.InputRange;

namespace NantCom.MapPad.Core
{
    /// <summary>
    /// Base class for all GamePadMapper solutions
    /// </summary>
    public abstract class GamePadMapper
    {
        /// <summary>
        /// Gets or sets the source offset.
        /// </summary>
        /// <value>
        /// The source offset.
        /// </value>
        public JoystickOffset SourceOffset { get; set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <value>
        /// The current value.
        /// </value>
        public int CurrentValue { get; private set; }

        /// <summary>
        /// Gets the last value.
        /// </summary>
        /// <value>
        /// The last value.
        /// </value>
        public int LastValue { get; private set; }

        /// <summary>
        /// Gets the delta.
        /// </summary>
        /// <value>
        /// The delta.
        /// </value>
        public int Delta
        {
            get
            {
                return this.CurrentValue - this.LastValue;
            }
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>
        /// The range.
        /// </value>
        public InputRange Range { get; set; }

        #region DeadZone Calculation

        private double _DeadZoneFactor;

        /// <summary>
        /// Gets the dead zone, where dead zone starts the midpoint of the range and expand both ways towards minimum and maximum
        /// </summary>
        /// <value>
        /// The middle dead zone.
        /// </value>
        protected InputRange MiddleDeadZone { get; private set; }

        /// <summary>
        /// Gets the dead zone, where dead zone starts from minimum of the range to deadzone factor
        /// </summary>
        /// <value>
        /// The lower dead zone.
        /// </value>
        protected InputRange ValidZone { get; private set; }

        /// <summary>
        /// Gets or sets the deadzone of this Mapping
        /// </summary>
        public double DeadZoneFactor
        {
            get
            {
                return _DeadZoneFactor;
            }
            set
            {
                _DeadZoneFactor = value;

                if (value != 0 &&
                    this.Range != null)
                {
                    // dead zone specified
                    var range = this.Range.Maximum - this.Range.Minimum;
                    var absRange = Math.Abs(range);
                    var middle = (range / 2) + this.Range.Minimum;

                    this.MiddleDeadZone = new InputRange((int)(middle - (absRange * value)),
                        (int)(middle + (absRange * value)));

                    this.ValidZone = new InputRange(this.Range.Minimum + (int)(range * value), this.Range.Maximum);

                }

            }
        }

        #endregion

        protected abstract void OnJoystickUpdate();

        /// <summary>
        /// Handles the specified update.
        /// </summary>
        /// <param name="update">The update.</param>
        public virtual void Handle(JoystickUpdate update)
        {
            if (update.Offset != this.SourceOffset)
            {
                return;
            }

            this.CurrentValue = update.Value;

            this.OnJoystickUpdate();

            this.LastValue = update.Value;
        }

        /// <summary>
        /// Standard Whole Axis Range
        /// </summary>
        public static readonly InputRange AxisRange = new InputRange(0, 65535);
        /// <summary>
        /// Half of the Standard Axis Range (Higher values) 
        /// </summary>
        public static readonly InputRange UpperHalfAxisRange = new InputRange(32768, 65535);
        /// <summary>
        /// Half of the Standard Axis Range (Lower Values) 
        /// </summary>
        public static readonly InputRange LowerHalfAxisRange = new InputRange(32768, 0);
        /// <summary>
        /// Standard button Range
        /// </summary>
        public static readonly InputRange ButtonsRange = new InputRange(0, 128);
    }
        
}
