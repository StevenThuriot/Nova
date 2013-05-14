﻿using Nova.Library.Actions;

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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Nova.Shell.Builders;
using Nova.Shell.Library;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Composition and initialization of the main viewmodel.
    /// </summary>
    public class ComposeAndInitializeAction : EnterAction<MainView, MainViewModel>
    {
        private BitmapImage _Icon;

        public override bool Enter()
        {
            ComposeModules();
            ReadConfiguration();

            return true;
        }

        public override void EnterCompleted()
        {
            ViewModel.Icon = _Icon;

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
            _Icon = null;
        }













        private static void ComposeModules()
        {
            var builders = new List<ModuleBuilder>();

            var app = (App)Application.Current;
            var container = app.CompositionManager.CompositionContainer;
            var moduleConfigurations = container.GetExportedValues<IModule>();

            foreach (var module in moduleConfigurations)
            {
                var builder = new ModuleBuilder();
                module.Configure(builder);
                builders.Add(builder);
            }

            //Sort by ranking: Descending
            builders.Sort((x, y) => x.Ranking < y.Ranking ? 1 : (x.Ranking > y.Ranking ? -1 : 0));

            var modules = builders.Select(x => x.Build())
                                  .ToList()
                                  .AsReadOnly();

            app.Model.Modules = modules;
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
    }
}