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

using Nova.Library;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action to focus a certain tab of choice.
    /// </summary>
    public class FocusTabAction : Actionflow<MainView, MainViewModel>
    {
        private SessionView _Session;

        public override bool Execute()
        {
            var index = ActionContext.GetValue<string>(RoutedAction.CommandParameter);

            int sessionIndex;

            if (string.IsNullOrWhiteSpace(index) || 
                !int.TryParse(index, out sessionIndex) ||
                ViewModel.Sessions.Count <= sessionIndex)
            {
                return false;
            }

            _Session = ViewModel.Sessions[sessionIndex];
            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentSession = _Session;
        }
    }
}
