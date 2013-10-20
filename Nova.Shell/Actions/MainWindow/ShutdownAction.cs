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

using System.Windows;
using Nova.Library.Actions;
using RESX = Nova.Shell.Properties.Resources;

namespace Nova.Shell.Actions.MainWindow
{
    public class ShutdownAction : LeaveAction<MainView, MainViewModel>
    {
        private bool _isLoading;

        public override void OnBefore()
        {
            _isLoading = View.IsLoading;
        }

        public override bool Leave()
        {
            if (!_isLoading) return true;
            
            var dialog = MessageBox.Show(View, RESX.CloseApplication, RESX.CloseApplicationTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);

            return dialog == MessageBoxResult.Yes;
        }
        public override void LeaveCompleted()
        {
            Application.Current.Shutdown();
        }
    }
}
