﻿#region License

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
