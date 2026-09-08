---
name: registry-plugin-development
description: Create and update Registry Explorer and RECmd registry plugins in this repository.
---

# Registry plugin development

## Project layout

Each plugin is a separate `RegistryPlugin.<Name>` project targeting `netstandard2.0`. A typical plugin contains:

- A class implementing `IRegistryPluginGrid`.
- An output model implementing `IValueOut`.
- A `.csproj` that references `RegistryPluginBase`.
- `Properties/AssemblyInfo.cs` when assembly information is not generated.

Use an existing, similar plugin as the starting point. Keep changes confined to the plugin being added or updated unless a shared interface or test genuinely requires a change.

## Plugin contract

`IRegistryPluginGrid` plugins must expose:

- A stable, unique `InternalGuid`.
- `KeyPaths` without a registry root name; wildcards are supported.
- `ValueName` for a specific value, or `null` when the plugin processes multiple values.
- Plugin metadata, `Errors`, `AlertMessage`, `PluginType`, `ProcessValues`, and `Values`.

Use a `BindingList<T>` for `Values`. At the beginning of `ProcessValues`, clear the output and error collections. Catch parsing exceptions, append a useful error to `Errors`, and set `AlertMessage` if errors occurred.

## Output rows

Each output row must implement `IValueOut`:

- Set `BatchKeyPath` and `BatchValueName` to preserve the source registry location.
- Put the primary parsed result in `BatchValueData1`.
- Put secondary context, such as timestamps, in `BatchValueData2`.
- Put remaining context, such as indices or identifiers, in `BatchValueData3`.

Expose parsed fields as public properties so Registry Explorer can display them as columns. Convert registry `DateTimeOffset` values to UTC `DateTime` for output, and render timestamps consistently as `yyyy-MM-dd HH:mm:ss.fffffff`.

## Parsing guidance

- Use `Registry.Abstractions.RegistryKey` values and subkeys rather than relying on a hive root in the plugin.
- Check that expected values exist before parsing them.
- Preserve source ordering when it has forensic significance; explicitly add an index column when requested.
- Decode binary text using the encoding specified by the artifact format. For UTF-16LE registry strings, use `Encoding.Unicode`, split null-terminated sequences, and discard empty entries.

## Validation

Build the affected project from the repository root:

```sh
dotnet build RegistryPlugin.<Name>/RegistryPlugin.<Name>.csproj
```

When a suitable fixture exists, add or update an NUnit test in `RegistryPlugins.Test/PluginTests.cs` and run:

```sh
dotnet test RegistryPlugins.Test/RegistryPlugins.Test.csproj
```

Before committing, inspect the diff, run the repository secret scan on changed files, obtain a code review, and run CodeQL.
