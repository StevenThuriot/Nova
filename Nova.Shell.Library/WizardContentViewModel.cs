﻿#region License
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

namespace Nova.Shell.Library
{
    /// <summary>
    ///     A viewmodel for pages that belong to a wizard.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class WizardContentViewModel<TView, TViewModel> : ContentViewModel<TView, TViewModel>
        where TView : class, IView
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        private IWizard _wizard;
        private IMultiStep _multistep;
        
        /// <summary>
        /// Gets the current step.
        /// </summary>
        /// <value>
        /// The current step.
        /// </value>
        public LinkedListNode<StepInfo> CurrentStep { get; private set; }

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
            get
            {
                return CurrentStep.Previous;
            }
        }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        public IEnumerable<IWizardButton> Buttons { get; private set; }
        
        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        internal override void Initialize(IDictionary<string, object> initializer)
        {
            try
            {
                CurrentStep = (LinkedListNode<StepInfo>)initializer["Node"];
                _wizard = (IWizard) initializer["Wizard"];
                _multistep = (IMultiStep)initializer["Multistep"];
                
                var buttons = CreateButtons();

                if (buttons == null || !buttons.Any())
                    throw new NotSupportedException("Created buttons are invalid.");

                Buttons = buttons;
            }
            finally
            {
                base.Initialize(initializer);
            }
        }

        /// <summary>
        /// Creates the buttons.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual IEnumerable<IWizardButton> CreateButtons()
        {
            var list = new List<IWizardButton>();

            var cancelButton = CreateCancelButton();
            list.Add(cancelButton);
            
            list.Add(_multistep.CanGoToNextStep() ? CreateNextButton() : CreateFinishButton());

            if (_multistep.CanGoToPreviousStep())
                list.Add(CreatePreviousButton());


            return list;
        }


        /// <summary>
        /// Creates the button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns></returns>
        protected IWizardButton CreateWizardButton(string title, Action<object> action, Predicate<object> canExecute = null)
        {
            return _wizard.CreateWizardButton(title, action, canExecute);
        }



        /// <summary>
        /// Creates the previous button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreatePreviousButton()
        {
            return CreateWizardButton("Previous", _ => DoStep(PreviousStep), _ => PreviousStep != null && PreviousStep.Value != null);
        }

        /// <summary>
        /// Creates the next button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateNextButton()
        {
            return CreateWizardButton("Next", _ => DoStep(NextStep), _ => NextStep != null && NextStep.Value != null);
        }

        /// <summary>
        /// Creates the finish button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateFinishButton()
        {
            return CreateWizardButton("Finish", _ => _multistep.Finish());
        }

        /// <summary>
        /// Creates the cancel button.
        /// </summary>
        /// <returns></returns>
        protected IWizardButton CreateCancelButton()
        {
            return CreateWizardButton("Cancel", _ => _multistep.Cancel(), _ => CanCancel);
        }


        /// <summary>
        /// Does the step to the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected bool DoStep(LinkedListNode<StepInfo> node)
        {
            return node != null && DoStep(node.Value);
        }

        /// <summary>
        /// Does the step to the specified node.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        protected bool DoStep(StepInfo step)
        {
            //TODO: ActionQueue!
            return step != null && _multistep.DoStep(step.NodeID);
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
    }
}