using System;
using System.Globalization;
using System.Linq;
using Nova.Controls;
using Nova.Properties;
using Nova.Validation;

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
        private ValidationResults _validationResults;

        /// <summary>
        /// Initializes a new instance of the ValidatableActionflow class.
        /// </summary>
        protected ValidatableActionflow()
        {
            _validationResults = new ValidationResults();
        }

        /// <summary>
        /// Validation Severity that counts as blocking. Messages with a severity lower than the set severity will not be considered blocking.
        /// However, if there are only non-blocking messages, they will appear to the user the first time. If this action is fired again, messages that have already been shown will be considered resolved.
        /// </summary>
        /// <value>
        /// The validation severity.
        /// </value>
        public virtual ValidationSeverity ValidationSeverity
        {
            get { return ValidationSeverity.Error; }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (_validationResults == null) return;

            _validationResults.InternalReset();
            _validationResults = null;
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

                    _validationResults.InternalAdd(validation);
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
            var errorCollection = ViewModel.ErrorCollection;
            var hasExistingErrors = errorCollection != null && errorCollection.Count > 0;
            
            Validate(_validationResults);
            CanComplete = _validationResults.IsValid;

            if (!CanComplete && hasExistingErrors)
            {
                var validationMessages = _validationResults.InternalGetValidations();

                var severity = ValidationSeverity;
                var errors = errorCollection.Where(x => x.Severity < severity).ToList();

                CanComplete = !validationMessages.Except(errors).Any();
            }

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
            
            var validationMessages = _validationResults.InternalGetValidations();
            ViewModel.ErrorCollection = new ReadOnlyErrorCollection(validationMessages);

            _validationResults.InternalReset();
        }
    }
}
