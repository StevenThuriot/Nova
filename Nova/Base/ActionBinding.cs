﻿#region License
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

using System;
using System.Windows.Data;

namespace Nova.Base
{
    /// <summary>
    /// A binding class to make binding to actions easier
    /// </summary>
    public class ActionBinding : Binding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBinding" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
		public ActionBinding(string path)
			: base("ActionManager." + path)
		{
			Mode = BindingMode.OneWay;
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBinding" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ActionBinding(Type type)
            :this(type.Name)
        {
            
        }
    }
}