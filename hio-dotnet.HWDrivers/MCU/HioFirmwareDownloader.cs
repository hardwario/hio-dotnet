using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.MCU
{
    public class HioFirmwareDownloader
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task DownloadFirmwareAsync(string url, string savePath)
        {
            try
            {
                Console.WriteLine("Downloading firmware...");

                // Fetch the firmware as a stream
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    // Save the firmware as a .hex file
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }

                Console.WriteLine($"Firmware downloaded and saved to: {savePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while downloading firmware: {ex.Message}");
            }
        }
    }
}
