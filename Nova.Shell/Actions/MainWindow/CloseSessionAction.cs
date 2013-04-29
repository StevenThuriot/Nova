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

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action to close a session.
    /// </summary>
    public class CloseSessionAction : Actionflow<MainView, MainViewModel>
    {
        private SessionView _Session;

        public override bool Execute()
        {
            var canComplete = ActionContext.TryGetValue(out _Session) && _Session != null;

            if (!canComplete)
                return false;

            var canLeave = _Session.ViewModel.Leave().Result;

            return canLeave;
        }

        public override void ExecuteCompleted()
        {
            //TODO: Check if leave was successful.
            ViewModel.Sessions.Remove(_Session);
        }
    }
}
