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
using System;
using System.Configuration;
using System.Linq;
using System.Windows.Media.Imaging;
using Nova.Base;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action To read in App.Config
    /// </summary>
    public class ReadConfigurationAction : Actionflow<MainView, MainViewModel>
    {
        private BitmapImage _Icon;

        public override bool Execute()
        {
            SetIcon();

            return true;
        }

        private void SetIcon()
        {
            var value = GetAppSettingsFor("Icon");

            var iconUri = string.IsNullOrWhiteSpace(value)
                              ? new Uri("pack://application:,,,/Nova.Shell;component/Nova.ico")
                              : new Uri(value);

            _Icon = new BitmapImage();
            _Icon.BeginInit();
            _Icon.UriSource = iconUri;
            _Icon.EndInit();
            _Icon.Freeze();
        }

        private static string GetAppSettingsFor(string key)
        {
            var settings = ConfigurationManager.AppSettings.GetValues(key);
            return settings == null ? "" : settings.FirstOrDefault();
        }

        public override void ExecuteCompleted()
        {
            ViewModel.Icon = _Icon;
        }

        protected override void DisposeManagedResources()
        {
            _Icon = null;
        }
    }
}
