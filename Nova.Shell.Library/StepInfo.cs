#region License
//   
//  Copyright 2013 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//    http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//   
#endregion

using System;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Step Info
    /// </summary>
    public class StepInfo
    {
        public StepInfo(string title, Type viewType, Type viewModelType, Guid nodeId)
        {
            Title = title;
            ViewType = viewType;
            ViewModelType = viewModelType;
            NodeId = nodeId;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>
        /// The type of the view.
        /// </value>
        public Type ViewType { get; private set; }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        public Type ViewModelType { get; private set; }
        
        /// <summary>
        /// Gets the node ID.
        /// </summary>
        /// <value>
        /// The node ID.
        /// </value>
        public Guid NodeId { get; private set; }
    }
}
