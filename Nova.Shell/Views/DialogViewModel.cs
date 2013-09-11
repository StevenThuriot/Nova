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

using System.Collections.Generic;
using Nova.Library;
using Nova.Shell.Library;

namespace Nova.Shell.Views
{
    /// <summary>
    /// Dialog view model
    /// </summary>
    public class DialogViewModel : WizardContentViewModel<DialogView, DialogViewModel>
    {
        private string _message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return _message; }
            private set { SetValue(ref _message, value); }
        }

        protected override IEnumerable<IWizardButton> CreateButtons()
        {
            var wizardButton = CreateButton("Ok", _ => RunFinishAction());

            return new[] { wizardButton };
        }

        public void OnBeforeEnter(ActionContext context)
        {
            Message = context.GetValue<string>(ActionContextConstants.DialogBoxMessage);
        }
    }
}