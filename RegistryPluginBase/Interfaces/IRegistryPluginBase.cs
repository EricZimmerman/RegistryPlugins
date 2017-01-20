using System.Collections.Generic;
using Registry.Abstractions;
using RegistryPluginBase.Classes;

namespace RegistryPluginBase.Interfaces
{
    public interface IRegistryPluginBase
    {
        /// <summary>
        ///     Gets the internal unique identifier.
        /// </summary>
        /// <remarks>
        ///     Set this to a static GUID value in plugin's constructor. This is used to make sure plugins aren't loaded more
        ///     than once.
        /// </remarks>
        /// <value>The internal unique identifier.</value>
        string InternalGuid { get; }

        /// <summary>
        ///     The path to the key this plugin handles.
        /// </summary>
        /// <remarks>Do not include the root key in the key path</remarks>
        /// <value>The key path.</value>
        List<string> KeyPaths { get; }

        /// <summary>
        ///     The value name this plugin handles
        /// </summary>
        /// <value>The name of the value.</value>
        string ValueName { get; }

        /// <summary>
        ///     Gets the alert message.
        /// </summary>
        /// <remarks>Optional message to display to user (an interesting value, missing info, etc)</remarks>
        /// <value>The alert message.</value>
        string AlertMessage { get; }

        RegistryPluginType.PluginType PluginType { get; }
        string Author { get; }
        string Email { get; }
        string Phone { get; }
        string PluginName { get; }
        string ShortDescription { get; }
        string LongDescription { get; }
        double Version { get; }
        List<string> Errors { get; }

        /// <summary>
        ///     Process raw values into plugin specific format
        /// </summary>
        /// <remarks>This method should populate the 'Values' property for plugins implementing this interface</remarks>
        /// <param name="key">The key where inValues originated. Also contains all subkeys, values, etc</param>
        void ProcessValues(RegistryKey key);
    }
}