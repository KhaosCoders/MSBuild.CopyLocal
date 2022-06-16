using NuGet.Packaging;
using NuGet.ProjectModel;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KC.MSBuild.CopyLocal;

internal sealed class NuGetPackageResolver
{
    private readonly FallbackPackagePathResolver _packagePathResolver;

    // Used when no package folders are provided, finds no packages.
    private static readonly NuGetPackageResolver s_noPackageFolderResolver = new NuGetPackageResolver();

    private NuGetPackageResolver()
    {
    }

    private NuGetPackageResolver(string userPackageFolder, IEnumerable<string> fallbackPackageFolders)
    {
        _packagePathResolver = new FallbackPackagePathResolver(userPackageFolder, fallbackPackageFolders);
    }

    public string GetPackageDirectory(string packageId, NuGetVersion version)
    {
        return _packagePathResolver?.GetPackageDirectory(packageId, version);
    }

    public string GetPackageDirectory(string packageId, NuGetVersion version, out string packageRoot)
    {
        var packageInfo = _packagePathResolver?.GetPackageInfo(packageId, version);
        if (packageInfo == null)
        {
            packageRoot = null;
            return null;
        }

        packageRoot = packageInfo.PathResolver.RootPath;
        return packageInfo.PathResolver.GetInstallPath(packageId, version);
    }

    public string ResolvePackageAssetPath(LockFileTargetLibrary package, string relativePath)
    {
        string packagePath = GetPackageDirectory(package.Name, package.Version);

        if (packagePath == null)
        {
            throw new Exception(
                string.Format("Package not found: {0} ({1})", package.Name, package.Version));
        }

        return Path.Combine(packagePath, NormalizeRelativePath(relativePath));
    }

    public static string NormalizeRelativePath(string relativePath)
    {
        return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }

    public static NuGetPackageResolver CreateResolver(LockFile lockFile)
    {
        return CreateResolver(lockFile.PackageFolders.Select(f => f.Path));
    }

    public static NuGetPackageResolver CreateResolver(IEnumerable<string> packageFolders)
    {
        string userPackageFolder = packageFolders.FirstOrDefault();

        if (userPackageFolder == null)
        {
            return s_noPackageFolderResolver;
        }

        return new NuGetPackageResolver(userPackageFolder, packageFolders.Skip(1));
    }
}
