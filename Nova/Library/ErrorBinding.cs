using System.Windows;
using System.Windows.Data;

namespace Nova.Library
{
	/// <summary>
	/// Inherits from binding to make binding to the error list easier. (Used for validation)
	/// </summary>
	public class ErrorBinding : Binding
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// Default error binding, uses "ErrorCollection" as its path.
		/// </summary>
		/// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path"/> for the binding.</param>
		private ErrorBinding(string path)
			: base(path)
		{
			Mode = BindingMode.OneWay;
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// Default error binding, uses "ErrorCollection" as its path.
		/// </summary>
		public ErrorBinding()
			: this("ErrorCollection")
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// </summary>
		/// <param name="errors">The errors.</param>
		public ErrorBinding(PropertyPath errors) : this(errors.Path)
		{
		}
	}
}
