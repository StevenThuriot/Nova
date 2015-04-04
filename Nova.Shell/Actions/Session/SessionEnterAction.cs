using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Nova.Library.Actions;
using Nova.Shell.Controls;
using Nova.Shell.Domain;

namespace Nova.Shell.Actions.Session
{
    /// <summary>
    /// The session enter action
    /// </summary>
    public class SessionEnterAction : EnterAction<SessionView, SessionViewModel>
    {
        private ReadOnlyCollection<NovaTreeModule> _modules;
        private INovaTree _tree;

        public override void OnBefore()
        {
            _tree = View._NovaTree;
        }

        public override bool Enter()
        {
            var model = ((App)Application.Current).Model;
            IEnumerable<NovaModule> modules = model.Modules;

            _modules = modules.OrderByDescending(x => x.Ranking)
                              .Select(x => x.Build(_tree, ViewModel))
                              .ToList()
                              .AsReadOnly();
            
            return true;
        }

        public override void EnterCompleted()
        {
            _tree.InitializeData(_modules);
        }
    }
}
