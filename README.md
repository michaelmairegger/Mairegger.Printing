# Mairegger.Printing
A printing library for WPF

Refer to [WIKI](https://github.com/xxMUROxx/Mairegger.Printing/wiki) for documentations.

## Installation 

Using NuGet

> Install-Package Mairegger.Printing 

Follow the link to the [NuGet Project Site](https://www.nuget.org/packages/Mairegger.Printing/).

## Branches and package versions

Package version = .NET version the line is built against (`src/Versions.props`); e.g. `10.x` contains only
`net10.0` (+ `net462`). Release lines: `release/v8.0`, `release/v9.0`, `release/v10.0` (and later lines).

| Branch | Target frameworks | Published |
|---|---|---|
| `main` | all (`CurrentNetVersion`/`PreviousNetVersion` in `src/Versions.props`, `NetFrameworkMinimum`) | no (build and test only) |
| `release/vX.0` | only `netX.0` (+ `NetFrameworkMinimum`) | every push: nuget.org `X.0.<.NET patch>[.<fix>]` and tag `v<version>` |

Publishing (`.github/workflows/publish.yml`) uses nuget.org [trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing): no API key secret; the nuget.org policy is bound
to the workflow file `publish.yml` and the GitHub environment `production`. Build, test and pack are shared with
`build.yml` through the composite action `.github/actions/build-and-test`.

`version.json` on `release/vX.0` holds the netX version from `src/Versions.props`; `main` holds `CurrentNetVersion`
with `.0-alpha` appended (`11.0.0.0-alpha`, shown by NuGet as `11.0.0-alpha`). The extra `.0` is required: the
Nerdbank.GitVersioning height only restarts when the numeric version changes, not when only `-alpha` is dropped.
The first package of a .NET version gets exactly that version (e.g. `10.0.15`); library fixes released without a new
.NET version get the Nerdbank.GitVersioning height as 4th component (`10.0.15.1`, `10.0.15.2`, ...). The build fails if
`version.json` does not match `src/Versions.props`, so after a .NET patch bump on `main` and `git merge main` into a
release branch, set its `version.json` to the new .NET version as well (on `main`: e.g. `11.0.1.0-alpha`).

A release branch differs from `main` only in `version.json`. If `version.json` has no prerelease suffix (no `-`
directly after the numeric part), `src/Directory.Packages.props` takes its major as `ReleaseNetMajor` and builds only
`netX.0` (in CI and locally); `main` (`-alpha`) builds all target frameworks. Override with `-p:ReleaseNetMajor=X`.

Update a release line with `git merge main` on the release branch. Run `git config merge.ours.driver true`
once per clone so `version.json` keeps the branch's version (see `.gitattributes`).

Create a new release line (e.g. for .NET 12, after `main` targets `net12.0` with `version.json` = `12.0.0.0-alpha`):

```
git switch -c release/v12.0 main
# version.json: "version": "12.0.0" (CurrentNetVersion), commit
```

The previous line (`release/v11.0`) keeps its own `version.json` (the `PreviousNetVersion`, e.g. `11.0.5`).

Release lines for .NET versions `main` no longer targets (`release/v8.0`, `release/v9.0`) also branch from `main`:
they additionally set that version as `MinimumNetVersion` in `src/Versions.props` (e.g. `8.0.34`), so the same
code is built for `net8.0`/`net9.0`. They are updated with `git merge main` as well, as long as `main` still
builds for that version (no dependency that requires a newer .NET).


## Version history

# 7.0.0
## Breaking changes

Remove support of `netcoreapp3.1`