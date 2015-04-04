using Nova.Controls;
using Nova.Shell.Library;

namespace Nova.TestModule
{
    public partial class WizardView2
    {
        public WizardView2()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// Interaction logic for WizardView2.xaml
    /// </summary>
    public abstract class BaseWizardView2 : ExtendedContentControl<WizardView2, WizardViewModel2>
    {
    }

    public class WizardViewModel2 : WizardContentViewModel<WizardView2, WizardViewModel2>
    {
    }
}
