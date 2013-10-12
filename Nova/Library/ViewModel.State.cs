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

namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void SaveState()
        {
            var objectToSave = new DynamicContext();
            SaveState(objectToSave);

            if (!objectToSave.IsEmpty)
            {
                DynamicContext.Save<TViewModel>(objectToSave);
            }
        }

        /// <summary>
        /// Loads this instance.
        /// LoadState(value) will only trigger in case the ViewModel has been saved before.
        /// </summary>
        public async void LoadState()
        {
            var dynamicContext = await DynamicContext.Load<TViewModel>();

            if (dynamicContext.IsEmpty) return;

            LoadState(dynamicContext);
        }

        /// <summary>
        /// You can use this method to specify what exactly you want to save.
        /// Add everything to the object to prepare it for serialization. 
        /// All the parameters added should be serializable.
        /// </summary>
        /// <param name="value">The object to save.</param>
        protected virtual void SaveState(dynamic value) { }

        /// <summary>
        /// You can use this method to specify what exactly you want to load.
        /// Retreive everything to the object and load it back onto your ViewModel.
        /// </summary>
        /// <param name="value">The object to load.</param>
        protected virtual void LoadState(dynamic value) { }
    }
}
