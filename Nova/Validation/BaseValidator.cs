using System;
using System.Collections.Generic;
using System.Globalization;
using Nova.Library;
using Nova.Properties;

namespace Nova.Validation
{
	/// <summary>
	/// Base class for validators.
	/// </summary>
	/// <typeparam name="T">The entity you are validating.</typeparam>
	public abstract class BaseValidator<T>
    {
        /// <summary>
        /// The validation results
        /// </summary>
        private readonly ValidationResults _results;

		/// <summary>
		/// Gets the action context.
		/// </summary>
		public ActionContext ActionContext { get; private set; }
        
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseValidator&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <param name="actionContext">The ActionContext.</param>
		protected BaseValidator(ValidationResults results, ActionContext actionContext)
		{
			ActionContext = actionContext;
			_results = results;
		}

		/// <summary>
		/// Validates the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public abstract void Validate(T entity);

		/// <summary>
		/// Validates the specified entities.
		/// </summary>
		/// <param name="entities">The entities.</param>
		public void Validate(IEnumerable<T> entities)
		{
			foreach (var entity in entities)
			{
				Validate(entity);
			}
		}

		/// <summary>
		/// Adds specified field as a required field.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		protected void AddRequired(string field)
		{
			var requiredField = string.Format(CultureInfo.CurrentCulture, Resources.RequiredField, field);
			Add(field, requiredField);
		}

		/// <summary>
		/// Adds specified field as a required field with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		protected void AddRequired(string field, ValidationSeverity severity)
		{
			var requiredField = string.Format(CultureInfo.CurrentCulture, Resources.RequiredField, field);
			Add(field, requiredField, severity);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		protected void Add(string field, string message)
		{
			var error = ValidationFactory.Create(field, message);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		protected void Add(string field, string message, Guid entityID)
		{
			var error = ValidationFactory.Create(field, message, entityID);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		protected void Add(string field)
		{
			var error = ValidationFactory.Create(field);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		protected void Add(string field, Guid entityID)
		{
			var error = ValidationFactory.Create(field, entityID);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		protected void Add(string field, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, severity);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		protected void Add(string field, Guid entityID, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, entityID, severity);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="severity"> </param>
		protected void Add(string field, string message, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, message, severity);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		protected void Add(string field, string message, Guid entityID, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, message, entityID, severity);
			_results.InternalAdd(error);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid
		{
			get { return _results.IsValid; }
		}
	}
}
