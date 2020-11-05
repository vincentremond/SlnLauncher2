using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SlnLauncher2
{
    internal static class Opener
    {
        private static readonly Lazy<string> _riderPath = new Lazy<string>(GetRiderPath);

        public static bool TryGetFromKeys(Keys key, out Action<string> action)
        {
            action = key switch
            {
                Keys.Alt | Keys.T => OpenWindowsTerminal,
                Keys.Alt | Keys.S => OpenRepositoryWithSourceTree,
                Keys.None | Keys.Enter => OpenSolutionWithRider,
                Keys.Shift | Keys.Enter => OpenContainingFolder,
                Keys.Control | Keys.Enter => OpenVisualStudioCode,
                Keys.Alt | Keys.Enter => OpenSolutionWithVisualStudio,
                Keys.Alt | Keys.Shift | Keys.Enter => OpenRepositoryWithSourceTree,
                _ => null,
            };
            return action != null;
        }

        internal static void DefaultAction(string item) => OpenSolutionWithRider(item);

        private static void OpenSolutionWithRider(string item)
        {
            var fi = GetInfo(item);
            if (!CheckIsSolutionFile(fi))
            {
                return;
            }

            Process.Start(_riderPath.Value, fi.FullName);
        }

        private static bool CheckIsSolutionFile(FileSystemInfo fsi)
        {
            if (fsi is FileInfo fi && fi.Name.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            MessageBox.Show($@"{fsi.FullName} is not a solution file");
            return false;
        }

        private static FileSystemInfo GetInfo(string path) =>
            File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                ? (FileSystemInfo)new DirectoryInfo(path)
                : new FileInfo(path);

        private static void OpenWindowsTerminal(string item)
        {
            var directory = GetDirectoryFullName(item);
            var windowsTerminal = LauncherConfiguration.Current.WindowsTerminalPath;

            Process.Start(
                new ProcessStartInfo(fileName: windowsTerminal)
                {
                    WorkingDirectory = directory,
                }
            );
        }

        private static void OpenContainingFolder(string item)
        {
            var directory = GetDirectoryFullName(item);
            Process.Start("explorer.exe", directory);
        }

        private static string GetRiderPath()
        {
            return LauncherConfiguration
                .Current
                .RiderPaths
                .SelectMany(
                    p => Directory
                        .GetDirectories(p, "JetBrains Rider *", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(i => i)
                )
                .Select(p => Path.Combine(p, @"bin\rider64.exe"))
                .Where(File.Exists)
                .FirstOrDefault();
        }

        private static string GetDirectoryFullName(string item)
        {
            return new FileInfo(item).Directory?.FullName
                   ?? throw new ApplicationException($"Failed to find directory for {item}");
        }

        private static void OpenRepositoryWithSourceTree(string item)
        {
            var folder = GetDirectoryFullName(item);
            Process.Start(LauncherConfiguration.Current.SourceTreePath, $"-f \"{folder}\"");
        }

        private static void OpenVisualStudioCode(string item)
        {
            var directory = GetDirectoryFullName(item);
            var path = LauncherConfiguration.Current.VisualStudioCodePath;
            Process.Start(path, directory);
        }

        private static void OpenSolutionWithVisualStudio(string item)
        {
            var fi = GetInfo(item);
            if (!CheckIsSolutionFile(fi))
            {
                return;
            }

            Process.Start(LauncherConfiguration.Current.VisualStudioPath, fi.FullName);
        }
    }
}
