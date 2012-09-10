#region License
// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using Nova.Base;
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
		/// Gets the action context.
		/// </summary>
		public ActionContext ActionContext { get; private set; }

		/// <summary>
		/// The validation results
		/// </summary>
		private readonly ValidationResults _Results;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseValidator&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <param name="actionContext">The ActionContext.</param>
		protected BaseValidator(ValidationResults results, ActionContext actionContext)
		{
			ActionContext = actionContext;
			_Results = results;
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
			_Results.InternalAdd(error);
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
			_Results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		protected void Add(string field)
		{
			var error = ValidationFactory.Create(field);
			_Results.InternalAdd(error);
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
			_Results.InternalAdd(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		protected void Add(string field, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, severity);
			_Results.InternalAdd(error);
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
			_Results.InternalAdd(error);
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
			_Results.InternalAdd(error);
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
			_Results.InternalAdd(error);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid
		{
			get { return _Results.IsValid; }
		}
	}
}
