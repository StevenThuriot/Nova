﻿#region License

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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Threading;
using Nova.Controls;
using Nova.Properties;
using Nova.Validation;
using System.Windows;

namespace Nova.Base
{
    /// <summary>
    /// A base class for actions that trigger validation.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class BaseValidatableAction<TView, TViewModel> : BaseAction<TView, TViewModel> 
        where TView : class, IView
        where TViewModel : BaseViewModel<TView, TViewModel>, new()
    {
        private ValidationResults _ValidationResults;

        protected BaseValidatableAction()
        {
            _ValidationResults = new ValidationResults();
        }

        /// <summary>
        /// Validates the required fields.
        /// </summary>
        internal void ValidateRequiredFields()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
                {
                    if (ViewModel.ValidationControl == null) return;

                    var results = ViewModel.ValidationControl.ValidateRequiredFields();

                    foreach (var result in results)
                    {
                        var field = result.Value;
                        var entityID = result.Key;

                        var requiredField = string.Format(CultureInfo.CurrentCulture, Resources.RequiredField, field);
                        var validation = ValidationFactory.Create(field, requiredField, entityID,
                                                                  ValidationSeverity.Error);

                        _ValidationResults.InternalAdd(validation);
                    }
                }));
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
        /// Runs the execute action.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal override void InternalExecute()
        {
            try
            {
                ValidateRequiredFields();
                Validate(_ValidationResults);
                CanComplete = _ValidationResults.IsValid && Execute();
            }
            catch (Exception exception)
            {
                CanComplete = false;
                ExceptionHandler.Handle(exception, Resources.ErrorMessageAsync);
            }
        }

        /// <summary>
        /// Runs the execute completed action.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal override void InternalExecuteCompleted()
        {
            if (CanComplete)
            {
                try
                {
                    ExecuteCompleted();
                    ActionContext.IsSuccessful = true;
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Handle(exception, Resources.ErrorMessageMainThread);
                }
            }

            var validationMessages = _ValidationResults.InternalGetValidations();
            ViewModel.ErrorCollection = new ReadOnlyErrorCollection(validationMessages);

            _ValidationResults.InternalReset();
        }
    }
}