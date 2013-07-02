#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nova.Shell.Managers
{
    /// <summary>
    /// The instance that manages all of the composition needed for Nova.Shell.
    /// </summary>
    internal class CompositionManager
    {
        private readonly Lazy<CompositionContainer> _compositionContainer;

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public CompositionContainer CompositionContainer
        {
            get { return _compositionContainer.Value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionManager"/> class.
        /// </summary>
        public CompositionManager()
        {
            var task = Task.Run(() => CreateContainer());
            _compositionContainer = new Lazy<CompositionContainer>(() => task.Result);
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <remarks>This method is called on a different thread to speed up startup.</remarks>
        /// <returns></returns>
        private static CompositionContainer CreateContainer()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var assemblyCatalog = new AssemblyCatalog(entryAssembly);

            var location = entryAssembly.Location;
            var directory = new FileInfo(location).DirectoryName;

            var directoryCatalog = new DirectoryCatalog(directory, "*.dll");
            var aggregateCatalog = new AggregateCatalog(assemblyCatalog, directoryCatalog);

            //Subdirectories.
            var directories = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);
            foreach (var catalog in directories.Select(x => new DirectoryCatalog(x, "*.dll")))
            {
                aggregateCatalog.Catalogs.Add(catalog);
            }

            return new CompositionContainer(aggregateCatalog);
        }
    }
}
