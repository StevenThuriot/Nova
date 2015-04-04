using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Nova.Library.Actions;
using Nova.Shell.Builders;
using Nova.Shell.Library;

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
            ComposeModules();
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
