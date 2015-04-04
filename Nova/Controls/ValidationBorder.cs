using System.Windows;
using System.Windows.Controls;

namespace Nova.Controls
{
    /// <summary>
    /// A validation border to use with Nova's validation system when grouping several controls.
    /// </summary>
    public class ValidationBorder : ContentControl 
    {
        /// <summary>
        /// Initializes the <see cref="ValidationBorder" /> class.
        /// </summary>
        static ValidationBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationBorder), new FrameworkPropertyMetadata(typeof(ValidationBorder)));
        }

        /// <summary>
        /// The is filled in property
        /// </summary>
        public static readonly DependencyProperty IsFilledInProperty =
            DependencyProperty.Register("IsFilledIn", typeof (bool), typeof (ValidationBorder), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filled in.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is filled in; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilledIn
        {
            get { return (bool) GetValue(IsFilledInProperty); }
            set { SetValue(IsFilledInProperty, value); }
        }
    }
}
