using NuGet.ProjectModel;
using NuGet.Versioning;

namespace KC.MSBuild.CopyLocal;

public interface INuGetPackageResolver
{
    string GetPackageDirectory(string packageId, NuGetVersion version);

    string GetPackageDirectory(string packageId, NuGetVersion version, out string packageRoot);

    string ResolvePackageAssetPath(LockFileTargetLibrary package, string relativePath);
}
