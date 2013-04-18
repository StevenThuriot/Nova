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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nova.Base.ActionMethodRepository
{
    /// <summary>
    /// A method cache entry
    /// </summary>
    internal abstract class MethodCacheEntry
    {
        protected List<OnAction> BeforeMethodInfos;
        protected List<OnAction> AfterMethodInfos;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the relevant on before methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<OnAction> GetRelevantOnBeforeMethods(Type type)
        {
            return GetRelevantMethods(type, "ONBEFORE", BeforeMethodInfos);
        }

        /// <summary>
        /// Gets the relevant on after methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<OnAction> GetRelevantOnAfterMethods(Type type)
        {
            return GetRelevantMethods(type, "ONAFTER", AfterMethodInfos);
        }


        /// <summary>
        /// Gets the relevant methods.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        private static IEnumerable<OnAction> GetRelevantMethods(Type actionType, string prefix, List<OnAction> methods)
        {
            //Optimalization
            if (methods == null || methods.Count == 0)
                return Enumerable.Empty<OnAction>();

            var typeName = actionType.Name;
            var generic = typeName.IndexOf('`'); //Don't include generics in the naming
            if (generic > 0)
            {
                typeName = typeName.Substring(0, generic);
            }

            if (typeName.EndsWith("ACTION", StringComparison.OrdinalIgnoreCase))
            {
                typeName = typeName.Substring(0, typeName.Length - 6);
            }

            var name = prefix + typeName.ToString(CultureInfo.InvariantCulture);
            return methods.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// A cache entry with the onbefore/after methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MethodCacheEntry<T> : MethodCacheEntry
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public override Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCacheEntry{T}" /> class.
        /// </summary>
        public MethodCacheEntry()
        {
            var type = typeof(T);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            BeforeMethodInfos = CreateMethods(methods, "ONBEFORE");
            AfterMethodInfos = CreateMethods(methods, "ONAFTER");
        }

        /// <summary>
        /// Creates the methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="startsWith">The starts with.</param>
        /// <returns></returns>
        private static List<OnAction> CreateMethods(IEnumerable<MethodInfo> methods, string startsWith)
        {
            return methods.Where(x => x.Name.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                          .Select(OnAction.Create<T>)
                          .Where(x => x != null)
                          .ToList();
        }
    }
}
