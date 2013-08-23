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
using System.Collections.Generic;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    ///     Wizard
    /// </summary>
    internal interface IWizard
    {
        /// <summary>
        ///     Creates the wizard button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">title</exception>
        IWizardButton CreateWizardButton(string title, Action<object> action, Predicate<object> canExecute = null);

        /// <summary>
        /// Navigates to the specified step.
        /// </summary>
        /// <param name="step">The step.</param>
        void DoStep(StepInfo step);

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        /// <param name="entries"></param>
        void Finish(IEnumerable<ActionContextEntry> entries);

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        void Cancel();
    }
}