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
namespace Nova.Validation
{
	/// <summary>
	/// The severity of the validation message. 
	/// Mainly used to be able to differentiate between shown control colors.
	/// </summary>
	public enum ValidationSeverity
	{
		/// <summary>
		/// Lowest level validation error, counts as a suggestion.
		/// </summary>
		Suggestion,
		/// <summary>
		/// Mid level validation error, counts as a warning.
		/// </summary>
		Warning,
		/// <summary>
		/// High level validation error, counts as an error.
		/// </summary>
		Error
	}
}
