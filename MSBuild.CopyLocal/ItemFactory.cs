using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KC.MSBuild.CopyLocal;

internal class ItemFactory
{
    private static readonly Type projectItemInstanceType = typeof(ProjectItemInstance);
    private static readonly FieldInfo taskItemField = projectItemInstanceType.GetField("_taskItem", BindingFlags.Instance | BindingFlags.NonPublic);

    private readonly ProjectInstance _project;

    public ItemFactory(ProjectInstance projectInstance)
    {
        _project = projectInstance;
    }

    public ITaskItem Create(string nugetDir, string filePath, string nugetId, string nugetVersion, string assetType)
    {
        filePath = PathUtils.NormalizeSlashes(filePath);
        string fullpath = PathUtils.NormalizeSlashes(Path.Combine(nugetDir, filePath));

        Dictionary<string, string> metadata = new()
        {
            ["DestinationSubPath"] = Path.GetFileName(filePath),
            ["PathInPackage"] = filePath,
            ["NuGetPackageId"] = nugetId,
            ["NuGetPackageVersion"] = nugetVersion,
            ["AssetType"] = assetType,
            ["CopyLocal"] = "true",
            ["CopyToPublishDirectory"] = "false",
        };

        // Use AddItem to create a new instance of ProjectItemInstance
        var item = _project.AddItem("Dependency", fullpath, metadata);
        _project.RemoveItem(item);

        return taskItemField.GetValue(item) as ITaskItem;
    }
}
