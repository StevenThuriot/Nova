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

using System.Windows;
using System.Windows.Data;

namespace Nova.Library
{
	/// <summary>
	/// Inherits from binding to make binding to the error list easier. (Used for validation)
	/// </summary>
	public class ErrorBinding : Binding
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// Default error binding, uses "ErrorCollection" as its path.
		/// </summary>
		/// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path"/> for the binding.</param>
		private ErrorBinding(string path)
			: base(path)
		{
			Mode = BindingMode.OneWay;
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// Default error binding, uses "ErrorCollection" as its path.
		/// </summary>
		public ErrorBinding()
			: this("ErrorCollection")
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorBinding"/> class.
		/// </summary>
		/// <param name="errors">The errors.</param>
		public ErrorBinding(PropertyPath errors) : this(errors.Path)
		{
		}
	}
}
