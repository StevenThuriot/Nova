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

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nova.Library
{
    /// <summary>
    /// Class used when trying to insert something into the ActionContext.
    /// </summary>
    public class ActionContextEntry
    {
        private readonly string _key;
        private readonly object _value;

        /// <summary>
        /// Creates a new ActionContext entry.
        /// </summary>
        /// <param name="key">The key to use for inserting the value into the ActionContext.</param>
        /// <param name="value">the value to insert.</param>
        /// <param name="cloneValue">True if the value should be cloned before inserting.</param>
        /// <returns>An ActionContext Entry</returns>
        public static ActionContextEntry Create<T>(string key, T value, bool cloneValue = true)
        {
            if (typeof(T).IsValueType)
                cloneValue = false;

            return new ActionContextEntry(key, value, cloneValue);
        }

        /// <summary>
        /// Creates a new ActionContext entry. This method uses the type as key.
        /// </summary>
        /// <param name="value">the value to insert.</param>
        /// <param name="cloneValue">True if the value should be cloned before inserting.</param>
        /// <returns>An ActionContext Entry</returns>
        public static ActionContextEntry Create<T>(T value, bool cloneValue = true)
        {
            var key = typeof(T).FullName;
            return Create(key, value, cloneValue);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ActionContextEntry" /> class from being created.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cloneValue">if set to <c>true</c> [clone value].</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        private ActionContextEntry(string key, object value, bool cloneValue)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            
            _key = key;
            _value = value == null
                         ? null
                         : cloneValue
                               ? BruteClone(value)
                               : value;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Clones an ICloneable object and casts it to the type you specified.
        /// </summary>
        /// <param name="value">The instance to clone.</param>
        /// <typeparam name="T">The type to cast the clone to.</typeparam>
        /// <returns>A clone of the specified instance, casted to the wanted type.</returns>
        private static T CloneAs<T>(ICloneable value)
        {
            return (T)value.Clone();
        }

        /// <summary>
        /// Clones an instance of type T. 
        /// If ICloneable is implemented, the implemented Clone() method will be called.
        /// If not, a deep clone (by using serialization) will be made.
        /// </summary>
        /// <typeparam name="T">The type of object being cloned.</typeparam>
        /// <param name="value">The object instance to clone.</param>
        /// <exception cref="ArgumentException">Throws an exception in case the type is not serializable and ICloneable is not implemented.</exception>
        /// <returns>The cloned object.</returns>
        private static T BruteClone<T>(T value)
        {
            var cloneable = value as ICloneable;
            return cloneable != null ? CloneAs<T>(cloneable) : DeepClone(value);
        }

        /// <summary>
        /// Perform a deep clone of the passed instance.
        /// </summary>
        /// <typeparam name="T">The type of object being cloned.</typeparam>
        /// <param name="value">The object instance to clone.</param>
        /// <exception cref="ArgumentException">Throws an exception in case the type is not serializable.</exception>
        /// <returns>The cloned object.</returns>
        /// <exception cref="ArgumentException">The type must be serializable.</exception>
        private static T DeepClone<T>(T value)
        {
            if (ReferenceEquals(value, null))
            {
                return default(T);
            }

            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException(@"The type must be serializable.", "value");
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
