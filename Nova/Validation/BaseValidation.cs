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
using System;
using System.Windows;
using System.Windows.Media;

namespace Nova.Validation
{
	/// <summary>
	/// A validation error.
	/// </summary>
	public abstract class BaseValidation
	{
		/// <summary>
		/// Gets the ranking. 
		/// The higher, the more severe a validation message is.
		/// </summary>
		public abstract ValidationSeverity Severity { get; }

		/// <summary>
		/// Gets the field.
		/// </summary>
		public string Field { get; private set; }

	    /// <summary>
	    /// Gets the severity brush.
	    /// </summary>
	    /// <returns></returns>
	    public Brush SeverityBrush
	    {
	        get
	        {
	            var severity = Severity.ToString("G");
	            return Application.Current.Resources[severity] as Brush;
	        }
	    }

		/// <summary>
		/// Gets the message.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// Gets the entity ID. 
		/// </summary>
		public Guid EntityID { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseValidation"/> class.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="message">The message.</param>
		/// <param name="entityID">The entity ID.</param>
		protected BaseValidation(string field, string message, Guid entityID)
		{
			Field = field;
			Message = message;
			EntityID = entityID;
		}

        /// <summary>
        /// Checks if equal to the specified instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
	    public bool Equals(BaseValidation other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return string.Equals(Field, other.Field) && string.Equals(Message, other.Message) && EntityID.Equals(other.EntityID);
	    }

	    /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != GetType()) return false;
	        return Equals((BaseValidation) obj);
	    }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Field != null ? Field.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Message != null ? Message.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ EntityID.GetHashCode();
	            return hashCode;
	        }
	    }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(BaseValidation left, BaseValidation right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(BaseValidation left, BaseValidation right)
        {
            return !Equals(left, right);
        }
	}
}
