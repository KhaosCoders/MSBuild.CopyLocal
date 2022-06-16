# KC.MSBuild.CopyLocal
This is a small Nuget package, that helps with copying dependencies to the build output dir, when msbuild doesn't.

![Build](https://github.com/KhaosCoders/MSBuild.CopyLocal/actions/workflows/build.yaml/badge.svg)
![Test](https://github.com/KhaosCoders/MSBuild.CopyLocal/actions/workflows/test.yaml/badge.svg)
[![codecov](https://codecov.io/gh/KhaosCoders/MSBuild.CopyLocal/branch/main/graph/badge.svg?token=t0Glt2gBti)](https://codecov.io/gh/KhaosCoders/MSBuild.CopyLocal)
[![NuGet](https://img.shields.io/nuget/v/KC.MSBuild.CopyLocal.svg?logo=nuget)](https://www.nuget.org/packages/KC.MSBuild.CopyLocal/)

You should try the build-in `<CopyLocalLockFileAssemblies>` property ([see](https://docs.microsoft.com/de-de/dotnet/core/project-sdk/msbuild-props#copylocallockfileassemblies)) first, but if you need more control over the result, you'll need this package.

# Installation
Install this package using Package Manager Console:
```powershell
Install-Package KC.MSBuild.CopyLocal
```

Or a terminal:
```bash
dotnet add package KC.MSBuild.CopyLocal
```

# Output
Once installed this package will copy all runtime assemblies of your referenced `<PagckageReference>` dependencies to the build output dir. This is helpfull if you're developing a `class lib` project and need all used assemblies inside the output dir.

## Selective copy
You can decide which `<PackageReference>` dependencies you want to be copied to the build output dir. By setting the `PrivateAssets="All"` attribute, you can disable the local copy of a PackageReference and all transitiv dependencies.
```xml
<!-- This will not copy Serilog.dll to the build output dir -->
<PackageReference Include="Serilog" Version="11.0.0" PrivateAssets="All" />
```

# Known Issues
 - Beside runtime assemblies PackageReferences can include native or resource files. These are not covered yet.

# Support this <3

If you like my work, please support this project!  
Donate via [PayPal](https://www.paypal.com/donate?hosted_button_id=37PBGZPHXY8EC)
or become a [Sponsor on GitHub](https://github.com/sponsors/Khaos66)