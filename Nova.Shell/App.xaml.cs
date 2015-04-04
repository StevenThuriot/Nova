using System;
using System.Diagnostics;
using System.Dynamic;
using Nova.Library;
using Nova.Shell.Managers;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic Model { get; private set; }

        /// <summary>
        /// Gets the module manager.
        /// </summary>
        /// <value>
        /// The module manager.
        /// </value>
        internal CompositionManager CompositionManager { get; private set; }

        public App()
        {
            //TODO: When configuration classes have been made, check if single instance is enabled.
            //var singleInstanceString = ConfigurationManager.AppSettings.GetValues("SingleInstance").FirstOrDefault();

            //bool singleInstance;
            //if (bool.TryParse(singleInstanceString, out singleInstance) && singleInstance)
            //{
            //    bool createdNew;
            //    var mutex = new Mutex(false, "#Nova.Shell#", out createdNew);

            //    GC.KeepAlive(mutex);

            //    if (!createdNew)
            //    {
            //        mutex.Dispose();
            //        MessageBox.Show("App already running.");

            //        Shutdown();
            //        return;
            //    }

            //    Exit += (sender, args) => mutex.Dispose();
            //}

            Startup += NovaFramework.Initialize;
#if DEBUG
            DispatcherUnhandledException += (sender, args) => Debugger.Break();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debugger.Break();

            const bool showStackTrace = true;
#else   
            const bool showStackTrace = false;
#endif
            ExceptionHandler.ShowStackTrace = showStackTrace;
            Model = new ExpandoObject();
            CompositionManager = new CompositionManager();
        }
    }
}
