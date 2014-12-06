using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NantCom.MapPad.Types
{
    /// <summary>
    /// Represents an Input Range of GamePad Axes and Buttons
    /// </summary>
    public class InputRange
    {
        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public int Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public int Maximum { get; set; }

        /// <summary>
        /// Gets the mid point
        /// </summary>
        /// <value>
        /// The mid.
        /// </value>
        public int Mid
        {
            get
            {
                var range = this.Maximum - this.Minimum;
                return this.Minimum + (range / 2);
            }
        }

        public InputRange(int min, int max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }

        public InputRange() { }

        /// <summary>
        /// Determines whether the specified value is in range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool IsInRange(int value)
        {
            if (this.Maximum > this.Minimum)
            {
                return value >= this.Minimum &&
                    value <= this.Maximum;
            }
            else
            {
                return value >= this.Maximum &&
                    value <= this.Minimum;
            }

        }
    }

}
