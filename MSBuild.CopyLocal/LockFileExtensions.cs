using NuGet.ProjectModel;
using System;
using System.Linq;

namespace KC.MSBuild.CopyLocal;

internal static class LockFileExtensions
{
    /// <summary>
    /// Gets the Nuget target for a TargetFramework
    /// </summary>
    /// <param name="lockfile">LockFile instance</param>
    /// <param name="targetFramework">Target framework</param>
    /// <exception cref="Exception"></exception>
    public static LockFileTarget GetTarget(this LockFile lockfile, string targetFramework)
    {
        var target = lockfile.Targets.FirstOrDefault(t => t.TargetFramework.ToString().Equals(targetFramework));
        if (target == null)
        {
            throw new Exception($"LockFile ({lockfile.Path}) doesn't contain target framework {targetFramework}.");
        }
        return target;
    }

    /// <summary>
    /// Gets the project framework information for a TargetFramework
    /// </summary>
    /// <param name="lockfile">LockFile instance</param>
    /// <param name="targetFramework">Target framework</param>
    /// <exception cref="Exception"></exception>
    public static TargetFrameworkInformation GetFramework(this LockFile lockfile, string targetFramework)
    {
        var group = lockfile.PackageSpec.TargetFrameworks.FirstOrDefault(f => f.FrameworkName.ToString() == targetFramework);
        if (group == null)
        {
            throw new Exception($"LockFile ({lockfile.Path}) doesn't contain project framework {targetFramework}.");
        }
        return group;
    }
}
