using System;
using System.IO;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace SlnLauncher2
{
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
                LauncherConfiguration.Current = ReadConfiguration<LauncherConfiguration>("Configuration.yml");

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
            var deserializer = new Deserializer();
            var content = File.ReadAllText(configurationFile);
            return deserializer.Deserialize<T>(content);
        }
    }
}
