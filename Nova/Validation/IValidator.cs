using System;

namespace Nova.Validation
{
	/// <summary>
	/// The validation interface.
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Adds specified field as a required field.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		void AddRequired(string field);

		/// <summary>
		/// Adds specified field as a required field with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		void AddRequired(string field, ValidationSeverity severity);

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		void Add(string field, string message);

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="message">The message.</param>
		void Add(string field, string message, Guid entityID);

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		void Add(string field);

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		void Add(string field, Guid entityID);

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		void Add(string field, ValidationSeverity severity);

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		void Add(string field, Guid entityID, ValidationSeverity severity);

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="severity"> </param>
		void Add(string field, string message, ValidationSeverity severity);

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		void Add(string field, string message, Guid entityID, ValidationSeverity severity);

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		bool IsValid { get; }
	}
}