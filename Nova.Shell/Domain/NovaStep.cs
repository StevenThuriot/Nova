using System;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Library;
using Nova.Shell.Views;

namespace Nova.Shell.Domain
{
    /// <summary>
    /// Step
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    internal class NovaStep<TView, TViewModel> : NovaStep
        where TView : class, IView, new()
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaStep{TView,TViewModel}" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="group">The group.</param>
        /// <param name="id">The id.</param>
        /// <param name="parameters">The parameters.</param>
        public NovaStep(string title, Guid @group, Guid id, params ActionContextEntry[] parameters)
            : base(title, @group, id, typeof(TView), typeof(TViewModel), parameters)
        {
        }

        /// <summary>
        /// Gets the or create view.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        internal override IView GetOrCreateView(MultiStepView parent)
        {
            if (View != null)
                return View;

            return View = parent.CreateStep<TView, TViewModel>(this);
        }
    }

    /// <summary>
    /// Step
    /// </summary>
    internal abstract class NovaStep : INovaStep
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public Guid Group { get; private set; }

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
        public ActionContextEntry[] Parameters { get; private set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public IView View { get; protected set; }

        /// <summary>
        /// Gets the node ID.
        /// </summary>
        /// <value>
        /// The node ID.
        /// </value>
        public Guid NodeId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is view initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is view initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewInitialized
        {
            get { return View != null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NovaStep" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="group">The group.</param>
        /// <param name="id">The id.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameters">The parameters.</param>
        protected NovaStep(string title, Guid @group, Guid id, Type viewType, Type viewModelType, params ActionContextEntry[] parameters)
        {
            NodeId = id;
            Title = title;
            Group = @group;
            ViewType = viewType;
            ViewModelType = viewModelType;
            Parameters = parameters;
        }

        internal abstract IView GetOrCreateView(MultiStepView parent);
    }
}
