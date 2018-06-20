# Windows Azure Diagnostics to CSV

`.NET Core` [global tool][dotnet-global-tools] to retrieve `MessageEvents` from a `WADLogsTable` and write them to a `CSV` file.

Only `MessageEvents` starting with `EventName="MessageEvent" Message="` will be parsed and written to the output file. This is to discard `MessageEvents` such as this one `EventName="DirectWrite" Message="Getting "TopSecret" from ServiceRuntime: PASS (<your-top-secret-value>)."` (refer to this [GitHub issue][github-issue] for context).

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

- `<last>`: based on the `time designator` of the [ISO 8601 durations][iso-8601-duration]. This duration is then substracted from the current `UTC` time. For example:
  - `5M`: get all the `MessageEvents` for the last `5` **minutes**
  - `1H`: get all the `MessageEvents` for the last `1` **hour**
  - You can combine them too if you feel like it, i.e. `2H3M5S` would get all the `MessageEvents` for the last `7385` **seconds**
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
