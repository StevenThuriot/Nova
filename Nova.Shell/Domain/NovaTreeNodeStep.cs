using System;
using System.Windows.Input;
using Nova.Controls;
using Nova.Shell.Library;

namespace Nova.Shell.Domain
{
    public class NovaTreeNodeStep<TView, TViewModel> : NovaTreeNodeStep
        where TView : class, IView, new()
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNodeStep{TView,TViewModel}" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="command">The command.</param>
        public NovaTreeNodeStep(Guid id, string title, ICommand command)
            : base(id, title, typeof(TView), typeof(TViewModel), command)
        {
        }

        internal override NovaStep ConvertToNovaStep()
        {
            return new NovaStep<TView, TViewModel>(Title, GroupId, Id);
        }
    }

    /// <summary>
    /// Nova Tree Node Step
    /// </summary>
    public abstract class NovaTreeNodeStep : NovaTreeNode, IEquatable<NovaTreeNodeStep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NovaTreeNodeStep" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="command">The command.</param>
        protected NovaTreeNodeStep(Guid id, string title, Type viewType, Type viewModelType, ICommand command)
            : base(id , title, viewType, viewModelType, command, false)
        {
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public NovaMultiStepTreeNode Parent { get; set; }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        /// <value>
        /// The group id.
        /// </value>
        public Guid GroupId { get { return Parent.GroupId; } }

        public bool Equals(NovaTreeNodeStep other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && GroupId.Equals(other.GroupId);
        }

        protected override bool CheckIfCurrent(Type pageType, Type viewModelType)
        {
            return PageType == pageType && ViewModelType == viewModelType;
        }

        internal abstract NovaStep ConvertToNovaStep();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NovaTreeNodeStep) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ GroupId.GetHashCode();
            }
        }

        public static bool operator ==(NovaTreeNodeStep left, NovaTreeNodeStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NovaTreeNodeStep left, NovaTreeNodeStep right)
        {
            return !Equals(left, right);
        }
    }
}
