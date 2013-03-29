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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Nova.Base;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            //TODO: When configuration classes have been made, check if single instance is enabled.
            //var singleInstanceString = ConfigurationManager.AppSettings.GetValues("SingleInstance").FirstOrDefault();

            //bool singleInstance;
            //if (bool.TryParse(singleInstanceString, out singleInstance) && singleInstance)
            //{
            //    bool createdNew;
            //    var mutex = new Mutex(false, "#Nova.Shell#", out createdNew);

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
        }
    }
}
