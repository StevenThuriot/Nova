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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Nova.Library.ActionMethodRepository
{
    /// <summary>
    /// A method cache entry
    /// </summary>
    internal abstract class MethodCacheEntry
    {
        private readonly Dictionary<Type, IEnumerable<string>> _TypeAliases;

        public const string OnBefore = "ONBEFORE";
        public const string OnAfter = "ONAFTER";

        protected List<OnAction> BeforeActions;
        protected List<OnAction> AfterActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCacheEntry" /> class.
        /// </summary>
        protected MethodCacheEntry()
        {
            _TypeAliases = new Dictionary<Type, IEnumerable<string>>();
        }

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
            return GetRelevantMethods(type, OnBefore, BeforeActions);
        }

        /// <summary>
        /// Gets the relevant on after methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<OnAction> GetRelevantOnAfterMethods(Type type)
        {
            return GetRelevantMethods(type, OnAfter, AfterActions);
        }


        /// <summary>
        /// Gets the relevant methods.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        private IEnumerable<OnAction> GetRelevantMethods(Type actionType, string prefix, IReadOnlyCollection<OnAction> methods)
        {
            //Optimalization
            if (methods.Count == 0)
                return Enumerable.Empty<OnAction>();

            var aliases = GetAliases(actionType)
                                .Select(x => prefix + x)
                                .ToList();

            return methods.Where(x => aliases.Any(name => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)) || prefix.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the aliases of the action.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <returns></returns>
        private IEnumerable<string> GetAliases(Type actionType)
        {
            IEnumerable<string> cachedAliases;
            if (_TypeAliases.TryGetValue(actionType, out cachedAliases))
            {
                return cachedAliases;
            }

            var aliases = actionType.GetCustomAttributes<AliasAttribute>()
                                .Select(x => GetActionName(x.Alias))
                                .ToList();

            var typeName = GetActionName(actionType);
            aliases.Add(typeName);

            _TypeAliases.Add(actionType, aliases.AsReadOnly());

            return aliases;
        }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        internal static string GetActionName(string actionName)
        {
            var generic = actionName.IndexOf('`'); //Don't include generics in the naming
            if (generic > 0)
            {
                actionName = actionName.Substring(0, generic);
            }

            if (actionName.EndsWith("ACTION", StringComparison.OrdinalIgnoreCase))
            {
                actionName = actionName.Substring(0, actionName.Length - 6);
            }
            return actionName;
        }

        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <returns></returns>
        internal static string GetActionName(Type actionType)
        {
            return GetActionName(actionType.Name);
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
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod);

            BeforeActions = CreateActions(methods, OnBefore);
            AfterActions = CreateActions(methods, OnAfter);
        }

        /// <summary>
        /// Creates the methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="startsWith">The starts with.</param>
        /// <returns></returns>
        private static List<OnAction> CreateActions(IEnumerable<MethodInfo> methods, string startsWith)
        {
            return methods.Where(x => x.Name.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                          .Select(OnAction.Create<T>)
                          .Where(x => x != null)
                          .ToList();
        }
    }
}
