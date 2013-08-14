﻿#region License

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
using Nova.Controls;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Builders
{
    /// <summary>
    /// Module builder
    /// </summary>
    internal class ModuleBuilder : IModuleBuilder
    {
        public const int DefaultRanking = 10;

        private TreeNodeBase _startupTreeNode;
        private readonly List<TreeNodeBase> _treeNodes = new List<TreeNodeBase>();
        private int? _ranking;
        private string _title;

        /// <summary>
        /// Gets the ranking.
        /// </summary>
        /// <value>
        /// The ranking.
        /// </value>
        public int Ranking
        {
            get { return _ranking ?? DefaultRanking; }
        }

        /// <summary>
        /// Sets the module title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">A title has already been set and can only be set once.</exception>
        /// <exception cref="System.ArgumentNullException">title</exception>
        public IModuleBuilder SetModuleTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(_title))
                throw new NotSupportedException("A title has already been set and can only be set once.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title");

            _title = title;

            return this;
        }

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title of the node. Default value is the type name.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <returns>The module builder instance.</returns>
        public IModuleBuilder AddNavigation<TPageView, TPageViewModel>(string title = null, int rank = 10)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            //TODO: Allow parameter (so one screen can be used for several purposed depending on the parameter, e.g full/light)
            
            var treeNode = TreeNode.New<TPageView, TPageViewModel>(title, rank);
            _treeNodes.Add(treeNode);

            return this;
        }

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <param name="builder">The multi step builder.</param>
        /// <param name="title">The title of the node.</param>
        /// <param name="rank">The ranking in the navigational tree. Default value is 10.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public IModuleBuilder AddNavigation(string title, Action<IMultiStepBuilder> builder, int rank = 10)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            var multiStepBuilder = new MultiStepBuilder(title, rank);
            builder(multiStepBuilder);

            var treeNode = multiStepBuilder.Build();
            _treeNodes.Add(treeNode);
            
            return this;
        }

        /// <summary>
        /// Marks the previously added navigational action as the startup page.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">A default use case has already been set and can only be set once.</exception>
        /// <remarks>
        /// Only allowed to be used once per module.
        /// </remarks>
        public IModuleBuilder AsStartup()
        {
            if (_startupTreeNode != null)
                throw new NotSupportedException("A default use case has already been set and can only be set once.");

            if (_treeNodes.Count == 0)
                throw new NotSupportedException("A use case has to be added before the start up use case can be set.");
            
            _startupTreeNode = _treeNodes.Last();
            return this;
        }

        /// <summary>
        /// Sets the module ranking.
        /// Used to determine startup module when there are multiple independant modules. (Highest ranking wins)
        /// </summary>
        /// <param name="ranking">The ranking.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Ranking can only be set once.</exception>
        /// <remarks>
        /// The ranking can only be set once. Default value is 10.
        /// </remarks>
        public IModuleBuilder SetModuleRanking(int ranking)
        {
            if (_ranking.HasValue)
                throw new NotSupportedException("Ranking can only be set once.");

            _ranking = ranking;

            return this;
        }

        /// <summary>
        /// Builds a module.
        /// </summary>
        /// <returns></returns>
        internal NovaModule Build()
        {
            return new NovaModule(_title, Ranking, _treeNodes, _startupTreeNode);
        }
    }
}
