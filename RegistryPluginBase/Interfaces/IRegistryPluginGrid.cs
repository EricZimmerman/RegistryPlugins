using System.ComponentModel;

namespace RegistryPluginBase.Interfaces
{
    public interface IRegistryPluginGrid : IRegistryPluginBase
    {
        /// <summary>
        ///     Gets the values after processing by a plugin.
        /// </summary>
        /// <remarks>The underlying class should extend ProcessedValue</remarks>
        /// <value>The values.</value>
        IBindingList Values { get; }
    }
}