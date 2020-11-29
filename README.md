# dotnet-coverageconverter
Convert `.coverage` (binary format) files to `.coveragexml` (xml format) files to support SonarCloud Code Coverage when using **VSTest@2**.

[![NuGet Badgedotnet-coverageconverter](https://buildstats.info/nuget/dotnet-coverageconverter)](https://www.nuget.org/packages/dotnet-coverageconverter)

## Installation
Install locally:
``` cmd
dotnet tool install --global dotnet-coverageconverter
```

or

Install the tool in your yml pipeline:
``` yml
- task: DotNetCoreCLI@2
  displayName: "Install tool: dotnet-coverageconverter"
  inputs:
    command: 'custom'
    custom: 'tool'
    arguments: 'update --global dotnet-coverageconverter'
```

## Usage

- Step 1: Define a **VSTest@2** task with `codeCoverageEnabled: true`
- Step 2: Define a **CmdLine@2** task with the `dotnet-coverageconverter`-tool and provide the CoverageFilesFolder where the `.coverage` files are placed by vstest.console.exe

#### Example YML pipeline tasks
``` yml
- task: VSTest@2
  displayName: 'VsTest'
  inputs:
    testSelector: 'testAssemblies'
    vsTestVersion: 16.0
    diagnosticsEnabled: true
    codeCoverageEnabled: true
    testAssemblyVer2: |
     **\*tests.dll
     !**\*UITests.dll
     !**\*TestAdapter.dll
     !**\obj\**

- task: CmdLine@2
  displayName: 'Convert .coverage to .coveragexml'
  inputs:
    script: 'dotnet-coverageconverter --CoverageFilesFolder "$(Agent.TempDirectory)\TestResults"'
```

## Commandline Options
```
  -f, --CoverageFilesFolder            Required. The folder where the .coverage files are defined.

  -d, --DotCoverageExtension           (Default: .coverage) The extension from the coverage files.

  -a, --AllDirectories                 (Default: true) Includes also sub-folders in the search operation.

  -p, --ProcessAllFiles                (Default: false) Process all .coverage files, if not set, then only folders which are a guid (that's the one VSTest creates) will be processed.

  -o, --Overwrite                      (Default: true) Overwrite the existing .coveragexml files.

  -r, --RemoveOriginalCoverageFiles    Remove the original .coverage files.

  --help                               Display this help screen.

  --version                            Display version information.
```

### Info
This project is inspired by [Visual-Studio-Coverage-Tools](https://github.com/danielpalme/ReportGenerator/wiki/Visual-Studio-Coverage-Tools).
