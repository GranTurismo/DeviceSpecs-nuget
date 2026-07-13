namespace DeviceSpecs
{
    /// <summary>
    /// Configuration options for the DeviceSpecs SDK client.
    /// </summary>
    public class DeviceSpecsOptions
    {
        /// <summary>
        /// Gets or sets the DeviceSpecs API key.
        /// Register and obtain this key from https://ds.gtgroup.dev.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base URL for the DeviceSpecs API.
        /// Defaults to "https://gsmarenaparser.p.rapidapi.com".
        /// </summary>
        public string BaseUrl { get; set; } = "https://gsmarenaparser.p.rapidapi.com";

        /// <summary>
        /// Gets or sets the API host header value.
        /// Defaults to "gsmarenaparser.p.rapidapi.com".
        /// </summary>
        public string ApiHost { get; set; } = "gsmarenaparser.p.rapidapi.com";
    }
}
