﻿#region License

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

        public override bool Execute()
        {
            _NextView = ActionContext.GetValue<IView>(SessionViewModel.NextViewConstant);

            if (_NextView == null)
                return false;
            
            var current = ActionContext.GetValue<IView>(SessionViewModel.CurrentViewConstant);
            
            if (current != null)
            {
                //TODO: Research if possible to wait here.
                current.ViewModel.Leave();
            }

            return base.Execute();
        }

        public override void ExecuteCompleted()
        {
            _NextView.ViewModel.Enter();
            ViewModel.CurrentView = _NextView;
        }
    }
}
