using System;
using System.IO;
using System.Windows.Forms;

namespace SlnLauncher2;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        try
        {
            // Ugly
            LauncherConfigurationContainer.Current = ReadConfiguration<LauncherConfiguration>("Configuration.yml");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SlnLauncher());
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
            throw;
        }
    }

    private static T ReadConfiguration<T>(string configurationFile)
    {
        using var content = File.OpenText(configurationFile);
        var deserializer = new YamlDotNet.Serialization.Deserializer();
        var yamlConfigurationObject = deserializer.Deserialize(content);
        var jsonConfigurationContent = Newtonsoft.Json.JsonConvert.SerializeObject(yamlConfigurationObject);
        var configuration = System.Text.Json.JsonSerializer.Deserialize<T>(jsonConfigurationContent);
        return configuration;
    }
}
