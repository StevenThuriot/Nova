﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nova.Validation
{
	/// <summary>
	/// List of validation messages.
	/// </summary>
	public class ValidationResults : IEnumerable<BaseValidation>
	{
		private readonly List<BaseValidation> _validations;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationResults"/> class.
		/// </summary>
		public ValidationResults()
		{
			_validations = new List<BaseValidation>();
		}

		/// <summary>
		/// Adds the specified validation.
		/// </summary>
		/// <param name="validation">The validation.</param>
		internal void InternalAdd(BaseValidation validation)
		{
			_validations.Add(validation);
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		internal void InternalReset()
		{
			_validations.Clear();
		}

		/// <summary>
		/// Gets the validations.
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<BaseValidation> InternalGetValidations()
		{
			return _validations.OrderByDescending(x => x.Severity).ToList().AsReadOnly();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid
		{
			get { return _validations == null || _validations.Count == 0; }
		}
		
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<BaseValidation> GetEnumerator()
		{
			return InternalGetValidations().GetEnumerator();
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
