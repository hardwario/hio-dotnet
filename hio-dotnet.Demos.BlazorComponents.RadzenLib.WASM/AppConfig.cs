namespace hio_dotnet.Demos.BlazorComponents.RadzenLib
{
    public class AppConfig
    {
        public string AppName { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public bool Debug { get; set; } = false;

        public string HioCloudBaseURL { get; set; } = "https://hardwario.cloud";
        public string ThingsBoardBaseURL { get; set; } = "http://localhost";
        public int ThingsBoardBasePort { get; set; } = 8080;

        public string RemoteServerBaseURL { get; set; } = "http://localhost";
        public int RemoteServerBasePort { get; set; } = 8042;

        public bool UseDefaultLoginForThingsBoard { get; set; } = false;

        public string DefaultLoginForThingsBoard { get; set; } = "tenant@thingsboard.org";
        public string DefaultPasswordForThingsBoard { get; set; } = "tenant";

    }
}
