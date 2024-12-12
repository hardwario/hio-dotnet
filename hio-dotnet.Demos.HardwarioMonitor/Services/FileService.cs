using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Demos.HardwarioMonitor.Services
{
    public class FileService
    {
        public async Task<string?> LoadJSONFile()
        {
            try
            {
                var jsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } }, // iOS
                    { DevicePlatform.Android, new[] { "application/json" } }, // Android
                    { DevicePlatform.WinUI, new[] { ".json" } }, // Windows
                    { DevicePlatform.MacCatalyst, new[] { "json" } } // Mac
                });

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a JSON file",
                    FileTypes = jsonFileType
                });

                if (result == null)
                {
                    Console.WriteLine("File picking canceled.");
                    return null;
                }

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                Console.WriteLine($"File Loaded: {result.FileName}");
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loading of the file failed: {ex.Message}");
                return null;
            }
        }

        public async Task SaveJSONFile(string content, string filename = "template.json", bool addTimestampPrefix = true)
        {
            try
            {
                var fileBytes = Encoding.UTF8.GetBytes(content);
                using var stream = new System.IO.MemoryStream(fileBytes);

                filename = addTimestampPrefix
                    ? $"{DateTime.Now:yyyyMMddHHmmss}_{filename}"
                    : filename;

                var result = await FileSaver.Default.SaveAsync(
                    "",
                    filename,
                    stream
                );
                if (result.IsSuccessful)
                {
                    Console.WriteLine($"File Saved to: {result.FilePath}");
                }
                else
                {
                    Console.WriteLine("Saving canceled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saving of the file failed: {ex.Message}");
            }
        }

        public async Task SaveFileWithDialogAsync(List<string> lines, string defaultFileName = "ConsoleOutput.txt", bool addTimestampPrefix = true)
        {
            try
            {
                var content = string.Join(Environment.NewLine, lines);
                var fileBytes = Encoding.UTF8.GetBytes(content);
                using var stream = new MemoryStream(fileBytes);

                defaultFileName = addTimestampPrefix
                    ? $"{DateTime.Now:yyyyMMddHHmmss}_{defaultFileName}"
                    : defaultFileName;

                var result = await FileSaver.Default.SaveAsync(
                    "",
                    defaultFileName,
                    stream
                );

                if (result.IsSuccessful)
                {
                    Console.WriteLine($"File Saved to: {result.FilePath}");
                }
                else
                {
                    Console.WriteLine("Saving canceled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saving of the file failed: {ex.Message}");
            }
        }
    }
}
