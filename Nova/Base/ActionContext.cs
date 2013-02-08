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
using System.Collections.Generic;

namespace Nova.Base
{
	/// <summary>
	/// Context used to pass data to actions and back.
	/// </summary>
	public class ActionContext
	{
        private readonly Dictionary<string, object> _Context;

		/// <summary>
		/// Gets a value indicating whether the execution of the action was successful.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is successful; otherwise, <c>false</c>.
		/// </value>
	    public bool IsSuccessful { get; internal set; }

		/// <summary>
		/// Default Ctor.
		/// </summary>
	    internal ActionContext()
		{
			_Context = new Dictionary<string, object>();
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			_Context.Clear();
		}

        /// <summary>
        /// Adds the passed value into the action context.
        /// If the instance does not implement ICloneable, a clone will be made through serializing. (and thus the Serializable attribute is needed!)
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// Itself.
        /// </returns>
        /// <exception cref="ArgumentException">Throws an exception in case the type is not serializable and ICloneable is not implemented.</exception>
		public ActionContext Add(ActionContextEntry entry)
		{
			_Context.Add(entry.Key, entry.Value);

			return this;
		}

        /// <summary>
        /// Adds the passed values into the action context.
        /// If the instance does not implement ICloneable, a clone will be made through serializing. (and thus the Serializable attribute is needed!)
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>
        /// Itself.
        /// </returns>
        /// <exception cref="ArgumentException">Throws an exception in case the type is not serializable and ICloneable is not implemented.</exception>
		public ActionContext AddRange(IEnumerable<ActionContextEntry> entries)
		{
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    Add(entry);
                }
            }

            return this;
		}

        /// <summary>
        /// Gets a value from the actioncontext.
        /// This method uses the requested type as the key.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>
        /// The requested value.
        /// </returns>
		public T GetValue<T>()
        {
            var key = typeof (T).FullName;
			return (T)_Context[key];
		}

		/// <summary>
		/// Gets a value from the actioncontext.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="key">The key linked to the value.</param>
		/// <returns>The requested value.</returns>
		public T GetValue<T>(string key)
		{
			return (T)_Context[key];
		}

		/// <summary>
		/// Tries to get a value from the actioncontext.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="key">The key linked to the value.</param>
		/// <param name="value">The requested value.</param>
		/// <returns>True if the value is found.</returns>
		public bool TryGetValue<T>(string key, out T value)
		{
			object resultValue;

			var result = _Context.TryGetValue(key, out resultValue);

			if (result)
			{
				value = (T) resultValue;
			}
			else
			{
				value = default(T);
			}

			return result;
		}
    }
}
