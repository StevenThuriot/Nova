using System.Collections.Generic;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Interface for multistep content view model
    /// </summary>
    internal interface IMultistepContentViewModel : IContentViewModel
    {
        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        LinkedListNode<StepInfo> CurrentStep { get; }
    }
}