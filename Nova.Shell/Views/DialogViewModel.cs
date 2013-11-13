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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Nova.Library;
using Nova.Shell.Library;
using RESX=Nova.Shell.Library.Properties.Resources;
namespace Nova.Shell.Views
{
    /// <summary>
    /// Dialog view model
    /// </summary>
    public class DialogViewModel : WizardContentViewModel<DialogView, DialogViewModel>
    {
        private string _message;
        private Visibility _imageVisibility = Visibility.Collapsed;
        private ImageSource _image;

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

        /// <summary>
        /// Gets the image visibility.
        /// </summary>
        /// <value>
        /// The image visibility.
        /// </value>
        public Visibility ImageVisibility
        {
            get { return _imageVisibility; }
            private set { SetValue(ref _imageVisibility, value); }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public ImageSource Image
        {
            get { return _image; }
            private set { SetValue(ref _image, value); }
        }

        protected override IEnumerable<IWizardButton> CreateButtons(ActionContext context)
        {
            IEnumerable<object> buttons;
            if (context.TryGetValue(ActionContextConstants.DialogBoxButtons, out buttons))
            {
                ActionContextEntry[] entries = { ActionContextEntry.Create(ActionContextConstants.SessionDialogBox, true, false)};
                
                return buttons.Reverse().Select(x => CreateButton(x.ToString(), _ => RunFinishAction(x, entries))).ToList().AsReadOnly();
            }

            var ok = RESX.OK;
            var wizardButton = CreateButton(ok, _ => RunFinishAction(ok));
            return new[] { wizardButton };
        }

        public void OnBeforeEnterAction(ActionContext context)
        {
            Message = context.GetValue<string>(ActionContextConstants.DialogBoxMessage);
            
            ImageSource imageSource;
            if (context.TryGetValue(ActionContextConstants.DialogBoxImage, out imageSource))
            {
                Image = imageSource;
                ImageVisibility = Visibility.Visible;
            }
        }
    }
}