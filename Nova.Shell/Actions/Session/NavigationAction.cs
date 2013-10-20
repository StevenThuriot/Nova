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
using System.Linq;
using System.Windows.Threading;
using Nova.Controls;
using Nova.Shell.Domain;
using Nova.Shell.Library;
using Nova.Shell.Views;
using Nova.Threading.Metadata;
using Nova.Library;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// Navigational action
    /// </summary>
    [Blocking]
    public class NavigationAction : Actionflow<SessionView, SessionViewModel>
    {
        private IView _nextView;
        private IView _current;

        private Type _viewType;
        private Type _viewModelType;

        private bool _navigatingInsideMultiStep;

        public override bool CanExecute()
        {
            var sessionViewModel = ViewModel;
            var currentView = sessionViewModel.CurrentView;

            return !sessionViewModel.IsStacked && (currentView == null || !currentView.IsLoading);
        }

        public override void OnBefore()
        {
            _current = ActionContext.GetValue<IView>(ActionContextConstants.CurrentViewConstant);
        }

        public override bool Execute()
        {
            var actionContextEntries = ActionContext.GetEntries().ToList();

            _viewType = ActionContext.GetValue<Type>(ActionContextConstants.ViewTypeConstant);
            _viewModelType = ActionContext.GetValue<Type>(ActionContextConstants.ViewModelTypeConstant);

            var currentlyInsideMultistep = typeof(IMultistepContentViewModel).IsAssignableFrom(_viewModelType);
            var triggerValidation = !currentlyInsideMultistep;

            NovaTreeNodeStep novaTreeNodeStep = null;
            var hasStep = false;
            if (currentlyInsideMultistep)
            {
                Guid key;

                if (!ActionContext.TryGetValue(ActionContextConstants.NodeId, out key))
                    throw new NotSupportedException("Navigating to a multistep view without providing a step key is not supported.");

                novaTreeNodeStep = View._NovaTree.FindStep(key);

                if (novaTreeNodeStep == null)
                    throw new NotSupportedException("Navigating to an unknown step.");

                var multistep = _current as MultiStepView;

                if (multistep != null)
                {
                    hasStep = multistep.HasStep(novaTreeNodeStep);
                    triggerValidation = !hasStep;
                }
            }
            
            var triggerEntry = ActionContextEntry.Create(ActionContextConstants.TriggerValidation, triggerValidation, false);
            actionContextEntries.Add(triggerEntry);

            var entries = actionContextEntries.ToArray();

            return LeavePreviousView(entries) && CreateNextView(currentlyInsideMultistep, novaTreeNodeStep, hasStep, entries);
        }

        public override void ExecuteCompleted()
        {
            if (_navigatingInsideMultiStep)
            {
                ((MultiStepView)ViewModel.CurrentView).DoStep(_nextView);
            }
            else
            {
                ViewModel.CurrentView = _nextView;
            }


            Guid key;
            ActionContext.TryGetValue(ActionContextConstants.NodeId, out key);
            View._NovaTree.ReevaluateState(key, _viewType, _viewModelType);
        }

        protected override void DisposeManagedResources()
        {
            _current = null;
            _nextView = null;
        }








        private bool LeavePreviousView(ActionContextEntry[] entries)
        {
            return _current == null || _current.ViewModel.Leave(entries).Result;
        }

        private bool CreateNextView(bool currentlyInsideMultistep, NovaTreeNodeStep novaTreeNodeStep, bool hasStep,
            ActionContextEntry[] entries)
        {
            if (currentlyInsideMultistep)
                CreateMultistep(novaTreeNodeStep, hasStep);
            else
                CreateNormalView();

            var result = _nextView != null && _nextView.ViewModel.Enter(entries).Result;

            if (result) return true;

            CreateNotAvailableView();

            return true;
        }

        private void CreateMultistep(NovaTreeNodeStep novaTreeNodeStep, bool hasChild)
        {
            var multistep = _current as MultiStepView;
            _navigatingInsideMultiStep = hasChild && multistep != null && Dispatch(() => multistep.GetOrCreateStep(novaTreeNodeStep, out _nextView));

            
            if (_navigatingInsideMultiStep) return;
            //Create new one.
            var steps = novaTreeNodeStep.Parent.Steps.Select(x => x.ConvertToNovaStep());

            var list = new LinkedList<NovaStep>(steps);
            var initialView = GetInitialView(novaTreeNodeStep, list);

            var groupId = novaTreeNodeStep.GroupId;
            _nextView = Dispatch(() => new MultiStepView(View, ViewModel, groupId, initialView));
        }

        private void CreateNormalView()
        {
            var createNextView = ActionContext.GetValue<Func<IView>>(ActionContextConstants.CreateNextViewConstant);
            if (createNextView != null)
            {
                _nextView = Dispatch(createNextView);
            }
        }

        private static LinkedListNode<NovaStep> GetInitialView(NovaTreeNode treeNode, LinkedList<NovaStep> list)
        {
            var node = list.First;

            while (node != null)
            {
                if (node.Value.NodeId == treeNode.Id)
                    return node;

                node = node.Next;
            }

            return list.First;
        }

        private void CreateNotAvailableView()
        {
            StepNotAvailableView notAvailableView = null;

            Dispatch(() =>
            {
                notAvailableView = ViewModel.CreateControl<StepNotAvailableView, StepNotAvailableViewModel>();
                notAvailableView.ViewModel.StepName = /* TODO View._NovaTree.FindTitle(_viewType, _viewModelType) ?? */_viewType.Name;
            });

            _nextView = notAvailableView;
        }

        /// <summary>
        /// Dispatches the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        private void Dispatch(Action action)
        {
            View.Dispatcher.Invoke(action, DispatcherPriority.Send);
        }

        /// <summary>
        /// Dispatches the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        private T Dispatch<T>(Func<T> function)
        {
            return View.Dispatcher.Invoke(function, DispatcherPriority.Send);
        }
    }
}
