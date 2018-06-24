# Windows Azure Diagnostics to CSV

`.NET Core` [global tool][dotnet-global-tools] to retrieve logs from a `WADLogsTable` and write them to a `CSV` file.

The tool will attempt to obfuscate settings that have been logged due to this [issue][github-issue] in the [Microsoft.WindowsAzure.ConfigurationManager][configuration-manager-nuget].

| Package | Release | Pre-release |
| --- | --- | --- |
| `dotnet-wad-to-csv` | [![NuGet][nuget-tool-badge]][nuget-tool-command] | [![MyGet][myget-tool-badge]][myget-tool-command] |

| CI | Status | Platform(s) | Framework(s) |
| --- | --- | --- | --- |
| [AppVeyor][app-veyor] | [![Build Status][app-veyor-shield]][app-veyor] | `Windows` | `netcoreapp2.1` |

## Installation

```posh
> dotnet tool install -g dotnet-wad-to-csv
```

## Usage

```posh
> dotnet wad-to-csv -l <last> -o <output-file-path>
```

```posh
> dotnet wad-to-csv -f <from> -o <output-file-path>
```

```posh
> dotnet wad-to-csv -f <from> -t <to> -o <output-file-path>
```

- `<last>`: based on the `time designator` of the [ISO 8601 durations][iso-8601-duration]. This duration is then substracted from the current `UTC` time. For example:
  - `5M`: get all the logs for the last `5` **minutes**
  - `1H`: get all the logs for the last `1` **hour**
  - You can combine them too if you feel like it, i.e. `2H3M5S` would get all the logs for the last `7385` **seconds**
- `<from>`: `ISO 8601 date time` expressed in local time. Cannot be combined with `<last>`.
  - Valid date time: `2018-06-24T23:12:15`
  - The time component can be omitted: `2018-06-24`
- `<to>`: `ISO 8601 date time` expressed in local time. Must be be combined with `<from>`.
- `<output-file-path>`: where you wish to write the output file, does not need to exist but should be valid. If a file exists with the same name it will be replaced.

The tool will prompt you for a [Shared Access Signature][sas] so that it doesn't get saved to your `CLI` history. I recommend you restrict the `SAS` to:

- `Read` and `List` permissions
- `Tables` service
- `Container` and `Object` resource types
- A short expiration time

## Output file format

```csv
Generated,Level,Message
2018-06-18T09:50:28.155T,Information,"Some logging event I wrote"
```

- `Generated` is expressed in `UTC`
- The `Level` is converted from the `Level` column:
  - `1`: `Fatal`
  - `2`: `Error`
  - `3`: `Warning`
  - `4`: `Information`
  - `5`: `Verbose`
  - Everything else: `Undefined`

**Note**: this format is compatible with the `Splunk` `csv` source type.

[iso-8601-duration]: https://en.wikipedia.org/wiki/ISO_8601#Durations
[dotnet-global-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools
[sas]: https://docs.microsoft.com/en-us/azure/storage/common/storage-dotnet-shared-access-signature-part-1
[github-issue]: https://github.com/Azure/azure-sdk-for-net/issues/653
[app-veyor]: https://ci.appveyor.com/project/GabrielWeyer/dotnet-wad-to-csv
[app-veyor-shield]: https://img.shields.io/appveyor/ci/gabrielweyer/dotnet-wad-to-csv/master.svg?label=AppVeyor&style=flat-square
[nuget-tool-badge]: https://img.shields.io/nuget/v/dotnet-wad-to-csv.svg?label=NuGet&style=flat-square
[nuget-tool-command]: https://www.nuget.org/packages/dotnet-wad-to-csv
[myget-tool-badge]: https://img.shields.io/myget/gabrielweyer-pre-release/v/dotnet-wad-to-csv.svg?label=MyGet&style=flat-square
[myget-tool-command]: https://www.myget.org/feed/gabrielweyer-pre-release/package/nuget/dotnet-wad-to-csv
[configuration-manager-nuget]: https://www.nuget.org/packages/Microsoft.WindowsAzure.ConfigurationManager/
