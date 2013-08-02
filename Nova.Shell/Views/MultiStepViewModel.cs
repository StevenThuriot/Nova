#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using Nova.Controls;
using Nova.Library;

namespace Nova.Shell.Views
{
    /// <summary>
    /// The Multi step view model
    /// </summary>
    internal class MultiStepViewModel : ViewModel<MultiStepView, MultiStepViewModel>
    {
        private IDisposable _deferral;
        private IEnumerable<IView> _steps;

        public IEnumerable<IView> Steps
        {
            get { return _steps; }
            private set { SetValue(ref _steps, value); }
        }

        private LinkedListNode<IView> _currentView;
        public LinkedListNode<IView> CurrentView
        {
            get { return _currentView; }
            set
            {
                if (value == null || !SetValue(ref _currentView, value)) 
                    return;

                View.Content = _currentView.Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStepViewModel"/> class.
        /// </summary>
        public MultiStepViewModel()
        {
            _deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
        }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <exception cref="System.ArgumentNullException">steps</exception>
        public void Initialize(IEnumerable<IView> steps)
        {
            if (steps == null)
                throw new ArgumentNullException("steps");

            var linkedSteps = new LinkedList<IView>(steps);

            Initialize(linkedSteps);
        }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <exception cref="System.ArgumentNullException">steps</exception>
        /// <exception cref="System.ArgumentException">Steps cannot be empty.;steps</exception>
        public void Initialize(LinkedList<IView> steps)
        {
            using (_deferral)
            {
                if (steps == null)
                    throw new ArgumentNullException("steps");

                if (steps.Count == 0)
                    throw new ArgumentException(@"Steps cannot be empty.", "steps");

                Steps = steps;

                _deferral = null;
            }
        }




        /// <summary>
        /// Determines whether this instance can go to next step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to next step]; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoToNextStep()
        {
            return CurrentView.Next != null;
        }

        /// <summary>
        /// Goes to next step.
        /// </summary>
        public void GoToNextStep()
        {
            DoStep(CurrentView.Next);
        }

        /// <summary>
        /// Determines whether this instance can go to previous step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to previous step]; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoToPreviousStep()
        {
            return CurrentView.Previous != null;
        }

        /// <summary>
        /// Goes to previous step.
        /// </summary>
        public void GoToPreviousStep()
        {
            DoStep(CurrentView.Previous);
        }

        /// <summary>
        /// Attempts to do a step to the specified step.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <returns>True is successful.</returns>
        public bool DoStep(string stepName)
        {
            var node = CurrentView.List.First;

            while (node != null)
            {
                if (node.Value.Title == stepName)
                    break;

                node = node.Next;
            }

            return DoStep(node);
        }

        private bool DoStep(LinkedListNode<IView> node)
        {
            if (node == null || node.Value == null)
                return false;

            CurrentView = node;
            return true;
        }





        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            if (_deferral == null) return;

            _deferral.Dispose();
            _deferral = null;
        }
    }
}