using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Library;
using Nova.Shell.Library.Actions.Wizard;

namespace Nova.TestModule
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage
    {
        public TestPage()
        {
            InitializeComponent();
        }
    }

    public abstract class TestPageBase : ExtendedContentControl<TestPage, TestPageViewModel>
    {

    }

    public class TestPageViewModel : ContentViewModel<TestPage, TestPageViewModel> 
    {
        protected override void OnCreated()
        {
            _goToPage2Command = CreateNavigationalAction<TestPage2, TestPage2ViewModel>(Module.Step2Id);

            var pack = new Uri("pack://application:,,,/Nova;component/Resources/Check-icon.png");
            
            var image = new BitmapImage(pack);
            image.Freeze();

            _showMessage = new RelayCommand(() => Task.Run(() =>
            {
                var result = ShowDialogBox("This is a message.", new[] {"Yes", "No"}, image);
            }));
        }


        private ICommand _showMessage;
        public ICommand ShowMessage
        {
            get { return _showMessage; }
            set { SetValue(ref _showMessage, value); }
        }

        private ICommand _goToPage2Command;
        public ICommand GoToPage2Command
        {
            get { return _goToPage2Command; }
            set { SetValue(ref _goToPage2Command, value); }
        }

        public void OnAfterEnter(ActionContext context)
        {
            Debug.Assert(context.ContainsKey("this is a string"));
        }
    }

    public class LongRunningAction : Actionflow<TestPage, TestPageViewModel>
    {
        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            //Simulate long running action.
            Thread.Sleep(5000);

            return base.Execute();
        }
    }

    public class Stack : StackAction<TestPage, TestPageViewModel>
    {
        protected override void BuildWizard(IWizardBuilder builder)
        {
            builder.AddStep<WizardView1, WizardViewModel1>()
                   .AddStep<WizardView2, WizardViewModel2>();
        }
    }
}
