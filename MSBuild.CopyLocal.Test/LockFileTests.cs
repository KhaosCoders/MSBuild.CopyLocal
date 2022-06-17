using KC.MSBuild.CopyLocal;

namespace MSBuild.CopyLocal.Test;

[TestClass]
public class LockFileTests
{
    [TestMethod]
    public void TestGetTarget()
    {
        var lockfile = TestData.MockLockFile();

        var target = lockfile.GetTarget(".net6,Version=v6.0.5");
        Assert.IsNotNull(target);

        Assert.ThrowsException<Exception>(() => lockfile.GetTarget(".net5"));
    }

    [TestMethod]
    public void TestGetFramework()
    {
        var lockfile = TestData.MockLockFile();

        var framework = lockfile.GetFramework(".net6,Version=v6.0.5");
        Assert.IsNotNull(framework);

        Assert.ThrowsException<Exception>(() => lockfile.GetFramework(".net5"));
    }
}