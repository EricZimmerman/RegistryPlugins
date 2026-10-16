using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.Test;

/// <summary>Every plugin in the repository (the test project references them all), and which keys they are run on</summary>
public static class Plugins
{
    public static List<IRegistryPluginBase> All()
    {
        var plugins = new List<IRegistryPluginBase>();

        var dir = Path.GetDirectoryName(typeof(Plugins).Assembly.Location)!;
        foreach (var dll in Directory.GetFiles(dir, "RegistryPlugin.*.dll").OrderBy(t => t, StringComparer.Ordinal))
        {
            foreach (var type in Assembly.LoadFrom(dll).GetExportedTypes())
            {
                if (type.IsAbstract || typeof(IRegistryPluginBase).IsAssignableFrom(type) == false) continue;

                plugins.Add((IRegistryPluginBase)Activator.CreateInstance(type));
            }
        }

        return plugins;
    }

    /// <summary>As RECmd's GetPluginsToActivate: a plugin runs on a key one of its key paths matches (* matches any part)</summary>
    public static bool Matches(IRegistryPluginBase plugin, string keyPath)
    {
        foreach (var path in plugin.KeyPaths)
        {
            if (path.Contains('*'))
            {
                var regex = path.Replace("*", ".+?").Replace("\\", "\\\\");
                if (Regex.IsMatch(keyPath, $@"{regex}\z", RegexOptions.IgnoreCase)) return true;
            }
            else if (path.Equals(keyPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
