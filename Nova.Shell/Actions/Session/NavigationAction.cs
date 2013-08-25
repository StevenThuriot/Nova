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
            return ViewModel.CurrentView == null || !ViewModel.CurrentView.IsLoading;
        }

        public override void OnBefore()
        {
            _current = ActionContext.GetValue<IView>(SessionViewModel.CurrentViewConstant);
        }

        public override bool Execute()
        {
            if (_current != null)
            {
                var canLeave = _current.ViewModel.Leave().Result;

                if (!canLeave) return false;
            }

            InitializeNextView();

            var result = _nextView != null && _nextView.ViewModel.Enter().Result;

            if (result) return true;

            CreateNotAvailableView();

            return true;
        }

        private void CreateNotAvailableView()
        {
            StepNotAvailableView notAvailableView = null;
            
            Dispatch(() =>
            {
                notAvailableView = ViewModel.CreateControl<StepNotAvailableView, StepNotAvailableViewModel>();
                notAvailableView.ViewModel.StepName = View._NovaTree.FindTitle(_viewType, _viewModelType) ?? _viewType.Name;
            });

            _nextView = notAvailableView;
        }

        private void InitializeNextView()
        {
            _viewType = ActionContext.GetValue<Type>(SessionViewModel.ViewTypeConstant);
            _viewModelType = ActionContext.GetValue<Type>(SessionViewModel.ViewModelTypeConstant);
            
            NovaTreeNodeStep novaTreeNodeStep;
            var navigatingToMultistep = ActionContext.TryGetValue(RoutedAction.CommandParameter, out novaTreeNodeStep) && novaTreeNodeStep != null;

            if (navigatingToMultistep)
                CreateMultistep(novaTreeNodeStep);
            else
                CreateNormalView();
        }

        private void CreateMultistep(NovaTreeNodeStep novaTreeNodeStep)
        {
            var multistep = _current as MultiStepView;
            if (multistep != null)
            {
                _navigatingInsideMultiStep = Dispatch(() => multistep.GetOrCreateStep(novaTreeNodeStep, out _nextView));
            }

            if (_navigatingInsideMultiStep) return;

            var groupId = novaTreeNodeStep.GroupId;
            var steps = novaTreeNodeStep.Parent.Steps.Select(x => x.ConvertToNovaStep());

            var list = new LinkedList<NovaStep>(steps);
            var initialView = GetInitialView(novaTreeNodeStep, list);


            _nextView = Dispatch(() => new MultiStepView(View, ViewModel, groupId, initialView));
        }

        private void CreateNormalView()
        {
            var createNextView = ActionContext.GetValue<Func<IView>>(SessionViewModel.CreateNextViewConstant);
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
                if (node.Value.ViewType == treeNode.PageType && node.Value.ViewModelType == treeNode.ViewModelType)
                    return node;

                node = node.Next;
            }

            return list.First;
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

            View._NovaTree.ReevaluateState(_viewType, _viewModelType);
        }

        protected override void DisposeManagedResources()
        {
            _current = null;
            _nextView = null;
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
