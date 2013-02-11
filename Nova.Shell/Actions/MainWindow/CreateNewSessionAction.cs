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

using System.Linq;
using System.Windows.Input;
using Nova.Base;

namespace Nova.Shell.Actions.MainWindow
{
    public class CreateNewSessionAction : Actionflow<MainView, MainViewModel>
    {
        private bool _HasOpenSessions;

        public override bool Execute()
        {
            _HasOpenSessions = ViewModel.Sessions.Any();
            return true;
        }

        public override void ExecuteCompleted()
        {
            var session = View.CreatePage<SessionView, SessionViewModel>();
            ViewModel.Sessions.Add(session);
            
            if (!_HasOpenSessions || Keyboard.Modifiers != (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ViewModel.CurrentSession = session;
            }
        }
    }
}
