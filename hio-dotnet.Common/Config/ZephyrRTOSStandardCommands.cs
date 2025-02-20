using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Config
{
    public class ZephyrRTOSCommand
    {
        public string Command { get; set; }
        public string Description { get; set; }
    }
    public static class ZephyrRTOSStandardCommands
    {
        /// <summary>
        /// List of Standard ZephyrRTOS commands and their help or description
        /// </summary>
        public static List<ZephyrRTOSCommand> StandardCommands { get; set; } = new List<ZephyrRTOSCommand>();

        /// <summary>
        /// Check if file exists and load commands from file
        /// Basic file is added as part of the drivers (zephyr_rtos_commands.txt).
        /// </summary>
        /// <param name="path"></param>
        public static void LoadCommandsFromFile(string path)
        {
            if (!File.Exists(path))
                return;
            try
            {
                var lines = File.ReadAllLines(path);
                LoadCommandsFromFile(lines.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Zephyr RTOS commands from file: {ex.Message}");
            }
        }
        public static async Task LoadCommandsFromFileViaHttp(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var lines = content.Split(Environment.NewLine).ToList();
                        LoadCommandsFromFile(lines);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Zephyr RTOS commands from URL: {ex.Message}");
            }
        }
        public static void LoadCommandsFromFile(List<string> lines)
        {
            StandardCommands = new List<ZephyrRTOSCommand>();

            foreach (var line in lines)
            {
                if (line.StartsWith("Command:"))
                {
                    var l = line.Replace("Command:", string.Empty);
                    if (l.Contains(" Help:"))
                    {
                        var parts = l.Split(" Help:");
                        if (parts.Length == 2)
                        {
                            StandardCommands.Add(new ZephyrRTOSCommand() { Command = parts[0], Description = parts[1] });
                        }
                    }
                    else
                    {
                        StandardCommands.Add(new ZephyrRTOSCommand() { Command = l, Description = "" });
                    }
                }
            }
        }
    }
}
