using System;
using System.Collections.Generic;
using System.Linq;
using SlnLauncher2.DTO;

namespace SlnLauncher2
{
    internal class DuplicateCleaner
    {
        internal IEnumerable<ItemDescriptor> Clean(IEnumerable<ItemDescriptor> items)
        {
            return items
                .GroupBy(i => i.Path)
                .Select(FilterGroup)
                .SelectMany();
        }

        private IEnumerable<ItemDescriptor> FilterGroup(IEnumerable<ItemDescriptor> group)
        {
            var typedGroups = @group
                .GroupBy(i => i.GetType())
                .ToDictionary();

            if (typedGroups.Count == 2)
            {
                var dirs = typedGroups[typeof(DirectoryDescriptor)];
                var files = typedGroups[typeof(FileDescriptor)];
                if (dirs.Length == 1 && dirs[0].Name == ".git" && files.All(f => f.Name.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return files;
                }
            }

            return group;
        }
    }
}
