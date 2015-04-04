using System;
using System.Collections.Generic;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Wizard
    /// </summary>
    internal interface IWizard
    {
        /// <summary>
        ///     Creates the wizard button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">title</exception>
        IWizardButton CreateWizardButton(string title, Action<object> action, Predicate<object> canExecute = null);

        /// <summary>
        /// Navigates to the specified step.
        /// </summary>
        /// <param name="step">The step.</param>
        void DoStep(StepInfo step);

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        /// <param name="entries"></param>
        void Finish(IEnumerable<ActionContextEntry> entries);

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Gets the previous step.
        /// </summary>
        /// <value>
        /// The previous step.
        /// </value>
        LinkedListNode<StepInfo> PreviousStep { get; }
    }
}