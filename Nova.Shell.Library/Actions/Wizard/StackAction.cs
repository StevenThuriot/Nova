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
    /// Action used for stacking a wizard on top of the current step.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating, Alias(Aliases.Stack)]
    public abstract class StackAction<TView, TViewModel> : Actionflow<TView, TViewModel>
        where TView : class, IView
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        private IWizardBuilder _builder;

        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            if (!base.Execute())
                return false;

            if (!Stack())
                return false;

            _builder = ViewModel.Session.CreateWizardBuilder();
            BuildWizard(_builder);

            return true;
        }

        /// <summary>
        /// Builds the wizard.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected abstract void BuildWizard(IWizardBuilder builder);

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            ViewModel.Session.StackWizard(_builder);
            StackCompleted();
        }

        /// <summary>
        /// Called when stacking a wizard.
        /// </summary>
        /// <returns></returns>
        public virtual bool Stack()
        {
            return true;
        }
        
        /// <summary>
        /// Called when stacking a wizard has completed.
        /// </summary>
        public virtual void StackCompleted()
        {
        }
    }
}
