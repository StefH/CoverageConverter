rem https://github.com/StefH/GitHubReleaseNotes

SET version=0.0.6

GitHubReleaseNotes --output ReleaseNotes.md --skip-empty-releases --exclude-labels question invalid doc --version %version%