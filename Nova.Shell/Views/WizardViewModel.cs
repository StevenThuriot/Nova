#region License
//   
//  Copyright 2013 Steven Thuriot
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  
#endregion

using System.Collections.ObjectModel;
using System.Diagnostics;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Domain;

namespace Nova.Shell.Views
{
    /// <summary>
    /// The Wizard's ViewModel
    /// </summary>
    public class WizardViewModel : ViewModel<WizardView, WizardViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardViewModel"/> class.
        /// </summary>
        public WizardViewModel()
        {
            Buttons = new ObservableCollection<WizardButton>();

            var test = new WizardButton("Test", new RelayCommand(() => Debug.WriteLine("test")));
            Buttons.Add(test);
        }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public ObservableCollection<WizardButton> Buttons { get; private set; }


        //TODO: Add Wizard property containing the multistep view.
        public IView Wizard { get; set; }
    }
}