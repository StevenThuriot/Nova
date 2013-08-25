#region License
// 
// Copyright 2013 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Windows;
using System.Windows.Input;
using RESX = Nova.Shell.Properties.Resources;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for SessionView.xaml
    /// </summary>
    public partial class SessionView
    {
        /// <summary>
        /// Initializes the <see cref="SessionView" /> class.
        /// </summary>
        static SessionView()
        {
            var propertyMetadata = new FrameworkPropertyMetadata(RESX.EmptySession)
            {
                CoerceValueCallback = OnCoerceTitleCallBack
            };

            TitleProperty.OverrideMetadata(typeof (SessionView), propertyMetadata);
        }

        private static object OnCoerceTitleCallBack(DependencyObject d, object basevalue)
        {
            return string.IsNullOrWhiteSpace(basevalue as string)
                ? RESX.EmptySession
                : basevalue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionView" /> class.
        /// </summary>
        public SessionView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public override void StartLoading()
        {
            base.StartLoading();
            Cursor = Cursors.AppStarting;
        }

        /// <summary>
        /// Stops the animated loading.
        /// </summary>
        public override void StopLoading()
        {
            base.StopLoading();

            if (!IsLoading)
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}
