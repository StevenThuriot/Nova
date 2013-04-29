using Nova.Library;

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
using Nova.Controls;
using Nova.Threading.Metadata;

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

        public override bool CanExecute()
        {
            return ViewModel.CurrentView == null || !ViewModel.CurrentView.IsLoading;
        }

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

            return EnterCurrentAndLeaveOldStep().Result; //.Result because we want to block this thread until execution finishes!
        }

        private async Task<bool> EnterCurrentAndLeaveOldStep()
        {
            if (_Current != null)
            {
                var canLeave = await _Current.ViewModel.Leave();

                if (!canLeave)
                    return false;
            }

            var result = await _NextView.ViewModel.Enter();
            
            if (!result)
            {
                //TODO: Set _NextView to "Step not available" view and enter that instead.
            }

            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentView = _NextView;
            
            //TODO: Set Correct Node as current.
            //foreach (var node in ViewModel.TreeNodes)
            //{
            //    node.CheckCurrentState();
            //}
        }

        protected override void DisposeManagedResources()
        {
            _Current = null;
            _NextView = null;
        }
    }
}
