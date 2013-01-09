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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Nova.Validation
{
	/// <summary>
	/// A validation error.
	/// </summary>
	public abstract class BaseValidation : INotifyPropertyChanged, IComparable<BaseValidation>, IEquatable<BaseValidation>
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets the ranking. 
		/// The higher, the more severe a validation message is.
		/// </summary>
		public abstract int Ranking { get; }

		/// <summary>
		/// Gets the field.
		/// </summary>
		public string Field { get; private set; }
		
		/// <summary>
		/// Gets the severity brush.
		/// </summary>
		/// <returns></returns>
		public abstract Brush SeverityBrush { get; }

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
		/// Called when [property changed].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
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
			return Equals(other.Field, Field);
		}

		/// <summary>
		/// Compares to the other instance.
		/// </summary>
		/// <param name="other">The other instance.</param>
		/// <returns></returns>
		public int CompareTo(BaseValidation other)
		{
			return string.Compare(Field, other.Field, CultureInfo.CurrentCulture, CompareOptions.Ordinal);
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
			if (obj.GetType() != typeof (BaseValidation)) return false;
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
			return (Field != null ? Field.GetHashCode() : 0);
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

		/// <summary>
		/// Implements the operator &lt;.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator <(BaseValidation left, BaseValidation right)
		{
			return Compare(left, right) < 0;
		}
		/// <summary>
		/// Implements the operator &gt;.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator >(BaseValidation left, BaseValidation right)
		{
			return Compare(left, right) > 0;
		}

		/// <summary>
		/// Compares the specified instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns></returns>
		public static int Compare(BaseValidation left, BaseValidation right)
		{
			if (ReferenceEquals(left, right))
			{
				return 0;
			}

			if (ReferenceEquals(left, null))
			{
				return -1;
			}

			return left.CompareTo(right);
		}
	}
}
