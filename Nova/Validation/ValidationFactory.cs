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

namespace Nova.Validation
{
	/// <summary>
	/// A factory to create validation messages.
	/// </summary>
	internal class ValidationFactory
	{
		/// <summary>
		/// Creates a new validation using the specified field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>A new validation</returns>
		public static BaseValidation Create(string field)
		{
			return Create(field, ValidationSeverity.Error);
		}

		/// <summary>
		/// Creates a new validation using the specified field and message.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <returns>A new validation</returns>
		public static BaseValidation Create(string field, string message)
		{
			return Create(field, message, ValidationSeverity.Error);
		}

		/// <summary>
		/// Creates a new validation using the specified field and severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity">The severity.</param>
		/// <returns>A new validation</returns>
		public static BaseValidation Create(string field, ValidationSeverity severity)
		{
			return Create(field, null, severity);
		}

		/// <summary>
		/// Creates a new validation using the specified field, message and severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="severity">The severity.</param>
		/// <returns>A new validation</returns>
		public static BaseValidation Create(string field, string message, ValidationSeverity severity)
		{
			return Create(field, message, Guid.Empty, severity);
		}

		/// <summary>
		/// Creates a new validation using the specified field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns>
		/// A new validation
		/// </returns>
		public static BaseValidation Create(string field, Guid entityID)
		{
			return Create(field, entityID, ValidationSeverity.Error);
		}

		/// <summary>
		/// Creates a new validation using the specified field and message.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns>
		/// A new validation
		/// </returns>
		public static BaseValidation Create(string field, string message, Guid entityID)
		{
			return Create(field, message, entityID, ValidationSeverity.Error);
		}

		/// <summary>
		/// Creates a new validation using the specified field and severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		/// <returns>
		/// A new validation
		/// </returns>
		public static BaseValidation Create(string field, Guid entityID, ValidationSeverity severity)
		{
			return Create(field, null, entityID, severity);
		}

		/// <summary>
		/// Creates a new validation using the specified field, message and severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		/// <returns>
		/// A new validation
		/// </returns>
		public static BaseValidation Create(string field, string message, Guid entityID, ValidationSeverity severity)
		{
			switch (severity)
			{
				case ValidationSeverity.Suggestion:
					return new ValidationSuggestion(field, message, entityID);
				case ValidationSeverity.Warning:
					return new ValidationWarning(field, message, entityID);
				case ValidationSeverity.Error:
					return new ValidationError(field, message, entityID);
				default:
					throw new ArgumentOutOfRangeException("severity");
			}
		}
	}
}
