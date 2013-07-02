using System;

namespace DependencySharp
{
    /// <summary>
    /// Represents an unmanaged dependency. By describing the requirements of the dependency, 
    /// the DependencyManager class can ensure it exists in the correct location with the correct 
    /// version and filesize.
    /// </summary>
    public class UnmanagedDependency
    {
        /// <summary>
        /// The expected location of the dependency on the client system.
        /// </summary>
        public string DependencyPath { get; private set; }

        /// <summary>
        /// A byte array containing the entire dependency.
        /// </summary>
        public byte[] DependencyBytes { get; private set; }

        /// <summary>
        /// (Optional) The expected version of the dependency on the client system.
        /// </summary>
        public Version DependencyVersion { get; private set; }

        /// <summary>
        /// The size of the dependency, in bytes.
        /// </summary>
        public int? DependencyFileSizeInBytes { get; private set; }

        /// <summary>
        /// Represents an unmanaged dependency. By describing the requirements of the dependency, 
        /// the DependencyManager class can ensure it exists in the correct location with the correct 
        /// version and filesize.
        /// </summary>
        /// <param name="dependencyPath">The expected location of the dependency on the client system. For
        /// registry-free COM libraries, this should be the location of the executing assembly which can 
        /// be obtained via .NET's Reflection library or the AssemblyUtilities class in this library.</param>
        /// <param name="dependencyBytes">A byte array containing the entire dependency. This can be
        /// stored in a project as a Resource for convenience.</param>
        /// <param name="dependencyVersion">(Optional) The expected version of the dependency on the client system.</param>
        public UnmanagedDependency(string dependencyPath, byte[] dependencyBytes, Version dependencyVersion = null)
        {
            DependencyPath = dependencyPath;
            DependencyBytes = dependencyBytes;
            DependencyVersion = dependencyVersion;

            // Automatically generate the filesize from the provided byte array
            DependencyFileSizeInBytes = dependencyBytes.Length;
        }
    }
}
