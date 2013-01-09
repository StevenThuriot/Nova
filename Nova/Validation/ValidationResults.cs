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
using System.Collections;
using System.Collections.Generic;

namespace Nova.Validation
{
	/// <summary>
	/// List of validation messages.
	/// </summary>
	public class ValidationResults : IEnumerable<BaseValidation>
	{
		private readonly List<BaseValidation> _Validations;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationResults"/> class.
		/// </summary>
		public ValidationResults()
		{
			_Validations = new List<BaseValidation>();
		}

		/// <summary>
		/// Adds the specified validation.
		/// </summary>
		/// <param name="validation">The validation.</param>
		internal void InternalAdd(BaseValidation validation)
		{
			_Validations.Add(validation);
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		internal void InternalReset()
		{
			_Validations.Clear();
		}

		/// <summary>
		/// Gets the validations.
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<BaseValidation> InternalGetValidations()
		{
			return _Validations.AsReadOnly();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid
		{
			get { return _Validations == null || _Validations.Count == 0; }
		}
		
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<BaseValidation> GetEnumerator()
		{
			return _Validations.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
