using Nova.Controls;
using Nova.Shell.Library;

namespace Nova.TestModule
{
    public partial class WizardView1
    {
        public WizardView1()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// Interaction logic for WizardView1.xaml
    /// </summary>
    public abstract class BaseWizardView1 : ExtendedContentControl<WizardView1, WizardViewModel1>
    {
    }

    public class WizardViewModel1 : WizardContentViewModel<WizardView1, WizardViewModel1>
    {
    }
}
