# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

- **`RegistryPluginBase`**: a `netstandard2.0` NuGet library (package id `RegistryPluginBase`) with the plugin interfaces (`IRegistryPluginBase`, `IRegistryPluginGrid`, `IValueOut`) and helpers. Its consumers are RECmd and Registry Explorer, so keep the public API compatible. It builds on Registry, ExtensionBlocks and GuidMapping; fixes to those belong in those repos, followed by a package bump here.
- **`RegistryPlugin.*`** (and `RegistryExplorer.MountedDevices`): one `netstandard2.0` plugin per project, with a project reference to RegistryPluginBase. They are not packages: the tools load `RegistryPlugin.*.dll` from their `Plugins` folder (`CopyPlugins.ps1 -Out <folder> [-Build]` copies the Release builds of the plugin projects in `RegistryPlugins.slnx` there, and fails when one has no Release build: a plugin project must be in the solution to ship). The hosts have RegistryPluginBase, Registry, Serilog, AlphaFS and ERZHelpers; anything else is embedded with Costura (`RegistryPlugin.AppCompatCache` embeds only the AppCompatCache package: `CopyLocalLockFileAssemblies` plus `IncludeAssemblies` in FodyWeavers.xml) or avoided (AppCompatFlags has its own LZNT1 decompressor instead of DiscUtils).
- `Directory.Build.props` makes every project non-packable except RegistryPluginBase.
- `.skills/registry-plugin-development/SKILL.md` (from PR #73) describes how to write a plugin.

LastVisitedPidlMRU, OpenSavePidlMRU and Taskband read their shell items with ExtensionBlocks' `ShellItemFactory.Create` (the classes ShellBags uses, with Lnk's fixes; since RegistryPluginBase 2026.10.2 they no longer have their own copies). An item that can't be read or isn't supported is an error, and the rest of the value is still listed.

Input may be corrupt or hostile: plugins look values up with `FirstOrDefault` (a key can have a value name twice), check sizes, and put problems in `Errors` rather than throwing.

## Commands

The SDK is pinned in `global.json` (.NET 10). Libraries use `LangVersion 10`. Tests target `net10.0` and use NUnit 5 + FluentAssertions 7 (pinned to `[7.2.2,8.0.0)`, since 8.x requires a commercial license).

```bash
dotnet build -c Release                                      # builds RegistryPlugins.slnx
dotnet test -c Release                                       # CI; HiveTests.Corpus is [Explicit]
dotnet test -c Release --filter "Name=Corpus"                # every plugin over every hive in ../Corpus/Registry (or REGISTRY_CORPUS); ~11 minutes
dotnet pack -c Release                                       # only RegistryPluginBase
```

## Tests

- `PluginContractTests`: every plugin loads, has a unique id and name, and key paths. `InternalGuid` is an id, not always a valid GUID (JumplistData's has an 'n'); saved settings use it, so ids never change.
- `HiveTests`: every plugin on every key it matches (RECmd's matching, in `Plugins.Matches`) in `Hives/` (ntuser1.dat, SAM); no exception may escape `ProcessValues` or the `BatchValueData` properties. `Corpus` does the same for every hive in the local corpus (logs replayed, deleted keys recovered).
- `PluginTests`: tests for specific plugins on hives in the local corpus (`TestHives.Corpus`, ignored when it's missing). Never copy corpus hives into the repo.
- `Lznt1Tests`: AppCompatFlags' decompressor on hand-made data.
- `PluginDumpTests.DumpCorpus` (`[Explicit]`): every value (all public properties, RECmd's batch columns) and `Errors` of the plugins named in `PLUGIN_DUMP_FILTER` (all when unset) for every key they match in the corpus, written to `PLUGIN_DUMP`. Run it on a worktree of the last release and on the change, and compare per plugin run. A package bump (Registry, ExtensionBlocks) is validated this way.

## Versioning and release

- RegistryPluginBase's version comes from the git tag through MinVer (bare tags like `2026.10.0`); there is no `<Version>` in its csproj. MinVer would make the assembly version `Major.0.0.0`, but 2026.5.0 shipped with assembly version 2026.5.0.0, so the csproj sets it to `Major.Minor.0.0` (plugins built against a version don't load with a lower one).
- Plugins have `AssemblyInfo.cs` versions, bumped by hand.
- To release RegistryPluginBase: push to `master`, then `git tag <ver> && git push origin <ver> && gh release create <ver> --verify-tag --generate-notes`.
- `.github/workflows/publish.yml` is identical to GuidMapping's. It packs, validates and tests on PRs and on pushes to `master`. When a GitHub Release is published, it pushes to NuGet.org with trusted publishing (`NuGet/login` with the `NUGET_USER` repo variable, `EricRZimmerman`).
- Dependabot (`.github/dependabot.yml`) opens monthly PRs for GitHub Actions and NuGet updates.
- `Build-ZimmermanTools.ps1` (get-zimmermantools) runs `CopyPlugins.ps1 -Build` and stages the DLLs in RECmd's and Registry Explorer's `Plugins`: every plugin project in the solution ships (remove a project from the solution to keep it out of a release).
