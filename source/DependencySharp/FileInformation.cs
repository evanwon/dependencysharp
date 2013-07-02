using System;
using System.Diagnostics;
using System.IO;

namespace DependencySharp
{
    public class FileInformation : IFileInformation
    {
        public Version GetFileVersion(string path)
        {
            var dependencyVersionInfo = FileVersionInfo.GetVersionInfo(path);
            var fileVersion = new Version(dependencyVersionInfo.FileVersion);

            return fileVersion;
        }

        public long GetFileSizeInBytes(string path)
        {
            var fileInfo = new FileInfo(path);

            var fileSize = fileInfo.Length;

            return fileSize;
        }

        public bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }
    }
}
