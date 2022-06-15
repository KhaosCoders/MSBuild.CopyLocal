using NuGet.LibraryModel;
using NuGet.ProjectModel;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.MSBuild.CopyLocal;

internal class NugetReferences
{
    private readonly LockFileTarget _target;
    private readonly TargetFrameworkInformation _framework;

    public NugetReferences(LockFile lockfile, string targetFramework)
    {
        _target = lockfile.GetTarget(targetFramework);
        _framework = lockfile.GetFramework(targetFramework);
    }

    public IEnumerable<LockFileTargetLibrary> CollectDependencyLibraries()
    {
        HashSet<LockFileTargetLibrary> libraries = new();
        foreach (var dependency in _framework.Dependencies)
        {
            if (dependency.SuppressParent == LibraryIncludeFlags.All)
            {
                continue;
            }

            foreach (var lib in AddDependency(dependency))
            {
                libraries.Add(lib);
            }
        }
        return libraries;
    }

    private IEnumerable<LockFileTargetLibrary> AddDependency(LibraryDependency dependency)
    {
        var targetLib = FindTargetLibrary(dependency.Name, dependency.VersionOverride);
        foreach (var lib in AddLibDependency(targetLib))
        {
            yield return lib;
        }
    }

    private IEnumerable<LockFileTargetLibrary> AddLibDependency(LockFileTargetLibrary targetLib)
    {
        if (targetLib.Type != "package")
        {
            yield break;
        }

        yield return targetLib;

        foreach (var dep in targetLib.Dependencies)
        {
            var transLib = FindTargetLibrary(dep.Id, dep.VersionRange);
            foreach (var lib in AddLibDependency(transLib))
            {
                yield return lib;
            }
        }
    }

    private LockFileTargetLibrary FindTargetLibrary(string name, VersionRange versionOverride)
    {
        var candidates = _target.Libraries.Where(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        return candidates.Count switch
        {
            0 => throw new Exception($"Target library not found: {name}"),
            1 => candidates[0],
            // Not implemented
            _ => throw new NotImplementedException($"Dependency tree has multiple versions of: {name} ({candidates.Count})"),
        };
    }
}
