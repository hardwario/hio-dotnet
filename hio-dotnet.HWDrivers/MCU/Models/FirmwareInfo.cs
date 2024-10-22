using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.MCU.Models
{
    public class FirmwareInfo
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; }

        [JsonPropertyName("git_revision")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GitRevision { get; set; }

        [JsonPropertyName("firmware_sha256")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FirmwareSha256 { get; set; }

        [JsonPropertyName("app_update_sha256")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AppUpdateSha256 { get; set; }

        [JsonPropertyName("manifest")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ManifestString { get; set; }

        [JsonIgnore]
        public Manifest? Manifest { get; set; }

        [JsonPropertyName("zephyr_elf_sha256")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ZephyrElfSha256 { get; set; }

        public void DeserializeManifest()
        {
            if (!string.IsNullOrEmpty(ManifestString))
            {
                Manifest = JsonSerializer.Deserialize<Manifest>(ManifestString);
            }
        }
    }
}
