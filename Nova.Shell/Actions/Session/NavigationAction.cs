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
using System.Threading.Tasks;
using Nova.Base;
using Nova.Controls;
using Nova.Threading;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// Navigational action
    /// </summary>
    [Blocking]
    public class NavigationAction : Actionflow<SessionView, SessionViewModel>
    {
        private IView _NextView;
        private IView _Current;

        public override void OnBefore()
        {
            var createNextView = ActionContext.GetValue<Func<IView>>(SessionViewModel.NextViewConstant);
            
            if (createNextView != null)
            {
                _NextView = createNextView();
            }
        }

        public override bool Execute()
        {
            if (_NextView == null)
                return false;

            _Current = ActionContext.GetValue<IView>(SessionViewModel.CurrentViewConstant);

            return EnterCurrentAndLeaveOldStep().Result;
        }

        private async Task<bool> EnterCurrentAndLeaveOldStep()
        {
            var result = await _NextView.ViewModel.Enter();

            if (!result)
            {
                //Can't enter the new step.
                await _NextView.ViewModel.Leave();
                return false;
            }

            if (_Current == null)
                return true;

            result = await _Current.ViewModel.Leave(); 
            
            if (!result)
            {
                //Leaving the old step has been blocked by the leave action (e.g. by a dirty viewmodel that requires saving first).
                await _NextView.ViewModel.Leave();
                return false;
            }
            
            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentView = _NextView;
        }
    }
}
