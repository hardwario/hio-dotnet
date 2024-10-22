using hio_dotnet.HWDrivers.MCU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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

        public static async Task DownloadFirmwareByHashAsync(string hash, string savePath)
        {
            try
            {
                Console.WriteLine("Downloading firmware...");

                var url = $"https://firmware.hardwario.com/chester/{hash}/hex";

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

        public async static Task<FirmwareInfo?> GetFirmwareInfoAsync(string hash)
        {
            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentException("Firmware hash cannot be empty.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri("https://firmware.hardwario.com/chester/");

                var url = $"api/v1/firmware/{hash}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while fetching attributes. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        throw new Exception("An error occurred while fetching attributes. Response is empty.");
                    }
                    else
                    {
                        var fwinfo = JsonSerializer.Deserialize<FirmwareInfo>(responseBody) ?? null;
                        fwinfo?.DeserializeManifest();
                        //Console.WriteLine($"Firmware info: {responseBody}");
                        return fwinfo;
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching attributes.", ex);
                }
            }
        }
    }
}
