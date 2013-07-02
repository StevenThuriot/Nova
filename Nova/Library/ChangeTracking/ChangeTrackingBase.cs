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

using System.ComponentModel;

namespace Nova.Library.ChangeTracking
{
    /// <summary>
    /// Base class to simplify change tracking.
    /// </summary>
    public abstract class ChangeTrackingBase : NotifyPropertyChanged, IChangeTracking
    {
        private bool _isChangeTracking;
        private bool _isChanged;
        private ExtendedChangeTrackingHelper<object> _helper;
        
        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            IsChanged = false;
            _helper.AcceptChanges(this);

            if (!_isChangeTracking) return;

            //Attach property again since change tracking is still turned on.
            PropertyChanged += ViewModelPropertyChanged;
        }

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <remarks>This will always return false if change tracking has been disabled.</remarks>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChanged
        {
            get { return _isChangeTracking && (_isChanged || _helper.IsChanged(this)); }
            private set { _isChanged = value; }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ChangeTrackingScope.IsPaused) return;

            _isChanged = true;
            PropertyChanged -= ViewModelPropertyChanged;
        }

        /// <summary>
        /// Initializes the delegates that are used to check and accept changes for properties.
        /// </summary>
        public void PrepareChangeTracking()
        {
            if (_helper == null)
                _helper = ChangeTrackingFactory.CreateExtendedBoxedHelper(this);
        }

        /// <summary>
        /// Initializes the change tracking.
        /// </summary>
        public void InitializeChangeTracking()
        {
            if (_isChangeTracking) return;

            _isChangeTracking = true;

            PrepareChangeTracking(); //If needed.
            
            PropertyChanged += ViewModelPropertyChanged;
        }

        /// <summary>
        /// Stops the change tracking.
        /// </summary>
        public void StopChangeTracking()
        {
            _isChangeTracking = false;
            PropertyChanged -= ViewModelPropertyChanged;

            _helper.StopChangeTracking(this);
        }
    }
}
