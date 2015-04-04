using System;
using System.Windows.Input;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Buttons in a wizard
    /// </summary>
    internal class WizardButton : IWizardButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardButton"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="navigationalCommand">The navigational command.</param>
        /// <exception cref="System.ArgumentNullException">
        /// title
        /// or
        /// navigationalCommand
        /// </exception>
        public WizardButton(string title, ICommand navigationalCommand)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            Title = title;
            NavigationalCommand = navigationalCommand;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        public ICommand NavigationalCommand { get; private set; }
    }
}