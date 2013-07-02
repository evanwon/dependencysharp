using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencySharp.UnitTests.Fakes
{
    public class FakeFileInformation : IFileInformation
    {
        public Version FakeExistingFileVersion { get; set; }

        public long FakeExistingFileSize { get; set; }

        public bool FakeFileExists { get; set; }

        public Version GetFileVersion(string path)
        {
            return FakeExistingFileVersion;
        }

        public long GetFileSizeInBytes(string path)
        {
            return FakeExistingFileSize;
        }

        public bool DoesFileExist(string path)
        {
            return FakeFileExists;
        }
    }
}
