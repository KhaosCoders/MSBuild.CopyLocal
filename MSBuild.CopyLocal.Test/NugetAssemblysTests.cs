using KC.MSBuild.CopyLocal;
using Moq;
using NuGet.ProjectModel;
using NuGet.Versioning;

namespace MSBuild.CopyLocal.Test;

[TestClass]
public class NugetAssemblysTests
{
    private LockFile _lockfile;
    private NugetAssemblys _assemblies;

    [TestInitialize]
    public void Setup()
    {
        var project = TestData.MockProject();
        _lockfile = TestData.MockLockFile();

        var root = string.Empty;
        var resolver = new Mock<INuGetPackageResolver>();
        resolver.Setup(r => r.GetPackageDirectory(It.IsAny<string>(),It.IsAny<NuGetVersion>(),out root)).Returns(@"C:\packages");

        _assemblies = new(project, resolver.Object);
    }

    [TestMethod]
    public void TestCollectDependencyAssemblies()
    {
        var dependencies = TestData.MockDependencies(_lockfile);

        var libraries = _assemblies.CollectDependencyAssemblies(dependencies, null);

        Assert.IsNotNull(libraries);

        var libs = libraries.ToList();
        Assert.AreEqual(6, libs.Count);
        Assert.AreEqual(@"C:\packages\lib\.net6\first.dll", libs[0].ItemSpec);
        Assert.AreEqual(@"C:\packages\lib\.net6\second-1.dll", libs[1].ItemSpec);
        Assert.AreEqual(@"C:\packages\lib\.net6\second-2.dll", libs[2].ItemSpec);
        Assert.AreEqual(@"C:\packages\lib\.net6\sub.dll", libs[3].ItemSpec);
        Assert.AreEqual(@"C:\packages\lib\.net6\subsub-1.dll", libs[4].ItemSpec);
        Assert.AreEqual(@"C:\packages\lib\.net6\subsub-2.dll", libs[5].ItemSpec);
    }
}
