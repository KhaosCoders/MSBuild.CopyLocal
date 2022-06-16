using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.MSBuild.CopyLocal;

internal class NugetAssemblys
{
    private readonly ItemFactory _itemFactory;
    private readonly NuGetPackageResolver _packageResolver;

    public NugetAssemblys(ProjectInstance project, LockFile lockfile)
    {
        _itemFactory = new ItemFactory(project);
        _packageResolver = NuGetPackageResolver.CreateResolver(lockfile);
    }

    public  IEnumerable<ITaskItem> CollectDependencyAssemblies(IEnumerable<LockFileTargetLibrary> dependencyLibs, TaskLoggingHelper log)
    {
        List<ITaskItem> runtimeTaskItem = new();
        foreach (var targetLibrary in dependencyLibs)
        {
            var package = new PackageIdentity(targetLibrary.Name, targetLibrary.Version);

            string libraryPath = _packageResolver.GetPackageDirectory(targetLibrary.Name, targetLibrary.Version, out string _);

            runtimeTaskItem.AddRange(GetResolvedFiles(targetLibrary.RuntimeAssemblies, package, libraryPath, "runtime"));
            runtimeTaskItem.AddRange(GetResolvedFiles(targetLibrary.NativeLibraries, package, libraryPath, "native"));

            // Todo RuntimeTargets: https://github.com/johnbeisner/remote1/blob/96b586b83e2bd218bcb9735f2600d5fbba99a44b/src/Tasks/Microsoft.NET.Build.Tasks/AssetsFileResolver.cs#L52
            if (targetLibrary.RuntimeTargets.Count > 0)
            {
                log.LogWarning($"RuntimeTargets in package {targetLibrary.Name} not supported yet.");
            }

            // Todo ResourceAssemblies https://github.com/johnbeisner/remote1/blob/96b586b83e2bd218bcb9735f2600d5fbba99a44b/src/Tasks/Microsoft.NET.Build.Tasks/AssetsFileResolver.cs#L75
            if (targetLibrary.ResourceAssemblies.Count > 0)
            {
                log.LogWarning($"ResourceAssemblies in package {targetLibrary.Name} not supported yet.");
            }
        }
        return runtimeTaskItem;
    }

    private IEnumerable<ITaskItem> GetResolvedFiles(IEnumerable<LockFileItem> items, PackageIdentity package, string libPath, string assetType)
    {
        foreach(var item in items)
        {
            if (NuGetUtils.IsPlaceholderFile(item.Path))
            {
                continue;
            }

            yield return _itemFactory.Create(libPath, item.Path, package.Id, package.Version.ToString(), assetType);
        }
    }
}
