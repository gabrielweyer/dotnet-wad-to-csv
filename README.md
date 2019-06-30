# Azure Diagnostics to CSV

`.NET Core` [global tools][dotnet-global-tools] to retrieve Azure Diagnostics logs from a `WADLogsTable` or blobs and write them to a `CSV` file.

| Package | Release |
| - | - |
| `dotnet-wad-to-csv` | [![NuGet][nuget-wad-tool-badge]][nuget-wad-tool-command] |
| `dotnet-blob-to-csv` | [![NuGet][nuget-blob-tool-badge]][nuget-blob-tool-command] |

| CI | Status | Platform(s) | Framework(s) |
| --- | --- | --- | --- |
| [AppVeyor][app-veyor] | [![Build Status][app-veyor-shield]][app-veyor] | `Windows` | `netcoreapp2.2` |

## Installation

```posh
> dotnet tool install -g dotnet-wad-to-csv
```

```posh
> dotnet tool install -g dotnet-blob-to-csv
```

## WAD to CSV Usage

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
- `<from>`: `ISO 8601 date time` expressed in `UTC`. Cannot be combined with `<last>`, can be combined with `<to>`.
  - Valid date time: `2018-06-24T23:12:15`
  - The time component can be omitted: `2018-06-24`
- `<to>`: `ISO 8601 date time` expressed in `UTC`. Must be be combined with `<from>`.
- `<output-file-path>`: where you wish to write the output file, does not need to exist but should be valid. If a file exists with the same name it will be replaced.

The tool will prompt you for a [Shared Access Signature][sas] so that it doesn't get saved to your `CLI` history. I recommend you restrict the `SAS` to:

- `Read` and `List` permissions
- `Tables` service
- `Container` and `Object` resource types
- A short expiration time

`WAD to CSV` will attempt to obfuscate settings that have been logged due to this [issue][github-issue] in the [Microsoft.WindowsAzure.ConfigurationManager][configuration-manager-nuget].

## Blob to CSV Usage

```posh
> dotnet blob-to-csv -l <last> -o <output-file-path> -c <container> -p <prefix>
```

```posh
> dotnet blob-to-csv -f <from> -o <output-file-path> -c <container> -p <prefix>
```

```posh
> dotnet blob-to-csv -f <from> -t <to> -o <output-file-path> -c <container> -p <prefix>
```

- `<last>`: based on the `time designator` of the [ISO 8601 durations][iso-8601-duration]. This duration is then substracted from the current `UTC` time. For example:
  - `5M`: get all the logs for the last `5` **minutes**
  - `1H`: get all the logs for the last `1` **hour**
  - You can combine them too if you feel like it, i.e. `2H3M5S` would get all the logs for the last `7385` **seconds**
- `<from>`: `ISO 8601 date time` expressed in `UTC`. Cannot be combined with `<last>`, can be combined with `<to>`.
  - Valid date time: `2018-06-24T23:12:15`
  - The time component can be omitted: `2018-06-24`
- `<to>`: `ISO 8601 date time` expressed in `UTC`. Must be be combined with `<from>`.
- `<output-file-path>`: where you wish to write the output file, does not need to exist but should be valid. If a file exists with the same name it will be replaced.
- `<container>`: The name of the container. For `https://account.blob.core.windows.net/container-name/prefix/2018/06/22/00/e872fe-54660.applicationLog.csv`, the container name is `container-name`.
- `<prefix>`: The prefix (if any). For `https://account.blob.core.windows.net/container-name/prefix/2018/06/22/00/e872fe-54660.applicationLog.csv`, the prefix is `prefix`.

The tool will prompt you for a [Shared Access Signature][sas] so that it doesn't get saved to your `CLI` history. I recommend you restrict the `SAS` to:

- `Read` and `List` permissions
- `Blobs` service
- `Container` and `Object` resource types
- A short expiration time

## Output file format

```csv
Generated,Level,Message
2018-06-18T09:50:28.155T,Information,"Some logging event I wrote"
```

- `Generated` is expressed in `UTC`
- The `Level` is written as-is for blobs and converted from the `Level` column for the `WADLogsTable`:
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
[nuget-wad-tool-badge]: https://img.shields.io/nuget/v/dotnet-wad-to-csv.svg?label=NuGet&style=flat-square
[nuget-wad-tool-command]: https://www.nuget.org/packages/dotnet-wad-to-csv
[configuration-manager-nuget]: https://www.nuget.org/packages/Microsoft.WindowsAzure.ConfigurationManager/
[nuget-blob-tool-badge]: https://img.shields.io/nuget/v/dotnet-blob-to-csv.svg?label=NuGet&style=flat-square
[nuget-blob-tool-command]: https://www.nuget.org/packages/dotnet-blob-to-csv
