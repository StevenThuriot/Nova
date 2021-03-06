﻿using Nova.Controls;
using Nova.Library;
using Nova.Threading.Metadata;

namespace Nova.Shell.Library.Actions.Multistep
{
    /// <summary>
    /// Action that triggers when a certain step is left that is part of a multistep.
    /// </summary>
    /// <remarks>Similar to a normal leave action but doesn't dispose the view yet. 
    /// This will be handled by the multistep itself.</remarks>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating, Alias(Nova.Library.Actions.Aliases.Leave)]
    public class LeaveMultistepAction<TView, TViewModel> : Actionflow<TView, TViewModel>
        where TView : class, IView
        where TViewModel : MultistepContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            if (!base.Execute())
                return false;

            bool triggerValidation;
            bool result;

            if (ActionContext.TryGetValue(ActionContextConstants.TriggerValidation, out triggerValidation) &&
                triggerValidation)
            {
                result = ActionValidationHelper.TriggerValidation(ViewModel);
            }
            else
            {
                result = true;
            }

            return result && Leave();
        }

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            LeaveCompleted();
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