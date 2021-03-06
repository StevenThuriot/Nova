﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Multi step treenode.
    /// </summary>
    public class NovaMultiStepTreeNode : NovaTreeNodeBase
    {
        private readonly Lazy<NovaTreeNodeStep> _firstStep;

        /// <summary>
        /// Gets the group id.
        /// </summary>
        /// <value>
        /// The group id.
        /// </value>
        public Guid GroupId { get; private set; }

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        public IEnumerable<NovaTreeNodeStep> Steps { get; private set; }

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <value>
        /// The first step.
        /// </value>
        public NovaTreeNodeStep FirstStep
        {
            get { return _firstStep.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaMultiStepTreeNode" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="steps">The steps.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="isStartupNode">if set to <c>true</c> [is startup node].</param>
        /// <exception cref="System.ArgumentNullException">title</exception>
        /// <exception cref="System.NotSupportedException">steps cannot be empty.</exception>
        public NovaMultiStepTreeNode(string title, IEnumerable<NovaTreeNodeStep> steps, Guid groupId, bool isStartupNode) 
            : base(title, isStartupNode)
        {
            if (steps == null)
                throw new ArgumentNullException("steps");
            
            var readonlySteps = steps.ToList().AsReadOnly();
            
            if (readonlySteps.Count == 0)
                throw new NotSupportedException("steps cannot be empty.");

            foreach (var step in readonlySteps)
                step.Parent = this;

            Steps = readonlySteps;
            GroupId = groupId;
            _firstStep = new Lazy<NovaTreeNodeStep>(readonlySteps.First);
        }

        protected override bool CheckIfCurrent(Type pageType, Type viewModelType)
        {
            return Steps.Select(step => step.ReevaluateState(pageType, viewModelType))
                        .Aggregate(false, (current, result) => current || result);
        }

        public override void Navigate()
        {
            FirstStep.Navigate();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        /// <summary>
        /// Finds the step.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public NovaTreeNodeStep FindStep(Guid key)
        {
            return Steps.FirstOrDefault(x => x.Id == key);
        }
    }
}
