namespace KC.MSBuild.CopyLocal;

internal static class PathUtils
{
    public static string NormalizeSlashes(string path)
    {
        return path.Replace('\\', '/').Replace('/', System.IO.Path.PathSeparator);
    }
}
