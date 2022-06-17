using KC.MSBuild.CopyLocal;

namespace MSBuild.CopyLocal.Test;
[TestClass]
public class NuGetUtilsTests
{
    [DataTestMethod]
    [DataRow("_._", true)]
    [DataRow("path/to/dir/_._", true)]
    [DataRow("path\\to\\dir\\_._", true)]
    [DataRow("file.dll", false)]
    [DataRow("path/to/dir/file.dll", false)]
    [DataRow("path\\to\\dir\\file.dll", false)]
    [DataRow("xxx_._", false)]
    public void TestIsPlaceholderFile(string path, bool result)
    {
        Assert.AreEqual(result, NuGetUtils.IsPlaceholderFile(path));
    }
}
