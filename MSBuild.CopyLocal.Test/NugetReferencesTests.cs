using KC.MSBuild.CopyLocal;

namespace MSBuild.CopyLocal.Test;

[TestClass]
public class NugetReferencesTests
{
    private NugetReferences _references;

    [TestInitialize]
    public void Setup()
    {
        var lockfile = TestData.MockLockFile();

        _references = new(lockfile, ".net6,Version=v6.0.5");
    }

    [TestMethod]
    public void TestCollectDependencyLibraries()
    {
        var dependencies = _references.CollectDependencyLibraries();
        Assert.IsNotNull(dependencies);

        var deps = dependencies.ToList();
        Assert.AreEqual(4, deps.Count);
        Assert.AreEqual("FirstDependency", deps[0].Name);
        Assert.AreEqual("SecondDependency", deps[1].Name);
        Assert.AreEqual("SubDependency", deps[2].Name);
        Assert.AreEqual("SubSubDependency", deps[3].Name);
    }
}
