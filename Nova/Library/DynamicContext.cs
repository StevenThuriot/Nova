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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Dynamic;

namespace Nova.Library
{
	/// <summary>
	/// Dynamic Context used for Serialization of the ViewModel.
	/// </summary>
	[Serializable]
	internal class DynamicContext : DynamicObject, ISerializable, ICloneable
	{
		private readonly Dictionary<string, object> _DynamicContext = new Dictionary<string, object>();

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
		/// </value>
		public bool IsEmpty { get { return _DynamicContext.Count == 0; }}

		/// <summary>
		/// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
		/// </summary>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
		/// </returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return (_DynamicContext.TryGetValue(binder.Name, out result));
		}

		/// <summary>
		/// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
		/// </summary>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
		/// </returns>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_DynamicContext.Add(binder.Name, value);
			return true;
		}

		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, object> kvp in _DynamicContext)
			{
				info.AddValue(kvp.Key, kvp.Value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicContext"/> class.
		/// </summary>
		public DynamicContext()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicContext"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected DynamicContext(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry entry in info)
			{
				_DynamicContext.Add(entry.Name, entry.Value);
			}
		}

		/// <summary>
		/// Gets the name of the serialization file for the passed type. 
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The serialized filename.</returns>
		private static string GetSerializationName(Type type)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var filename = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) ?? AppDomain.CurrentDomain.FriendlyName;

			var directory = Path.Combine(path, filename);

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			return Path.Combine(directory, type.Name + ".Nova");
		}

		/// <summary>
		/// Saves the specified object.
		/// </summary>
		/// <typeparam name="T">The type of class this object is saved on. Used to derive a filename.</typeparam>
		/// <param name="objectToSave">The object to save.</param>
		public static void Save<T>(DynamicContext objectToSave)
		{
		    Task.Run(() =>
		        {
		            var type = GetSerializationName(typeof (T));
		            using (var file = File.Create(type))
		            {
		                var formatter = new BinaryFormatter();
		                formatter.Serialize(file, objectToSave);
		            }
		        });
		}

		/// <summary>
		/// Loads this instance.
		/// </summary>
		/// <typeparam name="T">The type of class this object is saved on. Used to derive a filename.</typeparam>
		/// <returns>The deserialized instance.</returns>
		public static Task<DynamicContext> Load<T>()
		{
		    return Task.Run(() =>
		        {
		            var fileName = GetSerializationName(typeof (T));

		            if (!File.Exists(fileName)) return new DynamicContext();

		            using (var file = File.OpenRead(fileName))
		            {
		                var formatter = new BinaryFormatter();
		                var deserializedContext = formatter.Deserialize(file);
                        
		                return (DynamicContext) deserializedContext;
		            }
		        });
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			var clonedContext = new DynamicContext();

			foreach (var value in _DynamicContext)
			{
				clonedContext._DynamicContext.Add(value.Key, value.Value);
			}

			return clonedContext;
		}
	}

}
