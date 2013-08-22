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

using Nova.Controls;
using Nova.Library;
using Nova.Threading.Metadata;

namespace Nova.Shell.Library.Actions.Wizard
{
    /// <summary>
    /// Validatable Finish Wizard Action
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating]
    public class ValidatableFinishAction<TView, TViewModel> : ValidatableActionflow<TView, TViewModel>
        where TView : class, IView
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        public sealed override bool Execute()
        {
            return base.Execute() && Finish();
        }

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        /// <returns></returns>
        protected virtual bool Finish()
        {
            return true;
        }

        public sealed override void ExecuteCompleted()
        {
            base.ExecuteCompleted();
            ViewModel.Finish();
        }
    }
}