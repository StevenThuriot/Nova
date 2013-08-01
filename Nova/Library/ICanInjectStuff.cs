using Nova.Controls;

namespace Nova.Library
{
    /// <summary>
    /// Interface to help initialize views.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    internal interface ICanInjectStuff<TView, in TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : class, IView
    {
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void Inject(IView parent, TViewModel viewModel);
    }
}
