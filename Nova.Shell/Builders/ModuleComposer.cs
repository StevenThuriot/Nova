using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using Nova.Shell.Domain;
using Nova.Shell.Library;

namespace Nova.Shell.Builders
{
    static class ModuleComposer
    {
        public static ReadOnlyCollection<NovaModule> Compose(bool refreshContainer = false)
        {
            var builders = new List<ModuleBuilder>();

            var app = (App)Application.Current;
            var compositionManager = app.CompositionManager;
            var container = compositionManager.CompositionContainer;

            if (refreshContainer)
            {
                compositionManager.RebuildContainer();
                container = compositionManager.CompositionContainer;
            }

            var moduleConfigurations = container.GetExportedValues<IModule>();

            foreach (var module in moduleConfigurations)
            {
                var builder = new ModuleBuilder();
                module.Configure(builder);
                builders.Add(builder);
            }

            //Sort by ranking: Descending
            builders.Sort((x, y) => x.Ranking < y.Ranking ? 1 : (x.Ranking > y.Ranking ? -1 : 0));

            var modules = builders.Select(x => x.Build())
                                  .ToList()
                                  .AsReadOnly();

            app.Model.Modules = modules;
            return modules;
        }
    }
}
