using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DependencySharp
{
    /// <summary>
    /// Unmanaged Dependencies Suck™
    /// This class provides methods to check for required unmanaged Windows dependencies on a system, 
    /// and to write them to disk if they do not exist.
    /// </summary>
    public class DependencyManager
    {
        /// <summary>
        /// For unit testing only. Do not use in production. 
        /// An instance wrapper to avoid directly calling static .NET filesystem APIs. This allows for
        /// dependency injection and unit testing.
        /// </summary>
        public IFileInformation FileInformation
        {
            get
            {
                // If the object hasn't been set externally by a unit test, instantiate 
                // a real object.
                return _fileInformation ?? new FileInformation();
            }
            set
            {
                _fileInformation = value;
            }
        }
        private IFileInformation _fileInformation;

        /// <summary>
        /// For unit testing only. Do not use in production. 
        /// An instance wrapper to avoid directly calling static .NET filesystem APIs. This allows for
        /// dependency injection and unit testing.
        /// </summary>
        public IFileWriter FileWriter
        {
            get
            {
                // If the object hasn't been set externally by a unit test, instantiate 
                // a real object.
                return _fileWriter ?? new FileWriter();
            }
            set
            {
                _fileWriter = value;
            }
        }
        private IFileWriter _fileWriter;

        /// <summary>
        /// For each provided dependency, checks if a dependency exists at the path specifed, 
        /// verifies its size in bytes is correct, and (optionally) its version is correct. If the dependency 
        /// fails one of those tests, it will be written to the provided location.
        /// </summary>
        /// <param name="dependencies">A collection of type UnmanagedDependency that describes the path of 
        /// the dependency, a byte array representing the contents of the dependency, its file size 
        /// (automatically derived from the length of the byte array), and (optionally) its version.</param>
        /// <returns>True if dependencies were extracted, false if the dependencies already existed.</returns>
        public bool VerifyDependenciesAndExtractIfMissing(IEnumerable<UnmanagedDependency> dependencies)
        {
            var dependenciesExtracted = false;

            foreach (var dependency in dependencies)
            {
                Debug.WriteLine("Processing dependency: " + dependency.DependencyPath);

                var path = dependency.DependencyPath;
                var version = dependency.DependencyVersion;

                // Check dependency - always check for existence first
                var dependencyExists = CheckFileExists(path);
                var isVersionCorrect = false;
                var isFileSizeCorrect = false;

                if (dependencyExists)
                {
                    isVersionCorrect = CheckVersion(path, version);
                    isFileSizeCorrect = CheckFileSize(path, dependency.DependencyFileSizeInBytes);
                }

                Debug.WriteLine(
                    string.Format(
                        "Dependency exists: ({0}) Version match: ({1}) Filesize match: ({2})",
                        dependencyExists,
                        isVersionCorrect,
                        isFileSizeCorrect));

                // If the dependency fails any checks, write it to disk
                if (!dependencyExists || !isVersionCorrect || !isFileSizeCorrect)
                {
                    var dependencyBytes = dependency.DependencyBytes;

                    WriteUnmanagedDependencyToDisk(path, dependencyBytes);

                    dependenciesExtracted = true;
                }
            }

            return dependenciesExtracted;
        }

        /// <summary>
        /// For each provided dependency, checks if a dependency exists at the path specifed, 
        /// verifies its size in bytes is correct, and (optionally) its version is correct. If the dependency 
        /// fails one of those tests, it will be written to the provided location.
        /// If the dependency was written to disk, executes a custom action specified by the user. This 
        /// could be used for registring a DLL, triggering a process, etc.
        /// </summary>
        /// <param name="dependencies">A collection of type UnmanagedDependency that describes the path of 
        /// the dependency, a byte array representing the contents of the dependency, its file size 
        /// (automatically derived from the length of the byte array), and (optionally) its version.</param>
        /// <param name="action"></param>
        public void VerifyDependenciesAndExtractIfMissingThenPerformAction(
            IEnumerable<UnmanagedDependency> dependencies, Action action)
        {
            var dependenciesExtracted = VerifyDependenciesAndExtractIfMissing(dependencies);

            // Only execute the action if the dependencies didn't exist
            if (dependenciesExtracted)
            {
                action();
            }
        }

        /// <summary>
        /// Verifies the version of the dependency on disk is correct.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool CheckVersion(string path, Version version)
        {
            // If no version provided, comparison is disabled so return true
            if (version == null) return true;

            // If file isn't found, fails comparison
            if (!FileInformation.DoesFileExist(path)) return false;

            var fileVersion = FileInformation.GetFileVersion(path);

            return fileVersion == version;
        }

        /// <summary>
        /// Verifies the filesize in bytes of the dependency on disk is correct.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedFileSizeInBytes"></param>
        /// <returns></returns>
        private bool CheckFileSize(string path, long? expectedFileSizeInBytes)
        {
            // If no filesize provided, size verification is disabled so return true
            if (expectedFileSizeInBytes == null) return true;

            // If file isn't found, fails comparison
            if (!FileInformation.DoesFileExist(path)) return false;

            var originalFileSize = FileInformation.GetFileSizeInBytes(path);

            return expectedFileSizeInBytes == originalFileSize;
        }

        /// <summary>
        /// Writes a dependency (in bytes from memory) to a file on disk
        /// </summary>
        /// <param name="path">Full path of the dependency's destination location</param>
        /// <param name="unmanagedDepencencyBytes">Dependency in bytes</param>
        private void WriteUnmanagedDependencyToDisk(string path, byte[] unmanagedDepencencyBytes)
        {
            Debug.WriteLine("Writing dependency: " + path);

            FileWriter.WriteByteArrayToDisk(path, unmanagedDepencencyBytes);
        }

        /// <summary>
        /// Checks if a required external unmanaged dependency exists on disk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckFileExists(string path)
        {
            return FileInformation.DoesFileExist(path);
        }
    }
}
