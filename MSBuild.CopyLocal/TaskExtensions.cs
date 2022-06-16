using Microsoft.Build.Execution;
using Microsoft.Build.Utilities;
using System.Reflection;

namespace KC.MSBuild.CopyLocal;

internal static class TaskExtensions
{
    /// <summary>
    /// Find the build project instance for the current build task
    /// </summary>
    /// <param name="task">Current build task</param>
    public static ProjectInstance GetProject(this Task task)
    {
        object targetBuilderCallback =
            task.BuildEngine
            .GetType()
            .GetField("_targetBuilderCallback", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(task.BuildEngine);

        return targetBuilderCallback
            .GetType()
            .GetField("_projectInstance", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(targetBuilderCallback) as ProjectInstance;
    }
}
