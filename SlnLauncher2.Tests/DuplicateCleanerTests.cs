using System.Collections.Generic;
using NUnit.Framework;
using SlnLauncher2.DTO;

namespace SlnLauncher2.Tests
{
    public class DuplicateCleanerTests
    {
        [Test]
        public void CleanupTest()
        {
            var input = new List<ItemDescriptor>
            {
                new DirectoryDescriptor(@"D:\DIR\proj1", ".git"),
                new FileDescriptor(@"D:\DIR\proj2", "solution.sln"),
                new DirectoryDescriptor(@"D:\DIR\proj3", ".git"),
                new FileDescriptor(@"D:\DIR\proj3", "solution.sln"),
                new DirectoryDescriptor(@"D:\DIR\proj4", ".git"),
                new FileDescriptor(@"D:\DIR\proj4\subdir", "solution.sln"),
            };
            
            var sut = new DuplicateCleaner();
            var result = sut.Clean(input);

            var expected = new List<ItemDescriptor>
            {
                new DirectoryDescriptor(@"D:\DIR\proj1", ".git"),
                new FileDescriptor(@"D:\DIR\proj2", "solution.sln"),
                new FileDescriptor(@"D:\DIR\proj3", "solution.sln"),
                new DirectoryDescriptor(@"D:\DIR\proj4", ".git"),
                new FileDescriptor(@"D:\DIR\proj4\subdir", "solution.sln"),
            };

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
