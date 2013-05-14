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
using System.Globalization;
using Nova.Properties;
using Nova.Validation;
using Nova.Controls;

namespace Nova.Library
{
    /// <summary>
    /// A base class for actions that trigger validation.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ValidatableActionflow<TView, TViewModel> : Actionflow<TView, TViewModel> 
        where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
        private ValidationResults _ValidationResults;

        /// <summary>
        /// Initializes a new instance of the ValidatableActionflow class.
        /// </summary>
        protected ValidatableActionflow()
        {
            _ValidationResults = new ValidationResults();
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (_ValidationResults == null) return;

            _ValidationResults.InternalReset();
            _ValidationResults = null;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="results">The results.</param>
        public virtual void Validate(ValidationResults results)
        {
        }

        /// <summary>
        /// The logic that runs before the action.
        /// </summary>
        internal override void InternalOnBefore()
        {
            base.InternalOnBefore();

            try
            {
                var validationControl = View.ValidationControl;

                if (validationControl == null) return;

                var results = validationControl.ValidateRequiredFields();

                foreach (var result in results)
                {
                    var field = result.Value;
                    var entityID = result.Key;

                    var requiredField = string.Format(CultureInfo.CurrentCulture, Resources.RequiredField, field);
                    var validation = ValidationFactory.Create(field, requiredField, entityID, ValidationSeverity.Error);

                    _ValidationResults.InternalAdd(validation);
                }
            }
            catch (Exception exception)
            {
                CanComplete = false;
                ExceptionHandler.Handle(exception, Resources.ErrorMessageAsync);
            }
        }

        /// <summary>
        /// Method so inheriting classes may add extra internal logic during the InternalExecute stage.
        /// </summary>
        internal override void SafeInternalExecute()
        {
            Validate(_ValidationResults);
            CanComplete = _ValidationResults.IsValid;

            if (CanComplete)
            {
                base.SafeInternalExecute();
            }
        }

        /// <summary>
        /// Method so inheriting classes may add extra internal logic during the InternalExecuteCompleted stage.
        /// </summary>
        internal override void SafeInternalExecuteCompleted()
        {
            base.SafeInternalExecuteCompleted();

            var validationMessages = _ValidationResults.InternalGetValidations();
            ViewModel.ErrorCollection = new ReadOnlyErrorCollection(validationMessages);

            _ValidationResults.InternalReset();
        }
    }
}
