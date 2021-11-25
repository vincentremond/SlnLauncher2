using System;

namespace SlnLauncher2
{
    [Serializable]
    public class LauncherConfiguration
    {
        public string[] BasePaths { get; set; }
        public string WindowsTerminalPath { get; set; }
        public string VisualStudioCodePath { get; set; }
        public string VisualStudioPath { get; set; }
        public string[] RiderPaths { get; set; }

        internal static LauncherConfiguration Current { get; set; }
    }
}
