using System;
using System.Collections.Generic;
using System.Diagnostics;

using DependencySharp.UnitTests.Fakes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencySharp.UnitTests
{
    [TestClass]
    public class DependencyManagerTest
    {
        private DependencyManager DependencyManager { get; set; }

        private FakeFileInformation FakeFileInformation { get; set; }

        private FakeFileWriter FakeFileWriter { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            DependencyManager = new DependencyManager();
            FakeFileInformation = new FakeFileInformation();
            FakeFileWriter = new FakeFileWriter();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DependencyManager = null;
            FakeFileInformation = null;
            FakeFileWriter = null;
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileExists_FileNotWritten()
        {
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll", new byte[5])
                                   };

            FakeFileInformation.FakeFileExists = true;
            FakeFileInformation.FakeExistingFileSize = 5;

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 0);
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileExistsEverythingMatches_FileNotWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(1, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = true;
            FakeFileInformation.FakeExistingFileSize = 5;
            FakeFileInformation.FakeExistingFileVersion = new Version(1, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Insert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 0);
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileMissing_FileWritten()
        {
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(1, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = false;
            FakeFileInformation.FakeExistingFileSize = 5;

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 1);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileExistsVersionMismatch_FileWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(1, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = true;
            FakeFileInformation.FakeExistingFileSize = 5;
            FakeFileInformation.FakeExistingFileVersion = new Version(2, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Assert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 1);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileExistsFileSizeMismatch_FileWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(1, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = true;
            FakeFileInformation.FakeExistingFileSize = 6;
            FakeFileInformation.FakeExistingFileVersion = new Version(1, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Assert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 1);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileExistsVersionAndFileSizeMismatch_FileWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(2, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = true;
            FakeFileInformation.FakeExistingFileSize = 6;
            FakeFileInformation.FakeExistingFileVersion = new Version(1, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Assert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 1);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_FileMissingVersionAndFileSizeMismatch_FileWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/important.dll",
                                           new byte[5],
                                           new Version(2, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = false;
            FakeFileInformation.FakeExistingFileSize = 6;
            FakeFileInformation.FakeExistingFileVersion = new Version(1, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Assert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 1);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
        }

        [TestMethod]
        public void VerifyDependenciesAndExtractIfMissing_MultipleFilesMissing_FilesWritten()
        {
            // Arrange
            var dependencies = new List<UnmanagedDependency>()
                                   {
                                       new UnmanagedDependency(
                                           "C:/one.dll", new byte[5], new Version(2, 0, 0)),
                                       new UnmanagedDependency(
                                           "C:/two.dll", new byte[5], new Version(2, 0, 0))
                                   };

            FakeFileInformation.FakeFileExists = false;
            FakeFileInformation.FakeExistingFileSize = 5;
            FakeFileInformation.FakeExistingFileVersion = new Version(2, 0, 0);

            DependencyManager.FileInformation = FakeFileInformation;
            DependencyManager.FileWriter = FakeFileWriter;

            // Act
            DependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);

            // Assert
            Assert.IsTrue(FakeFileWriter.FilesWritten.Count == 2);
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[0].DependencyPath));
            Assert.IsTrue(FakeFileWriter.FilesWritten.Contains(dependencies[1].DependencyPath));
        }
    }
}
