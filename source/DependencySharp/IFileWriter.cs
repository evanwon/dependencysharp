namespace DependencySharp
{
    /// <summary>
    /// This object wraps static .NET APIs in an instance class, so we can use dependency
    /// injection to replace it during unit testing. This allows for isolation of the DependencyManager
    /// class, so we won't actually touch the filesystem during test.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// Writes a byte array to a specified path on disk.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        /// <param name="data">A byte array of data.</param>
        void WriteByteArrayToDisk(string path, byte[] data);
    }
}
