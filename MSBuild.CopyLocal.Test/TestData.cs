using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using NuGet.Versioning;
using System.Xml;

namespace MSBuild.CopyLocal.Test;

internal static class TestData
{
    private const string ProjectXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
</Project>
";

    internal static ProjectInstance MockProject()
    {
        using var xml = XmlReader.Create(new StringReader(ProjectXml));
        var project = new Project(xml, default, "Current", ProjectCollection.GlobalProjectCollection, ProjectLoadSettings.RecordDuplicateButNotCircularImports)
        {
            SkipEvaluation = true
        };
        return new ProjectInstance(project, ProjectInstanceSettings.None);
    }

    internal static LockFile MockLockFile()
    {
        LockFile lockfile = new()
        {
            PackageSpec = new()
        };
        lockfile.Targets.Add(new LockFileTarget()
        {
            TargetFramework = new(new NuGetFramework(".net6", new Version("6.0.5"))),
            Libraries = new List<LockFileTargetLibrary>()
            {
                new LockFileTargetLibrary()
                {
                    Name = "FirstDependency",
                    Version = new NuGetVersion(1,0,0),
                    Type = "package",
                    RuntimeAssemblies = new List<LockFileItem>()
                    {
                        new LockFileItem("lib/.net6/first.dll")
                    }
                },
                new LockFileTargetLibrary()
                {
                    Name = "SecondDependency",
                    Version = new NuGetVersion(2,0,0),
                    Type = "package",
                    RuntimeAssemblies = new List<LockFileItem>()
                    {
                        new LockFileItem("lib/.net6/second-1.dll"),
                        new LockFileItem("lib/.net6/second-2.dll")
                    },
                    Dependencies = new List<PackageDependency>()
                    {
                        new PackageDependency("SubDependency")
                    }
                },
                new LockFileTargetLibrary()
                {
                    Name = "SubDependency",
                    Version = new NuGetVersion(3,0,0),
                    Type = "package",
                    RuntimeAssemblies = new List<LockFileItem>()
                    {
                        new LockFileItem("lib/.net6/sub.dll")
                    },
                    Dependencies = new List<PackageDependency>()
                    {
                        new PackageDependency("SubSubDependency")
                    }
                },
                new LockFileTargetLibrary()
                {
                    Name = "SubSubDependency",
                    Version = new NuGetVersion(4,0,0),
                    Type = "package",
                    RuntimeAssemblies = new List<LockFileItem>()
                    {
                        new LockFileItem("lib/.net6/subsub-1.dll"),
                        new LockFileItem("lib/.net6/subsub-2.dll")
                    }
                }
            }
        });
        lockfile.PackageSpec.TargetFrameworks.Add(new TargetFrameworkInformation()
        {
            FrameworkName = new NuGetFramework(".net6", new Version("6.0.5")),
            Dependencies = new List<LibraryDependency>()
            {
                new LibraryDependency()
                {
                    SuppressParent = LibraryIncludeFlags.All,
                    LibraryRange = new LibraryRange("SkipDependency", LibraryDependencyTarget.Assembly)
                },
                new LibraryDependency()
                {
                    LibraryRange = new LibraryRange("FirstDependency", LibraryDependencyTarget.Assembly)
                },
                new LibraryDependency()
                {
                    LibraryRange = new LibraryRange("SecondDependency", LibraryDependencyTarget.Assembly)
                }
            }
        });
        return lockfile;
    }

    public static IEnumerable<LockFileTargetLibrary> MockDependencies(LockFile lockfile)
    {
        return new List<LockFileTargetLibrary>(lockfile.Targets[0].Libraries);
    }
}
