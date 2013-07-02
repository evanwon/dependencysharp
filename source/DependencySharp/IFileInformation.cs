using System;

namespace DependencySharp
{
    /// <summary>
    /// This object wraps static .NET APIs in an instance class, so we can use dependency
    /// injection to replace it during unit testing. This allows for isolation of the DependencyManager
    /// class, so we won't actually touch the filesystem during test.
    /// </summary>
    public interface IFileInformation
    {
        /// <summary>
        /// Returns the version number of an assembly.
        /// </summary>
        /// <param name="path">The path of the assembly to analyze</param>
        /// <returns></returns>
        Version GetFileVersion(string path);

        /// <summary>
        /// Returns the filesize in bytes of an assembly.
        /// </summary>
        /// <param name="path">The path of the assembly to analyze</param>
        /// <returns></returns>
        long GetFileSizeInBytes(string path);

        /// <summary>
        /// Determines whether a file exists on disk.
        /// </summary>
        /// <param name="path">The path of the assembly to analyze</param>
        /// <returns></returns>
        bool DoesFileExist(string path);
    }
}
