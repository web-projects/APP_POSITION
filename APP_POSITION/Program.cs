using APP_POSITION.Config;
using APP_POSITION.Positioning;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace APP_POSITION
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"\r\n==========================================================================================");
            Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} - Version {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"==========================================================================================\r\n");

            AppConfig configuration = ConfigurationLoad();

            // Restore window position
            WindowHandling windowHandling = new WindowHandling(configuration, ".");

            ClearFileContents(configuration.Application.ClearLogFile);

            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();

            do
            {
                bool escapeKeyPressed = windowHandling.SetWindowPositioning();

                if (!escapeKeyPressed)
                {
                    Console.WriteLine("\r\nPRESS <ENTER> to RERUN\r\nPRESS <ESC> to QUIT\r\n");
                    keyPressed = Console.ReadKey(true);
                }
                else
                {
                    Console.WriteLine("\r\n\r\nUSER ABORTED PROCESS!");
                    break;
                }

            } while (keyPressed.Key != ConsoleKey.Escape);

            // save window position
            windowHandling.StoreWindowPositioning();
        }

        static void ClearFileContents(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // filename: logYYYYMMDD.txt
                DateTime dt = DateTime.Now;
                string timestamp = dt.ToString("yyyyMMdd");
                string file = Path.Combine(path, $"log{timestamp}.txt");

                try
                {
                    //using (FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    //{
                    //    lock (fs)
                    //    {
                    //        fs.SetLength(0);
                    //    }
                    //}
                    File.WriteAllText(file, string.Empty);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception clearing log: {ex.Message}");
                }
            }
        }

        static AppConfig ConfigurationLoad()
        {
            string appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // Get appsettings.json config.
            AppConfig configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build()
                .Get<AppConfig>();

            return configuration;
        }
    }
}
