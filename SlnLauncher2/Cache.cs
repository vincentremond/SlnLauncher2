using System;
using System.IO;

namespace SlnLauncher2;

public static class Cache
{
    public static string Init()
    {
        var localAppDataFolder = Environment.GetEnvironmentVariable("LocalAppData")
                                 ?? throw new ApplicationException("Failed to find local app data folder");

        var localCacheFolder = Path.Combine(localAppDataFolder, "SlnLauncher");
        var localCacheProjectsFile = Path.Combine(localCacheFolder, "FileCache.txt");

        if (!Directory.Exists(localCacheFolder))
        {
            Directory.CreateDirectory(localCacheFolder);
        }

        return localCacheProjectsFile;
    }
}