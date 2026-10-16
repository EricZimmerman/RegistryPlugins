using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugins.Test;

/// <summary>What RECmd and Registry Explorer rely on</summary>
[TestFixture]
public class PluginContractTests
{
    [Test]
    public void EveryPluginLoads()
    {
        var dlls = System.IO.Directory.GetFiles(TestContext.CurrentContext.TestDirectory, "RegistryPlugin.*.dll");

        //every plugin project has one plugin (RegistryPluginBase.dll doesn't match the pattern)
        Plugins.All().Should().HaveCount(dlls.Length);
    }

    [Test]
    public void PluginsHaveUniqueGuidsAndNames()
    {
        var plugins = Plugins.All();

        foreach (var plugin in plugins)
        {
            //it is an id, not always a valid GUID (JumplistData's has an 'n'), and saved settings use it, so it stays as it is
            plugin.InternalGuid.Should().NotBeNullOrWhiteSpace(plugin.PluginName);
            plugin.PluginName.Should().NotBeNullOrWhiteSpace();
            plugin.KeyPaths.Should().NotBeNullOrEmpty(plugin.PluginName);
            plugin.KeyPaths.Should().NotContain(t => string.IsNullOrWhiteSpace(t), plugin.PluginName);
            plugin.Errors.Should().NotBeNull(plugin.PluginName);
        }

        plugins.Select(t => t.InternalGuid.ToLowerInvariant()).Should().OnlyHaveUniqueItems();
        plugins.Select(t => t.PluginName).Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void GridPluginsHaveValues()
    {
        foreach (var plugin in Plugins.All().OfType<IRegistryPluginGrid>())
        {
            plugin.Values.Should().NotBeNull(plugin.PluginName);
            plugin.Values.Count.Should().Be(0, plugin.PluginName);
        }
    }
}
