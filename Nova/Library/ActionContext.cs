using Nova.Library.ActionMethodRepository;

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

namespace Nova.Library
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
        /// Gets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
	    public string ActionName { get; internal set; }

        /// <summary>
        /// Default Ctor.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
	    private ActionContext(string actionName)
		{
            _Context = new Dictionary<string, object>();
            ActionName = actionName;
		}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <typeparam name="T">Type of the action</typeparam>
        /// <param name="entries">The entries.</param>
        /// <returns></returns>
        internal static ActionContext New<T>(params ActionContextEntry[] entries)
        {
            var actionName = MethodCacheEntry.GetActionName(typeof(T));
            return new ActionContext(actionName).AddRange(entries);
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
		public ActionContext AddRange(params ActionContextEntry[] entries)
		{
            if (entries == null || entries.Length == 0) return this;
            
            foreach (var entry in entries)
            {
                Add(entry);
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
            return GetValue<T>(key);
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
        /// This method uses the requested type as the key.
		/// </summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="value">The requested value.</param>
		/// <returns>True if the value is found.</returns>
		public bool TryGetValue<T>(out T value)
        {
            var key = typeof(T).FullName;
		    return TryGetValue(key, out value);
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
        
        /// <summary>
        /// Determines whether the <see cref="ActionContext"/> contains the specified key.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="ActionContext"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="ActionContext"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key)
        {
            return _Context.ContainsKey(key);
        }
    }
}