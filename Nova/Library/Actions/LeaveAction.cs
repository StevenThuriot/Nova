using Nova.Controls;
using Nova.Threading.Metadata;

namespace Nova.Library.Actions
{
    /// <summary>
    /// Action that triggers when a certain step is left.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating, Alias(Aliases.Leave)]
    public class LeaveAction<TView, TViewModel> : Actionflow<TView, TViewModel>
        where TView : IView
        where TViewModel : IViewModel
    {
        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            return base.Execute() && Leave();
        }

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            LeaveCompleted();
            View.Dispose();
        }

        /// <summary>
        /// Called when leaving a step.
        /// </summary>
        /// <returns></returns>
        public virtual bool Leave()
        {
            return true;
        }

        /// <summary>
        /// Called when leaving the step has completed.
        /// </summary>
        public virtual void LeaveCompleted()
        {
        }
    }
}