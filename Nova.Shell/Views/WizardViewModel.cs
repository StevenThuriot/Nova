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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Nova.Library;
using Nova.Shell.Actions.Wizard;
using Nova.Shell.Builders;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Views
{

    /// <summary>
    /// The Wizard's ViewModel
    /// </summary>
    public class WizardViewModel : ViewModel<WizardView, WizardViewModel>, IWizard
    {
        private IDisposable _deferral;
        private LinkedListNode<NovaStep> _initialView;
        private MultiStepView _multiStepView;

        /// <summary>
        /// Gets the session view model.
        /// </summary>
        /// <value>
        /// The session view model.
        /// </value>
        internal ISessionViewModel SessionViewModel { get; private set; }

        /// <summary>
        /// Gets the multi step view.
        /// </summary>
        /// <value>
        /// The multi step view.
        /// </value>
        internal MultiStepView MultiStepView
        {
            get { return _multiStepView; }
            private set
            {
                _multiStepView = value;
                View.Content = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardViewModel"/> class.
        /// </summary>
        public WizardViewModel()
        {
            _deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
        }

        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(CancelAction), typeof(FinishAction));

            var multiStepView = new MultiStepView(View, SessionViewModel, ID, _initialView);

            MultiStepView = multiStepView;

            var keyGesture = new KeyGesture(Key.Escape);
            var relayCommand = new RelayCommand(Cancel);
            var inputBinding = new KeyBinding(relayCommand, keyGesture);
            
            View.InputBindings.Add(inputBinding);
        }

        internal void Initialize(ISessionViewModel sessionViewModel, WizardBuilder builder)
        {
            var steps = builder.Build(ID);
            var linkedSteps = new LinkedList<NovaStep>(steps);
            Initialize(sessionViewModel, linkedSteps.First);
        }

        internal void Initialize(ISessionViewModel sessionViewModel, LinkedListNode<NovaStep> initialView)
        {
            using (_deferral)
            {
                if (sessionViewModel == null)
                    throw new ArgumentNullException("sessionViewModel");

                if (initialView == null)
                    throw new ArgumentNullException("initialView");

                SessionViewModel = sessionViewModel;
                _initialView = initialView;
            }

            _deferral = null;
        }


        public sealed override Task<bool> Enter()
        {
            return InvokeActionAsync<EnterWizardAction>();
        }


        /// <summary>
        /// Creates the wizard button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public IWizardButton CreateWizardButton(string title, Action<object> action, Predicate<object> canExecute = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            var command = new RelayCommand(action, canExecute);
            var button = new WizardButton(title, command);

            return button;
        }

        /// <summary>
        /// Does the step.
        /// </summary>
        /// <param name="step">The step.</param>
        public void DoStep(StepInfo step)
        {
            if (step == null)
                throw new ArgumentNullException("step");

            var entry = ActionContextEntry.Create(step, false);
            InvokeAction<WizardNavigationAction>(entry);
        }

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        public void Finish(IEnumerable<ActionContextEntry> entries)
        {
            InvokeAction<FinishAction>(entries.ToArray());
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            InvokeAction<CancelAction>();
        }


        protected override void DisposeManagedResources()
        {
            MultiStepView.Dispose();
        }
    }
}