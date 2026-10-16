using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Registry;
using Registry.Abstractions;
using Registry.Other;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.Test;

/// <summary>Every plugin on every key it matches, as RECmd runs them: a plugin reports bad data in Errors and doesn't throw</summary>
[TestFixture]
public class HiveTests
{
    [TestCase("ntuser1.dat")]
    [TestCase("SAM")]
    public void RepoHive(string name)
    {
        RunAll(TestHives.RepoPath(name)).Should().BeEmpty();
    }

    /// <summary>Every hive (file type 0, so not the logs) in the local corpus</summary>
    [Test]
    [Explicit("needs the local Registry corpus; slow")]
    public void Corpus()
    {
        var root = TestHives.CorpusRoot();
        if (root == null) Assert.Ignore("No local Registry corpus (Corpus/Registry next to the repository, or REGISTRY_CORPUS)");

        var failures = new List<string>();
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Where(IsHive))
        {
            RegistryHive hive;
            try
            {
                hive = Open(file);
            }
            catch (Exception)
            {
                //hives Registry can't parse are its problem (its own corpus tests)
                continue;
            }

            failures.AddRange(RunAll(hive).Select(t => $"{Path.GetRelativePath(root, file)}: {t}"));
        }

        failures.Should().BeEmpty();
    }

    private static bool IsHive(string file)
    {
        try
        {
            using var fs = File.OpenRead(file);
            var header = new byte[32];
            if (fs.Read(header, 0, header.Length) < header.Length) return false;

            //regf, file type 0 (logs are 1, 2 or 6)
            return BitConverter.ToUInt32(header, 0) == 0x66676572 && BitConverter.ToUInt32(header, 28) == 0;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static RegistryHive Open(string file)
    {
        var hive = new RegistryHive(file) { RecoverDeleted = true };

        if (hive.Header.PrimarySequenceNumber != hive.Header.SecondarySequenceNumber || hive.Header.ValidateCheckSum() == false)
        {
            var name = Path.GetFileName(file);
            var logs = Directory.GetFiles(Path.GetDirectoryName(file)!)
                .Where(t => Path.GetFileName(t).StartsWith(name + ".LOG", StringComparison.OrdinalIgnoreCase) && new FileInfo(t).Length > 0)
                .ToList();
            if (logs.Count > 0) hive.ProcessTransactionLogs(logs, true);
        }

        hive.ParseHive();
        return hive;
    }

    private static List<string> RunAll(string file) => RunAll(Open(file));

    private static List<string> RunAll(RegistryHive hive)
    {
        var plugins = Plugins.All();
        var failures = new List<string>();

        var keys = new List<RegistryKey>();
        void Walk(RegistryKey key)
        {
            keys.Add(key);
            foreach (var subKey in key.SubKeys) Walk(subKey);
        }

        Walk(hive.Root);

        foreach (var key in keys)
        {
            var keyPath = Helpers.StripRootKeyNameFromKeyPath(key.KeyPath);
            foreach (var plugin in plugins.Where(t => Plugins.Matches(t, keyPath)))
            {
                try
                {
                    plugin.ProcessValues(key);

                    if (plugin is IRegistryPluginGrid grid)
                    {
                        foreach (var value in grid.Values.Cast<IValueOut>())
                        {
                            //what RECmd writes for each value
                            _ = value.BatchValueData1 + value.BatchValueData2 + value.BatchValueData3;
                        }
                    }
                }
                catch (Exception e)
                {
                    failures.Add($"{plugin.PluginName} on {keyPath}: {e.GetType().Name}: {e.Message}");
                }
            }
        }

        return failures;
    }
}
