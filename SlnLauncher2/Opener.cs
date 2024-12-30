using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SlnLauncher2;

internal static class Opener
{
    private static readonly Lazy<string> _riderPath = new Lazy<string>(GetRiderPath);

    public static bool TryGetFromKeys(Keys key, out Action<string> action)
    {
        action = key switch
        {
            Keys.Alt | Keys.T => OpenWindowsTerminal,
            Keys.None | Keys.Enter => OpenSolutionWithRider,
            Keys.Shift | Keys.Enter => OpenContainingFolder,
            Keys.Control | Keys.Enter => OpenVisualStudioCode,
            Keys.Alt | Keys.Enter => OpenSolutionWithVisualStudio,
            Keys.Control | Keys.O => OpenRepositoryUrl,
            Keys.Control | Keys.K => CloneRepository,
            Keys.Alt | Keys.F => OpenGitFork,
            Keys.Control | Keys.F => OpenGitFork,
            _ => null,
        };
        return action != null;
    }

    private static void CloneRepository(string item)
    {
        var outputDirectory = "D:\\GIT";

        string Clone(string s)
        {
            using var process = new RunProcess.ProcessHost("git.exe", outputDirectory);
            process.Start($"clone {s}");
            process.WaitForExit(TimeSpan.MaxValue);
            return process.StdErr.ReadAllText(Encoding.UTF8);
        }

        string GetTargetDirectory(string cloneOutput)
        {
            var regex = "^Cloning into \'(?<Folder>.+?)\'";
            var match = Regex.Match(cloneOutput, regex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var outputFolder = match.Groups["Folder"].Value;
            return Path.Join(outputDirectory, outputFolder);
        }

        var url = GetRepositoryUrl(item);
        var cloneOutput = Clone(url);
        var targetDirectory = GetTargetDirectory(cloneOutput);
        Process.Start("explorer.exe", targetDirectory);
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
        var windowsTerminal = LauncherConfigurationContainer.Current.WindowsTerminalPath;

        Process.Start(
            new ProcessStartInfo(fileName: windowsTerminal) { WorkingDirectory = directory, }
        );
    }

    private static void OpenGitFork(string item)
    {
        var directory = GetDirectoryFullName(item);
        var gitForkPath = LauncherConfigurationContainer.Current.ForkPath;

        Process.Start(
            new ProcessStartInfo(fileName: gitForkPath, arguments: directory) { WorkingDirectory = directory, }
        );
    }

    private static void OpenContainingFolder(string item)
    {
        var directory = GetDirectoryFullName(item);
        Process.Start("explorer.exe", directory);
    }

    private static string GetRiderPath()
    {
        return LauncherConfigurationContainer
            .Current
            .RiderPaths
            .Where(Directory.Exists)
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

    private static void OpenRepositoryUrl(string item)
    {
        var url = GetRepositoryUrl(item);
        Process.Start(
            new ProcessStartInfo(url) { UseShellExecute = true, }
        );
    }

    private static string GetRepositoryUrl(string item)
    {
        var folder = GetDirectoryFullName(item);
        using var process = new RunProcess.ProcessHost("git.exe", folder);
        process.Start("config --get remote.origin.url");
        process.WaitForExit(TimeSpan.MaxValue);
        var url = process.StdOut.ReadAllText(Encoding.UTF8).Trim();
        return url;
    }

    private static void OpenVisualStudioCode(string item)
    {
        var directory = GetDirectoryFullName(item);
        var path = LauncherConfigurationContainer.Current.VisualStudioCodePath;
        Process.Start(path, directory);
    }

    private static void OpenSolutionWithVisualStudio(string item)
    {
        var fi = GetInfo(item);
        if (!CheckIsSolutionFile(fi))
        {
            return;
        }

        Process.Start(LauncherConfigurationContainer.Current.VisualStudioPath, fi.FullName);
    }
}