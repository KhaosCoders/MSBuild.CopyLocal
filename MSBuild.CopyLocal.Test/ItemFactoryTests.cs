﻿using KC.MSBuild.CopyLocal;

namespace MSBuild.CopyLocal.Test;

[TestClass]
public class ItemFactoryTests
{
    private ItemFactory factory;

    [TestInitialize()]
    public void Startup()
    {
        var projectInstance = TestData.MockProject();
        factory = new ItemFactory(projectInstance);
    }

    [TestMethod]
    public void TestCreate()
    {
        const string nugetDir = @"C:\nuget";
        const string filePath = "lib/.net/assembly.dll";
        const string nugetId = "Dependency";
        const string nugetVersion = "1.0.0";
        var item = factory.Create(nugetDir, filePath, nugetId, nugetVersion, "runtime");

        var expectedFullPath = Path.Combine(nugetDir, filePath).Replace("/", @"\");
        var expectedFilePath = filePath.Replace("/", @"\");
        if (Environment.NewLine != @"\")
        {
            expectedFullPath = expectedFullPath.Replace(@"\", Environment.NewLine);
            expectedFilePath = expectedFilePath.Replace(@"\", Environment.NewLine);
        }

        Assert.IsNotNull(item);
        Assert.AreEqual(expectedFullPath, item.ItemSpec.Replace("/", Environment.NewLine));
        Assert.AreEqual(Path.GetFileName(filePath), item.GetMetadata("DestinationSubPath"));
        Assert.AreEqual("runtime", item.GetMetadata("AssetType"));
        Assert.AreEqual("true", item.GetMetadata("CopyLocal"));
        Assert.AreEqual("false", item.GetMetadata("CopyToPublishDirectory"));
        Assert.AreEqual(expectedFilePath, item.GetMetadata("PathInPackage").Replace("/", Environment.NewLine));
        Assert.AreEqual(nugetVersion, item.GetMetadata("NuGetPackageVersion"));
        Assert.AreEqual(nugetId, item.GetMetadata("NuGetPackageId"));
    }
}
