using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Nova.Library.Actions;
using Nova.Shell.Domain;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// The session enter action
    /// </summary>
    public class SessionEnterAction : EnterAction<SessionView, SessionViewModel>
    {
        private IEnumerable<NovaTreeNode> _Nodes;

        public override bool Enter()
        {
            var model = ((App)Application.Current).Model;
            IEnumerable<NovaModule> modules = model.Modules;

            //TODO: Temporarily support only one module until the module list is built into the tree.
            var novaModule = modules.OrderByDescending(x => x.Ranking).FirstOrDefault();

            _Nodes = novaModule == null
                         ? Enumerable.Empty<NovaTreeNode>()
                         : novaModule.BuildNovaTreeNodes(ViewModel);

            return true;
        }

        public override void EnterCompleted()
        {
            var tree = View._NovaTree;
            
            tree.InitializeData(_Nodes);
        }
    }
}
