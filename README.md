# dotnet-CoverageConverter
Convert `.coverage` files to `.coveragexml` files to support SonarCloud Code Coverage when using **VSTest@2**.

## Installation

Install the tool via your yml pipeline:
``` yml
- task: DotNetCoreCLI@2
  displayName: "Install tool: dotnet-coverageconvertor"
  inputs:
    command: 'custom'
    custom: 'tool'
    arguments: 'update --global dotnet-coverageconvertor'
```

## Usage

- Step 1: Define a **VSTest@2** task with `codeCoverageEnabled: true`
- Step 2: Define a **CmdLine@2** task with the `dotnet-coverageconvertor`-tool and provide the CoverageFilesFolder where the `.coverage` files are placed by vstest.console.exe

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
    script: 'dotnet-coverageconvertor --CoverageFilesFolder "$(Agent.TempDirectory)\TestResults"'
```

## Commandline Options
```
  -f, --CoverageFilesFolder     Required. The folder where the .coverage files are defined.

  -d, --DotCoverageExtension    (Default: .coverage) The extension from the coverage files.

  -a, --AllDirectories          (Default: true) Includes also sub-folders in the search operation.

  -g, --Guid                    (Default: true) Only take .coverage file if the folder is a guid (that's the one VSTest creates).

  -o, --Overwrite               (Default: true) Overwrite the existing .coveragexml files.
```

### Info
This project is inspired by [Visual-Studio-Coverage-Tools](https://github.com/danielpalme/ReportGenerator/wiki/Visual-Studio-Coverage-Tools).