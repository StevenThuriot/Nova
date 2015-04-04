using Nova.Controls;
using Nova.Library;
using Nova.Threading.Metadata;

namespace Nova.Shell.Library.Actions.Wizard
{
    /// <summary> 
    /// Action used for returning from a wizard on top of the current step.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Creational, Alias(Aliases.Return)]
    public class ReturnAction<TView, TViewModel> : Actionflow<TView, TViewModel>
        where TView : class, IView
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        private StackHandle<string> _handle;
        private string _result;

        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            if (!base.Execute())
                return false;
            
            var result = Return();
            
            StackInfo info;
            if (ActionContext.TryGetValue(ActionContextConstants.StackHandle, out info))
            {
                _handle = info as StackHandle<string>;

                if (_handle != null)
                {
                    _result = ActionContext.GetValue<string>(ActionContextConstants.DialogBoxResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            ReturnCompleted();

            if (_handle == null) return;
            _handle.Release(_result);
        }

        /// <summary>
        /// Called when returning from a wizard.
        /// </summary>
        /// <returns></returns>
        public virtual bool Return()
        {
            return true;
        }

        /// <summary>
        /// Called when returning from a wizard has completed.
        /// </summary>
        public virtual void ReturnCompleted()
        {
        }
    }
}