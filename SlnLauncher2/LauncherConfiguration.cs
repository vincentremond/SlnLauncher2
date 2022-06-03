using System;

namespace SlnLauncher2;

[Serializable]
public record LauncherConfiguration(
    string[] BasePaths,
    string WindowsTerminalPath,
    string VisualStudioCodePath,
    string VisualStudioPath,
    string ForkPath,
    string[] RiderPaths
);

internal static class LauncherConfigurationContainer
{
    internal static LauncherConfiguration Current { get; set; }
}

