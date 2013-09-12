#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Nova.Library;
using System.Threading;
using System.Windows.Input;
using Nova.Controls;
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

            _showMessage = new RelayCommand(() => ShowDialogBox("This is a message.", image));
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
