using System.Collections.Generic;
using Nova.Controls;
using Nova.Library;
using Nova.Threading.Metadata;

namespace Nova.Shell.Library.Actions.Wizard
{
    /// <summary>
    /// Validatable Finish Wizard Action
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating]
    public class ValidatableFinishAction<TView, TViewModel> : ValidatableActionflow<TView, TViewModel>
        where TView : class, IView
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        private IEnumerable<ActionContextEntry> _entries;

        public sealed override bool Execute()
        {
            var execute = base.Execute() && Finish();

            if (!execute)
                return false;

            _entries = ActionContext.GetEntries();

            return true;
        }

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        /// <returns></returns>
        protected virtual bool Finish()
        {
            return true;
        }

        public sealed override void ExecuteCompleted()
        {
            base.ExecuteCompleted();
            ViewModel.Finish(_entries);
        }
    }
}