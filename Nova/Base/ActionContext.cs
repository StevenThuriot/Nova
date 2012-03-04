using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
		/// A clone will be made, if needed. 
		/// If the instance does not implement ICloneable, a clone will be made through serializing. (and thus the Serializable attribute is needed!)
		/// </summary>
		/// <typeparam name="T">The type of object being cloned.</typeparam>
		/// <param name="value">The object instance to clone.</param>
		/// <param name="key">The key to add.</param>
		/// <exception cref="ArgumentException">Throws an exception in case the type is not serializable and ICloneable is not implemented.</exception>
		/// <returns>Itself.</returns>
		public ActionContext Add<T>(string key, T value)
		{
			object valueToAdd = typeof (T).IsValueType
			                    	? value
			                    	: BruteClone(value);

			_Context.Add(key, valueToAdd);

			return this;
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
