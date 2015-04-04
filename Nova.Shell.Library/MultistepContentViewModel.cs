using System.Collections.Generic;
using System.Threading.Tasks;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Library.Actions.Multistep;

namespace Nova.Shell.Library
{
    /// <summary>
    ///     A viewmodel for pages that belong to a multistep.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class MultistepContentViewModel<TView, TViewModel> : ContentViewModel<TView, TViewModel>, IMultistepContentViewModel 
        where TView : class, IView
        where TViewModel : MultistepContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public LinkedListNode<StepInfo> CurrentStep { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public dynamic Model { get; private set; }

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <param name="triggerDeferal">if set to <c>true</c> [trigger deferal].</param>
        internal override void Initialize(IDictionary<string, object> initializer, bool triggerDeferal = true)
        {
            base.Initialize(initializer, false);

            CurrentStep = (LinkedListNode<StepInfo>)initializer["Node"];
            Model = initializer["Model"];

            if (triggerDeferal) TriggerDeferal();
        }

        public override Task<bool> Leave(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<LeaveMultistepAction<TView, TViewModel>>(parameters);
        }
    }
}