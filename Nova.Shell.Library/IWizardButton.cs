using System.Windows.Input;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Buttons in a wizard
    /// </summary>
    public interface IWizardButton
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        ICommand NavigationalCommand { get; }
    }
}
