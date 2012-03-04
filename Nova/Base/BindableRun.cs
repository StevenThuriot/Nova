using System;
using System.Windows;
using System.Windows.Documents;

namespace Nova.Base
{
	/// <summary>
	/// A subclass of the Run element that exposes a DependencyProperty property
	/// to allow data binding.
	/// </summary>
	public class BindableRun : Run
	{
		/// <summary>
		/// The bound text.
		/// </summary>
		public static readonly DependencyProperty BoundTextProperty = DependencyProperty.Register("BoundText", typeof(string), typeof(BindableRun), new PropertyMetadata(OnBoundTextChanged));

		/// <summary>
		/// Called when [bound text changed].
		/// </summary>
		/// <param name="d">The Dependency Object.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Run)d).Text = (string)e.NewValue;
		}

		/// <summary>
		/// Gets or sets the bound text.
		/// </summary>
		/// <value>
		/// The bound text.
		/// </value>
		public String BoundText
		{
			get { return (string)GetValue(BoundTextProperty); }
			set { SetValue(BoundTextProperty, value); }
		}
	}
}
