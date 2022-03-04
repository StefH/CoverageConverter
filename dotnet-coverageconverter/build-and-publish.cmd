dotnet-releaser build --force dotnet-releaser.toml

dotnet-releaser publish --nuget-token %NG_TOKEN_CC% --github-token %GH_TOKEN% --force dotnet-releaser.toml