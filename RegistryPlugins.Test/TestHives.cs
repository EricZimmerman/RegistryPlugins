using System;
using System.IO;
using NUnit.Framework;
using Registry;

namespace RegistryPlugins.Test;

/// <summary>Where test hives are: Hives (copied next to the tests), and the local corpus for the ones that aren't in the repo</summary>
public static class TestHives
{
    /// <summary>A hive in Hives</summary>
    public static RegistryHive Repo(string name) => new(RepoPath(name));

    public static string RepoPath(string name) =>
        System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Hives", name);

    /// <summary>
    ///     The local Registry corpus: Corpus/Registry next to the repository, or REGISTRY_CORPUS. Null when there is none
    /// </summary>
    public static string CorpusRoot()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("REGISTRY_CORPUS");
        if (string.IsNullOrEmpty(fromEnvironment) == false) return fromEnvironment;

        //the repository is the folder with RegistryPlugins.slnx
        for (var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(System.IO.Path.Combine(dir.FullName, "RegistryPlugins.slnx")) == false) continue;

            var corpus = System.IO.Path.Combine(dir.Parent?.FullName ?? dir.FullName, "Corpus", "Registry");
            return Directory.Exists(corpus) ? corpus : null;
        }

        return null;
    }

    /// <summary>A hive in the corpus (written with Windows separators); the test is ignored when it isn't there</summary>
    public static RegistryHive Corpus(string name)
    {
        var root = CorpusRoot();
        var path = root == null ? null : System.IO.Path.Combine(root, name.Replace('\\', System.IO.Path.DirectorySeparatorChar));
        if (path == null || File.Exists(path) == false) Assert.Ignore($"{name} is not in the local Registry corpus (Corpus/Registry next to the repository, or REGISTRY_CORPUS)");

        return new RegistryHive(path);
    }
}
