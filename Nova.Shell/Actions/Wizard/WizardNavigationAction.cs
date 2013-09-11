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
using Nova.Shell.Domain;
using Nova.Shell.Library;
using Nova.Shell.Views;
using Nova.Threading.Metadata;

namespace Nova.Shell.Actions.Wizard
{
    /// <summary>
    /// Wizard Navigation Action
    /// </summary>
    [Blocking]
    public class WizardNavigationAction : Actionflow<WizardView, WizardViewModel>
    {
        private NovaStep _current;
        private NovaStep _nextStep;

        public override void OnBefore()
        {
            _current = ViewModel.MultiStepView.CurrentView.Value;

            var stepInfo = ActionContext.GetValue<StepInfo>();
            _nextStep = ViewModel.MultiStepView.GetNovaStep(stepInfo);
        }

        public override bool Execute()
        {
            var canLeave = _current.View.ViewModel.Leave(_current.Parameters).Result;

            if (!canLeave)
                return false;

            var previousStep = ViewModel.PreviousStep;
            var isReturning = previousStep != null && previousStep.Value.NodeId == _nextStep.NodeId;
            
            LinkedListNode<StepInfo> poppedStep = null;
            if (isReturning)
            {
                poppedStep = ViewModel.PreviousSteps.Pop();
            }
            else
            {
                var currentStepInfo = ViewModel.MultiStepView.GetStepInfoNode(_current);
                ViewModel.PreviousSteps.Push(currentStepInfo);
            }
            
            var nextView = _nextStep.GetOrCreateView(ViewModel.MultiStepView);
            
            if (nextView != null)
            {
                var nextViewModel = nextView.ViewModel;
                var result = nextViewModel.Enter(_nextStep.Parameters).Result;

                if (result)
                    return true;
            }

            return RollBack(isReturning, poppedStep);
        }

        private bool RollBack(bool isReturning, LinkedListNode<StepInfo> poppedStep)
        {
            if (isReturning)
                ViewModel.PreviousSteps.Push(poppedStep);
            else
                ViewModel.PreviousSteps.Pop();

            return _current.View.ViewModel.Enter(_current.Parameters).Result; // Stay on current.
        }

        public override void ExecuteCompleted()
        {
            ViewModel.MultiStepView.DoStep(_nextStep.NodeId);
        }
    }
}
