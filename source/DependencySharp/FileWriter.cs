using System.IO;

namespace DependencySharp
{
    public class FileWriter : IFileWriter
    {
        public void WriteByteArrayToDisk(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }
    }
}
