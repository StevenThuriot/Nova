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
    /// Multistep
    /// </summary>
    internal interface IMultiStep
    {
        /// <summary>
        /// Determines whether this instance can go to next step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to next step]; otherwise, <c>false</c>.
        /// </returns>
        bool CanGoToNextStep();

        /// <summary>
        /// Determines whether this instance can go to previous step.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can go to previous step]; otherwise, <c>false</c>.
        /// </returns>
        bool CanGoToPreviousStep();

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Does the step.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        bool DoStep(Guid id);

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        void Finish();
    }
}
