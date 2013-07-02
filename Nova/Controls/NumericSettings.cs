using System;
using System.ComponentModel;

namespace Nova.Controls
{
    /// <summary>
    /// Numeric Settings for the Numeric TextBox.
    /// </summary>
    public class NumericSettings
    {
        private int _maximumNumbers;
        private int _maximumDecimals;

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
            get { return _maximumNumbers; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Maximum Numbers cannot be negative or zero.");

                _maximumNumbers = value;
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
            get { return _maximumDecimals; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Maximum Numbers cannot be negative.");

                _maximumDecimals = value;
            }
        }
    }
}