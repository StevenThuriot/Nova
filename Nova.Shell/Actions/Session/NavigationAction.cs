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
using Nova.Shell.Views;
using Nova.Threading.Metadata;
using Nova.Library;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// Navigational action
    /// </summary>
    [Blocking]
    public class NavigationAction : Actionflow<SessionView, SessionViewModel>
    {
        private IView _nextView;
        private IView _current;

        private Type _pageType;
        private Type _viewModelType;

        public override bool CanExecute()
        {
            return ViewModel.CurrentView == null || !ViewModel.CurrentView.IsLoading;
        }

        public override void OnBefore()
        {
            var createNextView = ActionContext.GetValue<Func<IView>>(SessionViewModel.CreateNextViewConstant);
            
            if (createNextView != null)
            {
                _nextView = createNextView();
            }
        }

        public override bool Execute()
        {
            if (_nextView == null)
                return false;

            _current = ActionContext.GetValue<IView>(SessionViewModel.CurrentViewConstant);

            return EnterCurrentAndLeaveOldStep().Result; //.Result because we want to block this thread until execution finishes!
        }

        private async Task<bool> EnterCurrentAndLeaveOldStep()
        {
            if (_current != null)
            {
                var canLeave = await _current.ViewModel.Leave();

                if (!canLeave)
                    return false;
            }

            _pageType = _nextView.GetType();
            _viewModelType = _nextView.ViewModel.GetType();

            var result = await _nextView.ViewModel.Enter();

            if (!result)
            {
                StepNotAvailableView notAvailableView = null;
                await View.Dispatcher.InvokeAsync(() => notAvailableView = ViewModel.CreatePage<StepNotAvailableView, StepNotAvailableViewModel>(true));
                
                notAvailableView.ViewModel.StepName = _pageType.Name; //TODO: Find actual name in tree.
                _nextView = notAvailableView;
            }

            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentView = _nextView;
            View._NovaTree.ReevaluateState(_pageType, _viewModelType);
        }

        protected override void DisposeManagedResources()
        {
            _current = null;
            _nextView = null;
        }
    }
}
