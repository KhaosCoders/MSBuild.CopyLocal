using System;

namespace KC.MSBuild.CopyLocal;

internal static class NuGetUtils
{
    public static bool IsPlaceholderFile(string path)
    {
        // PERF: avoid allocations here as we check this for every file in project.assets.json
        if (!path.EndsWith("_._", StringComparison.Ordinal))
        {
            return false;
        }

        if (path.Length == 3)
        {
            return true;
        }

        char separator = path[path.Length - 4];
        return separator == '\\' || separator == '/';
    }
}
