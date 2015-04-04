using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Nova.Shell.Domain;

namespace Nova.Shell.Controls
{
    /// <summary>
    /// Nova Tree Interface
    /// </summary>
    internal interface INovaTree
    {
        /// <summary>
        /// Initializes the tree's data.
        /// </summary>
        /// <param name="modules">The modules.</param>
        void InitializeData(IEnumerable<NovaTreeModule> modules);

        /// <summary>
        /// Activates the module.
        /// </summary>
        /// <param name="module">The module.</param>
        void ActivateModule(NovaTreeModule module);
    }

    /// <summary>
    /// Interaction logic for NovaTree.xaml
    /// </summary>
    public partial class NovaTree : INotifyPropertyChanged, INovaTree
    {
        private IEnumerable<NovaTreeNodeBase> _treeNodes;
        private bool _showModules;

        /// <summary>
        /// Gets the tree nodes.
        /// </summary>
        /// <value>
        /// The tree nodes.
        /// </value>
        public IEnumerable<NovaTreeNodeBase> TreeNodes
        {
            get { return _treeNodes; }
            private set
            {
                if (Equals(_treeNodes, value)) return;

                _treeNodes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the modules.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the list of modules should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool ShowModules
        {
            get { return _showModules; }
            set
            {
                if (_showModules == value) return;

                _showModules = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <value>
        /// The modules.
        /// </value>
        public IEnumerable<NovaTreeModule> Modules { get; private set; }

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        public bool ReevaluateState(Guid key, Type pageType, Type viewModelType)
        {
            return ReevaluateState(key) || ReevaluateState(pageType, viewModelType);
        }


        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="key">The key.</param>
        private bool ReevaluateState(Guid key)
        {
            if (key == Guid.Empty)
                return false;

            var novaTreeNodes = TreeNodes;
            if (ReevaluateNodes(novaTreeNodes, key)) return true; //Node found in current module

            //Search TreeNodes in a different module.
            var nodes = Modules.Select(x => x.TreeNodes)
                               .Where(x => x != novaTreeNodes)
                               .FirstOrDefault(x => ReevaluateNodes(x, key));

            if (nodes == null) return false; //Leave the navigational tree intact if the current node was not found.

            TreeNodes = nodes;
            return true;
        }


        /// <summary>
        /// Reevaluates the state.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        private bool ReevaluateState(Type pageType, Type viewModelType)
        {
            var novaTreeNodes = TreeNodes;
            if (ReevaluateNodes(novaTreeNodes, pageType, viewModelType)) return true; //Node found in current module
            
            //Search TreeNodes in a different module.
            var nodes = Modules.Select(x => x.TreeNodes)
                               .Where(x => x != novaTreeNodes)
                               .FirstOrDefault(x => ReevaluateNodes(x, pageType, viewModelType));

            if (nodes == null) return false; //Leave the navigational tree intact if the current node was not found.

            TreeNodes = nodes;
            return true;
        }

        /// <summary>
        /// Reevaluates the nodes.
        /// </summary>
        /// <param name="novaTreeNodes">The nova tree nodes.</param>
        /// <param name="_pageType">Type of the _page.</param>
        /// <param name="_viewModelType">Type of the _view model.</param>
        /// <returns>
        /// True if one of the nodes is the current node.
        /// </returns>
        private static bool ReevaluateNodes(IEnumerable<NovaTreeNodeBase> novaTreeNodes, Type _pageType, Type _viewModelType)
        {
            return novaTreeNodes.Select(node => node.ReevaluateState(_pageType, _viewModelType))
                                .Aggregate(false, (current, result) => current || result);
        }

        /// <summary>
        /// Reevaluates the nodes.
        /// </summary>
        /// <param name="novaTreeNodes">The nova tree nodes.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// True if one of the nodes is the current node.
        /// </returns>
        private static bool ReevaluateNodes(IEnumerable<NovaTreeNodeBase> novaTreeNodes, Guid key)
        {
            return novaTreeNodes.Select(node => node.ReevaluateState(key))
                                .Aggregate(false, (current, result) => current || result);
        }

        /// <summary>
        /// Finds the step.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal NovaTreeNodeStep FindStep(Guid key)
        {
            var nodes = new[] {TreeNodes}
                            .Union(Modules.Select(x => x.TreeNodes))
                            .SelectMany(x => x).OfType<NovaMultiStepTreeNode>();

            return nodes.Select(multiStep => multiStep.FindStep(key))
                        .FirstOrDefault(step => step != null);
        }

        /// <summary>
        /// Navigates to the startup page.
        /// </summary>
        public void NavigateToStartupPage()
        {
            var node = TreeNodes.FirstOrDefault(x => x.IsStartupNode) ?? TreeNodes.First();
            
            node.Navigate();
        }

        /// <summary>
        /// Initializes the tree's data.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <exception cref="System.ArgumentNullException">modules</exception>
        void INovaTree.InitializeData(IEnumerable<NovaTreeModule> modules)
        {
            if (modules == null)
                throw new ArgumentNullException("modules");

            var amountOfModules = modules.Count();

            if (amountOfModules == 0)
                throw new ArgumentException(@"No modules found.", "modules");

            Modules = modules;
            TreeNodes = Modules.First().TreeNodes;
            ShowModules = amountOfModules > 1;

            InitializeComponent();
        }


        /// <summary>
        /// Activates the module.
        /// </summary>
        /// <param name="module">The module.</param>
        void INovaTree.ActivateModule(NovaTreeModule module)
        {
            TreeNodes = module.TreeNodes;
        }
    }
}
