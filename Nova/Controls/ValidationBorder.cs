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

using System.Windows;
using System.Windows.Controls;

namespace Nova.Controls
{
    /// <summary>
    /// A validation border to use with Nova's validation system when grouping several controls.
    /// </summary>
    public class ValidationBorder : ContentControl 
    {
        /// <summary>
        /// Initializes the <see cref="ValidationBorder" /> class.
        /// </summary>
        static ValidationBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationBorder), new FrameworkPropertyMetadata(typeof(ValidationBorder)));
        }

        /// <summary>
        /// The is filled in property
        /// </summary>
        public static readonly DependencyProperty IsFilledInProperty =
            DependencyProperty.Register("IsFilledIn", typeof (bool), typeof (ValidationBorder), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filled in.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is filled in; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilledIn
        {
            get { return (bool) GetValue(IsFilledInProperty); }
            set { SetValue(IsFilledInProperty, value); }
        }
    }
}
