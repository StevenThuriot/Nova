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

using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Nova.Shell.Builders;
using Nova.Library.Actions;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Composition and initialization of the main viewmodel.
    /// </summary>
    public class ComposeAndInitializeAction : EnterAction<MainView, MainViewModel>
    {
        private BitmapImage _icon;

        public override bool Enter()
        {
            ModuleComposer.Compose();
            ReadConfiguration();

            return true;
        }

        public override void EnterCompleted()
        {
            ViewModel.Icon = _icon;

            var intialSession = ViewModel.CreateSession();

            if (intialSession != null)
            {
                ViewModel.Sessions.Add(intialSession);
                ViewModel.CurrentSession = intialSession;
            }

            //TODO: Hide splash screen.
            View.Visibility = Visibility.Visible;
        }

        protected override void DisposeManagedResources()
        {
            _icon = null;
        }













        private void ReadConfiguration()
        {
            SetIcon();
        }

        private void SetIcon()
        {
            var value = GetAppSettingsFor("Icon");

            var iconUri = string.IsNullOrWhiteSpace(value)
                              ? new Uri("pack://application:,,,/Nova.Shell;component/Nova.ico")
                              : new Uri(value);

            _icon = new BitmapImage();
            _icon.BeginInit();
            _icon.UriSource = iconUri;
            _icon.EndInit();
            _icon.Freeze();
        }

        private static string GetAppSettingsFor(string key)
        {
            var settings = ConfigurationManager.AppSettings.GetValues(key);
            return settings == null ? "" : settings.FirstOrDefault();
        }
    }
}
