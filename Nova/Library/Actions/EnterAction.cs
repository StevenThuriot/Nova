using Nova.Controls;
using Nova.Threading.Metadata;

namespace Nova.Library.Actions
{
    /// <summary>
    /// Action that triggers when a certain step is entered.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Creational, Alias(Aliases.Enter)]
    public class EnterAction<TView, TViewModel> : Actionflow<TView, TViewModel>
		where TView : IView
		where TViewModel : IViewModel
    {
        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            if (!base.Execute())
                return false;
            
            ViewModel.PrepareChangeTracking();
            return Enter();
        }

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            EnterCompleted();
            ViewModel.InitializeChangeTracking();
        }

        /// <summary>
        /// Called when entering a step.
        /// </summary>
        /// <returns></returns>
        public virtual bool Enter()
        {
            return true;
        }

        /// <summary>
        /// Called when entering the step has completed.
        /// </summary>
        public virtual void EnterCompleted()
        {
        }
	}
}
