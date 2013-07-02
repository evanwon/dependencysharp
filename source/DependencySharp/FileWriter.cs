using System.IO;

namespace DependencySharp
{
    public class FileWriter : IFileWriter
    {
        public void WriteByteArrayToDisk(string path, byte[] data)
        {
            var directoryPath = Path.GetDirectoryName(path);
            var directoryExists = Directory.Exists(directoryPath);

            if (!directoryExists)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllBytes(path, data);
        }
    }
}
