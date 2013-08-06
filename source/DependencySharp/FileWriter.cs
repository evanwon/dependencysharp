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

            if (File.Exists(path))
            {
                // Make sure file is writeable if it already exists
                var fileInfo = new FileInfo(path);

                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }

            File.WriteAllBytes(path, data);
        }
    }
}
