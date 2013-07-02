using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencySharp.UnitTests.Fakes
{
    public class FakeFileWriter : IFileWriter
    {
        public List<string> FilesWritten { get; set; }

        public FakeFileWriter()
        {
            FilesWritten = new List<string>();
        }

        public void WriteByteArrayToDisk(string path, byte[] data)
        {
            FilesWritten.Add(path);
        }
    }
}