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

using Nova.Controls;
using Nova.Library;
using Nova.Threading.Metadata;

namespace Nova.Shell.Library.Actions.Multistep
{
    /// <summary>
    /// Action that triggers when a certain step is left that is part of a multistep.
    /// </summary>
    /// <remarks>Similar to a normal leave action but doesn't dispose the view yet. 
    /// This will be handled by the multistep itself.</remarks>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Terminating, Alias(Nova.Library.Actions.Aliases.Leave)]
    public class LeaveMultistepAction<TView, TViewModel> : Actionflow<TView, TViewModel>
        where TView : IView
        where TViewModel : IViewModel
    {
        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
        public sealed override bool Execute()
        {
            return base.Execute() && Leave();
        }

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
        public sealed override void ExecuteCompleted()
        {
            LeaveCompleted();
        }

        /// <summary>
        /// Called when leaving a step.
        /// </summary>
        /// <returns></returns>
        public virtual bool Leave()
        {
            return true;
        }

        /// <summary>
        /// Called when leaving the step has completed.
        /// </summary>
        public virtual void LeaveCompleted()
        {
        }
    }
}