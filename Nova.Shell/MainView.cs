using System.Windows.Input;

namespace Nova.Shell
{
    /// <summary>
    /// The Main view window.
    /// </summary>
    public partial class MainView
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainView" /> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            DropShadow.ForWindow(this);
        }

        /// <summary>
        ///     Moves the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="MouseButtonEventArgs" /> instance containing the event data.
        /// </param>
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            DragMove();

            e.Handled = true;
        }

        /// <summary>
        /// Updates the cursor.
        /// </summary>
        /// <param name="isloading">if set to <c>true</c> [isloading].</param>
        protected override void UpdateCursor(bool isloading)
        {
            //We want to update the cursor on Session level only. Leave the window alone!
        }
    }
}