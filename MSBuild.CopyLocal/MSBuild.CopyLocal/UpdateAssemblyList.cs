using Microsoft.Build.Framework;
using System;
using System.Linq;

namespace KC.MSBuild.CopyLocal;

public class UpdateAssemblyList : Microsoft.Build.Utilities.Task
{
    [Required]
    public string TargetFramework { get; set; }

    [Required]
    public string ProjectAssetsFile { get; set; }

    [Output]
    public ITaskItem[] RuntimeAssemblies { get; private set; }

    public override bool Execute()
    {
        try
        {
            var project = this.GetProject();
            var lockfile = ProjectAssetsUtils.LoadLockFile(ProjectAssetsFile);

            var references = new NugetReferences(lockfile, TargetFramework);
            var dependencyLibraries = references.CollectDependencyLibraries();

            var assemblies = new NugetAssemblys(project, lockfile);
            var dependencyAssemblies = assemblies.CollectDependencyAssemblies(dependencyLibraries, Log);

            RuntimeAssemblies = dependencyAssemblies.ToArray();
        }
        catch (Exception ex)
        {
            Log.LogError(ex.Message);
        }

        return !Log.HasLoggedErrors;
    }
}
