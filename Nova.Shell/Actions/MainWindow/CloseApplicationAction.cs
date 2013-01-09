#region License

// 
//  Copyright 2012 Steven Thuriot
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
using System.Windows;
using Nova.Base;
namespace Nova.Shell.Actions.MainWindow
{
    public class CloseApplicationAction : BaseAction<MainView, MainViewModel>
    {
        public override bool Execute()
        {
            var isLoading = ActionContext.GetValue<bool>("IsLoading");
            if (isLoading)
            {
                var dialog = MessageBox.Show("Close app?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (dialog == MessageBoxResult.No)
                    return false;
            }

            return true;
        }

        public override void ExecuteCompleted()
        {
            Application.Current.Shutdown();
        }
    }
}
