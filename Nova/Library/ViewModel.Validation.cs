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

using Nova.Validation;

namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private bool _isValid = true;
        private ReadOnlyErrorCollection _errorCollection;

        /// <summary>
        /// Gets the error collection.
        /// </summary>
        public ReadOnlyErrorCollection ErrorCollection
        {
            get { return _errorCollection; }
            internal set
            {
                if (SetValue(ref _errorCollection, value))
                {
                    IsValid = _errorCollection == null || _errorCollection.Count == 0;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return _isValid; }
            private set { SetValue(ref _isValid, value); }
        }
    }
}
