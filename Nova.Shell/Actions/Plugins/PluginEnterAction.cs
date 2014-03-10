using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Nova.Library.Actions;
using Nova.Shell.Modules;
using NuGet;

namespace Nova.Shell.Actions.Plugins
{
    internal class PluginEnterAction : EnterAction<PluginView, PluginViewModel>
    {
        private List<IPackage> _plugins;

        public override bool Enter()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var location = entryAssembly.Location;
            var pluginDirectory = new FileInfo(location).Directory;
            var pluginFolder = Path.Combine(pluginDirectory.FullName, "Modules");

            var repository = PackageRepositoryFactory.Default.CreateRepository(@"http://novamodules.apphb.com/nuget");

            var manager = new PackageManager(repository, pluginFolder);
            
            _plugins = repository.GetPackages().Where(x => x.IsLatestVersion).ToList();


            //// TODO: check if package is newer than one already downloaded, if so , only delete the specific older package and not the whole addin folder
            //if (Directory.Exists(pluginFolder))
            //    Directory.Delete(pluginFolder, true);

            foreach (var plugin in _plugins)
                manager.InstallPackage(plugin, false, true);


            var catalogs = pluginDirectory.GetDirectories("lib", SearchOption.AllDirectories).Select(x => new DirectoryCatalog(x.FullName));
            var directoryAggregate = new AggregateCatalog(catalogs);
            var container = new CompositionContainer(directoryAggregate);

            return base.Enter();
        }

        public override void EnterCompleted()
        {
            ViewModel.Plugins = _plugins;
            base.EnterCompleted();
        }
    }
}
