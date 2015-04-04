using System.ComponentModel.Composition;

namespace Nova.Shell.Library
{
    /// <summary>
    /// A base class to configure your module and prepare it for usage with the Nova Shell.
    /// </summary>
    [InheritedExport(typeof(IModule))]
    public interface IModule
    {
        /// <summary>
        /// Configures the module.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void Configure(IModuleBuilder builder);
    }
}
