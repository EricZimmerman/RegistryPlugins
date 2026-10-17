using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Registry;
using Registry.Abstractions;
using Registry.Other;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.Test;

/// <summary>
///     Writes what plugins return for every key they match in the local corpus (every public property of every value,
///     RECmd's batch columns, and Errors) into the file named by PLUGIN_DUMP, to compare two builds (a package bump, a
///     plugin change): run it on each and diff the files. PLUGIN_DUMP_FILTER: plugin names to run (comma separated
///     parts of the name; all when not set)
/// </summary>
[TestFixture]
public class PluginDumpTests
{
    [Test]
    [Explicit("Set PLUGIN_DUMP to the file to write; needs the local Registry corpus")]
    public void DumpCorpus()
    {
        var target = Environment.GetEnvironmentVariable("PLUGIN_DUMP");
        if (string.IsNullOrEmpty(target)) Assert.Ignore("PLUGIN_DUMP is not set");

        var root = TestHives.CorpusRoot();
        if (root == null) Assert.Ignore("No local Registry corpus");

        var filter = (Environment.GetEnvironmentVariable("PLUGIN_DUMP_FILTER") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var plugins = Plugins.All().Where(p => filter.Length == 0 || filter.Any(f => p.GetType().FullName!.Contains(f, StringComparison.OrdinalIgnoreCase))).ToList();

        using var writer = new StreamWriter(target!, false, new UTF8Encoding(false));
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).OrderBy(t => t, StringComparer.Ordinal))
        {
            RegistryHive hive;
            try
            {
                if (IsHive(file) == false) continue;
                hive = new RegistryHive(file) { RecoverDeleted = true };
                hive.ParseHive();
            }
            catch (Exception)
            {
                continue;
            }

            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');

            void Walk(RegistryKey key)
            {
                var keyPath = Helpers.StripRootKeyNameFromKeyPath(key.KeyPath);
                foreach (var plugin in plugins.Where(t => Plugins.Matches(t, keyPath)))
                {
                    writer.WriteLine($"##### {relative} | {keyPath} | {plugin.GetType().FullName}");
                    try
                    {
                        plugin.ProcessValues(key);
                        foreach (var error in plugin.Errors) writer.WriteLine($"ERROR {error}");

                        if (plugin is IRegistryPluginGrid grid)
                        {
                            foreach (var value in grid.Values)
                            {
                                foreach (var property in value.GetType().GetProperties().Where(t => t.GetIndexParameters().Length == 0).OrderBy(t => t.Name, StringComparer.Ordinal))
                                {
                                    object v;
                                    try
                                    {
                                        v = property.GetValue(value);
                                    }
                                    catch (Exception e)
                                    {
                                        v = $"<{e.GetBaseException().GetType().Name}>";
                                    }

                                    writer.WriteLine($"{property.Name}: {Text(v)}");
                                }

                                writer.WriteLine("--");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        writer.WriteLine($"EXCEPTION {e.GetType().Name}: {e.Message}");
                    }
                }

                foreach (var subKey in key.SubKeys) Walk(subKey);
            }

            Walk(hive.Root);
        }
    }

    private static string Text(object v) =>
        v switch
        {
            null => "",
            byte[] b => BitConverter.ToString(b),
            DateTimeOffset d => d.ToString("O"),
            DateTime d => d.ToString("O"),
            string s => s,
            IEnumerable e => string.Join(" | ", e.Cast<object>().Select(Text)),
            _ => v.ToString()
        };

    private static bool IsHive(string file)
    {
        using var fs = File.OpenRead(file);
        var header = new byte[32];
        if (fs.Read(header, 0, header.Length) < header.Length) return false;
        return BitConverter.ToUInt32(header, 0) == 0x66676572 && BitConverter.ToUInt32(header, 28) == 0;
    }
}
