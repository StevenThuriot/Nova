using System;
using System.Windows;
using System.Windows.Media;

namespace Nova.Validation
{
	/// <summary>
	/// A validation with Warning severity.
	/// </summary>
	public class ValidationWarning : BaseValidation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationWarning"/> class.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		public ValidationWarning(string field, string message, Guid entityID) 
			: base(field, message, entityID)
		{
		}

		/// <summary>
		/// Gets the ranking.
		/// The higher, the more severe a validation message is.
		/// </summary>
		public override int Ranking
		{
			get { return 50; }
		}

		/// <summary>
		/// Gets the severity brush.
		/// </summary>
		/// <returns>A brush from the resource file.</returns>
		public override Brush SeverityBrush
		{
			get { return Application.Current.Resources["Warning"] as Brush; }
		}
	}
}
