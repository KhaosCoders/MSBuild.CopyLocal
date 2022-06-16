using NuGet.ProjectModel;
using System;
using System.IO;

namespace KC.MSBuild.CopyLocal;

internal static class ProjectAssetsUtils
{
    /// <summary>
    /// Load the project.assets.json Nuget lockfile
    /// </summary>
    /// <param name="path">Path to project.assets.json</param>
    /// <exception cref="Exception"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static LockFile LoadLockFile(string path)
    {
        // https://github.com/NuGet/Home/issues/6732
        //
        // LockFileUtilties.GetLockFile has odd error handling:
        //
        //   1. Exceptions creating TextReader from path (after up to 3 tries) will
        //      bubble out.
        //
        //   2. There's an up-front File.Exists that returns null without logging
        //      anything.
        //
        //   3. Any other exception whatsoever is logged by its Message property
        //      alone, and an empty, non-null lock file is returned.
        //
        // This wrapper will never return null or empty lock file and instead throw
        // if the assets file is not found  or cannot be read for any other reason.

        LockFile lockFile;

        try
        {
            lockFile = LockFileUtilities.GetLockFile(
                path,
                NuGet.Common.NullLogger.Instance);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            // Case 1
            throw new Exception(
                string.Format($"Error reading project assets file: {ex.Message}"),
                ex);
        }

        if (lockFile == null)
        {
            // Case 2
            // NB: Cannot be moved to our own up-front File.Exists check or else there would be
            // a race where we still need to handle null for delete between our check and
            // NuGet's.
            throw new FileNotFoundException(path);
        }

        return lockFile;
    }
}
