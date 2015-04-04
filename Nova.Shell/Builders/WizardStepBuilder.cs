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
    /// Wizard Step Builder
    /// </summary>
    internal class WizardStepBuilder<TView, TViewModel> : WizardStepBuilder
        where TView : ExtendedContentControl<TView, TViewModel>, new() 
        where TViewModel : WizardContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStepBuilder{TView,TViewModel}" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parameters">The parameters.</param>
        public WizardStepBuilder(string title, params ActionContextEntry[] parameters)
            : base(title, typeof(TView), typeof(TViewModel), parameters)
        {
        }


        /// <summary>
        /// Builds the step.
        /// </summary>
        /// <param name="groupId">The group Id.</param>
        /// <returns></returns>
        public override NovaStep Build(Guid groupId)
        {
            return new NovaStep<TView, TViewModel>(Title, groupId, Guid.NewGuid(), Parameters.ToArray());
        }
    }

    /// <summary>
    /// Wizard step builder base.
    /// </summary>
    internal abstract class WizardStepBuilder
    {
        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>
        /// The type of the view.
        /// </value>
        public Type ViewType { get; private set; }
        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IEnumerable<ActionContextEntry> Parameters { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStepBuilder" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameters">The parameters.</param>
        protected WizardStepBuilder(string title, Type viewType, Type viewModelType, params ActionContextEntry[] parameters)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = viewType.Name;

            Title = title;
            ViewType = viewType;
            ViewModelType = viewModelType;
            Parameters = parameters;
        }

        /// <summary>
        /// Builds the step.
        /// </summary>
        /// <param name="id">The groupId.</param>
        /// <returns></returns>
        public abstract NovaStep Build(Guid id);
    }
}
