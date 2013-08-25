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

using System;
using System.Windows.Input;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Buttons in a wizard
    /// </summary>
    public class WizardButton : IWizardButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardButton"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="navigationalCommand">The navigational command.</param>
        /// <exception cref="System.ArgumentNullException">
        /// title
        /// or
        /// navigationalCommand
        /// </exception>
        public WizardButton(string title, ICommand navigationalCommand)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            if (navigationalCommand == null)
                throw new ArgumentNullException("navigationalCommand");

            Title = title;
            NavigationalCommand = navigationalCommand;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the navigational command.
        /// </summary>
        /// <value>
        /// The navigational command.
        /// </value>
        public ICommand NavigationalCommand { get; private set; }
    }
}