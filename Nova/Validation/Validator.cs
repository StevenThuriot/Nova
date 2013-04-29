using Nova.Library;

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
using Nova.Controls;
using Nova.Properties;

namespace Nova.Validation
{
	/// <summary>
	/// A helper class to validate the model.
	/// </summary>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public class Validator<TView, TViewModel> : IValidator 
		where TViewModel : ViewModel<TView, TViewModel>, new()
		where TView : class, IView
	{
		/// <summary>
		/// The list of errors.
		/// </summary>
		private List<BaseValidation> _Errors;

		/// <summary>
		/// Gets the view model.
		/// </summary>
		private readonly TViewModel _ViewModel;

		/// <summary>
		/// Sets the error collection.
		/// </summary>
		internal void InternalSetErrorCollection()
		{
			_ViewModel.ErrorCollection = new ReadOnlyErrorCollection(_Errors);
			_Errors = new List<BaseValidation>();
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Validator&lt;TView, TViewModel&gt;"/> class.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		internal Validator(TViewModel viewModel)
		{
			_ViewModel = viewModel;
			_Errors = new List<BaseValidation>();
		}

		/// <summary>
		/// Adds specified field as a required field.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		public void AddRequired(string field)
		{
			var requiredField = string.Format(CultureInfo.CurrentCulture, Resources.RequiredField, field);
			Add(field, requiredField);
		}

		/// <summary>
		/// Adds specified field as a required field with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		public void AddRequired(string field, ValidationSeverity severity)
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
		public void Add(string field, string message)
		{
			var error = ValidationFactory.Create(field, message);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		public void Add(string field, string message, Guid entityID)
		{
			var error = ValidationFactory.Create(field, message, entityID);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		public void Add(string field)
		{
			var error = ValidationFactory.Create(field);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation.
		/// The severity is set at "Error" level.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		public void Add(string field, Guid entityID)
		{
			var error = ValidationFactory.Create(field, entityID);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="severity"> </param>
		public void Add(string field, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, severity);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		public void Add(string field, Guid entityID, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, entityID, severity);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="severity"> </param>
		public void Add(string field, string message, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, message, severity);
			_Errors.Add(error);
		}

		/// <summary>
		/// Adds the specified validation with a custom severity.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="severity">The severity.</param>
		public void Add(string field, string message, Guid entityID, ValidationSeverity severity)
		{
			var error = ValidationFactory.Create(field, message, entityID, severity);
			_Errors.Add(error);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid
		{
			get { return _Errors.Count == 0; }
		}
	}
}
