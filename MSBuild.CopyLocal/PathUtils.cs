using System;

namespace KC.MSBuild.CopyLocal;

internal static class PathUtils
{
    public static string NormalizeSlashes(string path) =>
        path.Replace("\\", "/").Replace("/", Environment.NewLine);
}
