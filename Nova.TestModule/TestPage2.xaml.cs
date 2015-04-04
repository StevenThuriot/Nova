using System.Windows.Input;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Library;

namespace Nova.TestModule
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage2
    {
        public TestPage2()
        {
            InitializeComponent();
        }
    }

    public abstract class TestPage2Base : ExtendedContentControl<TestPage2, TestPage2ViewModel>
    {

    }

    public class TestPage2ViewModel : ContentViewModel<TestPage2, TestPage2ViewModel>
    {
        protected override void OnCreated()
        {
            _goToPage1Command = CreateNavigationalAction<TestPage, TestPageViewModel>(Module.Step1Id, ActionContextEntry.Create("this is a string", "test", false));
        }

        private ICommand _goToPage1Command;
        public ICommand GoToPage1Command
        {
            get { return _goToPage1Command; }
            set { SetValue(ref _goToPage1Command, value); }
        }
    }
}
