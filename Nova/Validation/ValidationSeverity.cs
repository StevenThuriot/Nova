namespace Nova.Validation
{
	/// <summary>
	/// The severity of the validation message. 
	/// Mainly used to be able to differentiate between shown control colors.
	/// </summary>
	public enum ValidationSeverity
	{
		/// <summary>
		/// Lowest level validation error, counts as a suggestion.
		/// </summary>
		Suggestion,
		/// <summary>
		/// Mid level validation error, counts as a warning.
		/// </summary>
		Warning,
		/// <summary>
		/// High level validation error, counts as an error.
		/// </summary>
		Error
	}
}
