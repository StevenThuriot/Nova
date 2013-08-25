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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nova.Shell.Controls
{
    /// <summary>
    /// Interaction logic for SessionControl.xaml
    /// </summary>
    public partial class SessionControl
    {
        /// <summary>
        /// The sessions property
        /// </summary>
        public static readonly DependencyProperty SessionsProperty =
            DependencyProperty.Register("Sessions", typeof (IEnumerable<SessionView>), typeof (SessionControl), new PropertyMetadata(null));

        /// <summary>
        /// The current session property
        /// </summary>
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof (SessionView), typeof (SessionControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// The add command property
        /// </summary>
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof (ICommand), typeof (SessionControl), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionControl"/> class.
        /// </summary>
        public SessionControl()
        {
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;

            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public IEnumerable<SessionView> Sessions
        {
            get { return (IEnumerable<SessionView>) GetValue(SessionsProperty); }
            set { SetValue(SessionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public SessionView CurrentSession
        {
            get { return (SessionView) GetValue(CurrentSessionProperty); }
            set { SetValue(CurrentSessionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the add command.
        /// </summary>
        /// <value>
        /// The add command.
        /// </value>
        public ICommand AddCommand
        {
            get { return (ICommand) GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }
    }
}
