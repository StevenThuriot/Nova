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

namespace Nova.Library
{
    /// <summary>
    /// Attribute to give actions an alias.
    /// </summary>
    /// <remarks>Useful for OnBefore/After logic.
    /// e.g. OnAfterEnter when the action is not called Enter, 
    /// or for reusing methods for multiple actions.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasAttribute" /> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        public AliasAttribute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new Exception("An alias can not be null, empty or whitespace.");

            Alias = alias.Replace(" ", "");
        }
    }
}
