using System.Windows;
using Nova.Controls;

namespace Nova.Shell.Views
{
    /// <summary>
    /// Interaction logic for StepNotAvailableView.xaml
    /// </summary>
    public class StepNotAvailableView : ExtendedControl<StepNotAvailableView, StepNotAvailableViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepNotAvailableView"/> class.
        /// </summary>
        static StepNotAvailableView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepNotAvailableView), new FrameworkPropertyMetadata(typeof(StepNotAvailableView)));
        }
    }
}
