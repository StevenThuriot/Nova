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

using System.Threading;
using System.Windows.Input;
using Nova.Base;
using Nova.Controls;
using Nova.Shell.Library;

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

    public abstract class TestPageBase : ExtendedPage<TestPage, TestPageViewModel>
    {

    }

    public class TestPageViewModel : ContentViewModel<TestPage, TestPageViewModel> 
    {
        protected override void OnCreated()
        {
            _GoToPage2Command = CreateNavigationalAction<TestPage2, TestPage2ViewModel>();
        }

        private ICommand _GoToPage2Command;
        public ICommand GoToPage2Command
        {
            get { return _GoToPage2Command; }
            set { SetValue(ref _GoToPage2Command, value); }
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
}
