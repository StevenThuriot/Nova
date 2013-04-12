using System;
using System.ComponentModel;

namespace Nova.Controls
{
    /// <summary>
    /// Numeric Settings for the Numeric TextBox.
    /// </summary>
    public class NumericSettings
    {
        private int _MaximumNumbers;
        private int _MaximumDecimals;

        /// <summary>
        /// Gets or sets the maximum numbers.
        /// </summary>
        /// <value>
        /// The maximum numbers.
        /// </value>
        /// <exception cref="System.ArgumentException">Maximum Numbers cannot be negative or zero.</exception>
        [Bindable(false)]
        public int MaximumNumbers
        {
            get { return _MaximumNumbers; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Maximum Numbers cannot be negative or zero.");

                _MaximumNumbers = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum decimals.
        /// </summary>
        /// <value>
        /// The maximum decimals.
        /// </value>
        /// <exception cref="System.ArgumentException">Maximum Numbers cannot be negative.</exception>
        [Bindable(false)]
        public int MaximumDecimals
        {
            get { return _MaximumDecimals; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Maximum Numbers cannot be negative.");

                _MaximumDecimals = value;
            }
        }
    }
}