using System;
using System.Collections.Generic;
using System.Linq;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Builders
{
    /// <summary>
    /// Wizard builder
    /// </summary>
    internal class WizardBuilder : IWizardBuilder
    {
        private static readonly ExtendedSize _defaultSize = new ExtendedSize(640, 480, minWidth: 640, minHeight: 480);
        private readonly List<WizardStepBuilder> _steps;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public ExtendedSize Size { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardBuilder" /> class.
        /// </summary>
        public WizardBuilder()
        {
            Size = _defaultSize;
            _steps = new List<WizardStepBuilder>();
        }

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        public IWizardBuilder AddStep<TPageView, TPageViewModel>(string title = null, params ActionContextEntry[] parameters) 
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
            where TPageViewModel : WizardContentViewModel<TPageView, TPageViewModel>, new()
        {

            if (string.IsNullOrWhiteSpace(title))
                title = typeof(TPageView).Name;

            var step = new WizardStepBuilder<TPageView, TPageViewModel>(title, parameters);
            _steps.Add(step);

            return this;
        }

        /// <summary>
        /// Builds the steps.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public IEnumerable<NovaStep> Build(Guid id)
        {
            var steps = _steps.Select(x => x.Build(id)).ToList();
            return steps.AsReadOnly();
        }
    }
}
