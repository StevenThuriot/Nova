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
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Library.Actions.Wizard;
using RESX = Nova.Shell.Library.Properties.Resources;

namespace Nova.Shell.Library
{
    /// <summary>
    ///     A viewmodel for pages that belong to a wizard.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class WizardContentViewModel<TView, TViewModel> : MultistepContentViewModel<TView, TViewModel>
        where TView : class, IView
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        private IWizard _wizard;

        /// <summary>
        /// Gets the next step.
        /// </summary>
        /// <value>
        /// The next step.
        /// </value>
        public virtual LinkedListNode<StepInfo> NextStep
        {
            get
            {
                return CurrentStep.Next;
            }
        }

        /// <summary>
        /// Gets the previous step.
        /// </summary>
        /// <value>
        /// The previous step.
        /// </value>
        public virtual LinkedListNode<StepInfo> PreviousStep
        {
            get { return _wizard.PreviousStep; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can cancel.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can cancel; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanCancel
        {
            get { return true; }
        }


        private IEnumerable<IWizardButton> _buttons;
        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public IEnumerable<IWizardButton> Buttons
        {
            get { return _buttons; }
            private set { SetValue(ref _buttons, value); }
        }


        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        internal override void Initialize(IDictionary<string, object> initializer, bool triggerDeferal = true)
        {
            base.Initialize(initializer, false);
            _wizard = (IWizard)initializer["Wizard"];

            if (triggerDeferal) TriggerDeferal();
        }

        protected void OnBeforeEnter(ActionContext context)
        {
            var buttons = CreateButtons(context);

            if (buttons == null || !buttons.Any())
                throw new NotSupportedException("Created buttons are invalid.");

            Buttons = buttons;
        }

        /// <summary>
        /// Creates the buttons.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual IEnumerable<IWizardButton> CreateButtons(ActionContext context)
        {
            var list = new List<IWizardButton>();

            if (CanCancel) list.Add(CreateCancelButton());

            list.Add(NextStep != null ? CreateNextButton() : CreateFinishButton());

            if (PreviousStep != null) list.Add(CreatePreviousButton());
            
            return list;
        }


        /// <summary>
        /// Creates the button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns></returns>
        protected IWizardButton CreateButton(string title, Action<object> action, Predicate<object> canExecute = null)
        {
            return _wizard.CreateWizardButton(title, action, canExecute);
        }

        /// <summary>
        /// Creates the previous button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreatePreviousButton()
        {
            return CreateButton(RESX.Previous, _ => DoStep(PreviousStep), _ => PreviousStep != null && PreviousStep.Value != null);
        }

        /// <summary>
        /// Creates the next button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateNextButton()
        {
            return CreateButton(RESX.Next, _ => DoStep(NextStep), _ => NextStep != null && NextStep.Value != null);
        }

        /// <summary>
        /// Creates the finish button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateFinishButton()
        {
            var finish = RESX.Finish;
            return CreateButton(finish, _ => RunFinishAction(finish));
        }

        /// <summary>
        /// Runs the finish action.
        /// </summary>
        protected virtual void RunFinishAction(object result = null, params ActionContextEntry[] entries)
        {
            if (result != null)
            {
                var entry = ActionContextEntry.Create(ActionContextConstants.DialogBoxResult, result, false);
                var actionContextEntries = entries.ToList();
                actionContextEntries.Add(entry);

                entries = actionContextEntries.ToArray();
            }

            InvokeAction<FinishAction<TView, TViewModel>>(entries);
        }

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        internal void Finish(IEnumerable<ActionContextEntry> entries)
        {
            _wizard.Finish(entries);
        }

        /// <summary>
        /// Creates the cancel button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateCancelButton()
        {
            return CreateButton(RESX.Cancel, _ => _wizard.Cancel(), _ => CanCancel);
        }


        /// <summary>
        /// Does the step to the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected void DoStep(LinkedListNode<StepInfo> node)
        {
            if (node == null)
                return;

            DoStep(node.Value);
        }

        /// <summary>
        /// Does the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected void DoStep(StepInfo step)
        {
            if (step == null)
                return;

            _wizard.DoStep(step);
        }
    }
}