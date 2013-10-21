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

namespace Nova.Shell.Library
{
    /// <summary>
    /// Stack Info
    /// </summary>
    internal class StackInfo
    {
        /// <summary>
        /// Gets the stack id.
        /// </summary>
        /// <value>
        /// The stack id.
        /// </value>
        public Guid StackId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackInfo"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        public StackInfo(Guid stackId)
        {
            StackId = stackId;
        }
    }
}