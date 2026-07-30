using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DeviceSpecs.Exceptions;
using DeviceSpecs.Models;

namespace DeviceSpecs
{
    /// <summary>
    /// Implementation of the <see cref="IDeviceSpecsClient"/> for interacting with the DeviceSpecs API.
    /// </summary>
    public class DeviceSpecsClient : IDeviceSpecsClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly DeviceSpecsOptions _options;
        private readonly bool _disposeHttpClient;

        // Constants to avoid string hardcoding
        private const string ApiKeyHeader = "x-rapidapi-key";
        private const string ApiHostHeader = "x-rapidapi-host";

        private const string DevicesPath = "api/values/clean/devices";
        private const string DevicesByManufacturerPath = "api/values/clean/getdevices";
        private const string DevicesByChipsetPath = "api/values/clean/devicesbychipset";
        private const string GetSpecsPath = "api/values/clean/getspecs";
        private const string DeviceByModelNumberPath = "api/values/clean/devicebymodelnumber";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSpecsClient"/> class using a provided <see cref="HttpClient"/> and configuration.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use for requests.</param>
        /// <param name="options">The configuration options.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
        public DeviceSpecsClient(HttpClient httpClient, DeviceSpecsOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _disposeHttpClient = false;

            ConfigureHttpClient(_httpClient, _options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSpecsClient"/> class with specific options.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        public DeviceSpecsClient(DeviceSpecsOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = new HttpClient();
            _disposeHttpClient = true;

            ConfigureHttpClient(_httpClient, _options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSpecsClient"/> class with an API key and default gateway.
        /// </summary>
        /// <param name="apiKey">The DeviceSpecs API Key obtained from https://ds.gtgroup.dev.</param>
        /// <exception cref="ArgumentException">Thrown when API key is null or empty.</exception>
        public DeviceSpecsClient(string apiKey)
            : this(new DeviceSpecsOptions { ApiKey = apiKey })
        {
        }

        private static void ConfigureHttpClient(HttpClient client, DeviceSpecsOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                throw new ArgumentException("API key cannot be null or empty. Please get a free key or buy a plan at https://ds.gtgroup.dev", nameof(options));
            }

            if (client.BaseAddress == null)
            {
                var baseUrl = options.BaseUrl;
                if (!baseUrl.EndsWith("/"))
                {
                    baseUrl += "/";
                }
                client.BaseAddress = new Uri(baseUrl);
            }

            if (!client.DefaultRequestHeaders.Contains(ApiKeyHeader))
            {
                client.DefaultRequestHeaders.Add(ApiKeyHeader, options.ApiKey);
            }

            if (!client.DefaultRequestHeaders.Contains(ApiHostHeader))
            {
                client.DefaultRequestHeaders.Add(ApiHostHeader, options.ApiHost);
            }
        }

        /// <inheritdoc />
        public DeviceQueryBuilder Where()
        {
            return new DeviceQueryBuilder(this);
        }

        /// <inheritdoc />
        public DeviceQueryBuilder Query()
        {
            return new DeviceQueryBuilder(this);
        }

        /// <inheritdoc />
        public Task<List<DeviceSpecsResponse>> GetDevicesAsync(
            int? batteryGte = null,
            string? chipsetContains = null,
            int? minRamGb = null,
            int? maxRamGb = null,
            CancellationToken cancellationToken = default)
        {
            var builder = Query();

            if (batteryGte.HasValue)
            {
                builder.WhereBatteryCapacityGte(batteryGte.Value);
            }

            if (!string.IsNullOrWhiteSpace(chipsetContains))
            {
                builder.WhereChipsetContains(chipsetContains);
            }

            if (minRamGb.HasValue && maxRamGb.HasValue)
            {
                builder.WhereRamGbBetween(minRamGb.Value, maxRamGb.Value);
            }
            else if (minRamGb.HasValue)
            {
                builder.WhereRamGbGte(minRamGb.Value);
            }
            else if (maxRamGb.HasValue)
            {
                builder.WhereRamGbLte(maxRamGb.Value);
            }

            return GetDevicesAsync(builder, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<DeviceSpecsResponse>> GetDevicesAsync(
            DeviceQueryBuilder query,
            CancellationToken cancellationToken = default)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var queryString = query.ToQueryString();
            var relativeUrl = $"{DevicesPath}{queryString}";
            return await SendRequestAsync<List<DeviceSpecsResponse>>(relativeUrl, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<List<DeviceSpecsResponse>> GetDevicesByManufacturerAsync(
            string manufacturer,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                throw new ArgumentException("Manufacturer name cannot be null or empty.", nameof(manufacturer));
            }

            var relativeUrl = $"{DevicesByManufacturerPath}/{Uri.EscapeDataString(manufacturer)}";
            return await SendRequestAsync<List<DeviceSpecsResponse>>(relativeUrl, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<List<DeviceSpecsResponse>> GetDevicesByChipsetAsync(
            string chipset,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(chipset))
            {
                throw new ArgumentException("Chipset name cannot be null or empty.", nameof(chipset));
            }

            var relativeUrl = $"{DevicesByChipsetPath}/{Uri.EscapeDataString(chipset.Trim())}";
            return await SendRequestAsync<List<DeviceSpecsResponse>>(relativeUrl, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<DeviceSpecsResponse?> GetSpecsAsync(
            string manufacturer,
            string model,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(manufacturer))
            {
                throw new ArgumentException("Manufacturer name cannot be null or empty.", nameof(manufacturer));
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException("Model name cannot be null or empty.", nameof(model));
            }

            var relativeUrl = $"{GetSpecsPath}/{Uri.EscapeDataString(manufacturer.Trim())}/{Uri.EscapeDataString(model.Trim())}";

            try
            {
                return await SendRequestAsync<DeviceSpecsResponse>(relativeUrl, cancellationToken).ConfigureAwait(false);
            }
            catch (DeviceSpecsApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<DeviceSpecsResponse?> GetDeviceByModelNumberAsync(
            string modelNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(modelNumber))
            {
                throw new ArgumentException("Model number cannot be null or empty.", nameof(modelNumber));
            }

            var relativeUrl = $"{DeviceByModelNumberPath}/{Uri.EscapeDataString(modelNumber.Trim())}";

            try
            {
                return await SendRequestAsync<DeviceSpecsResponse>(relativeUrl, cancellationToken).ConfigureAwait(false);
            }
            catch (DeviceSpecsApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private async Task<T> SendRequestAsync<T>(string relativeUrl, CancellationToken cancellationToken)
        {
            using (var response = await _httpClient.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false))
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new DeviceSpecsApiException(
                        response.StatusCode,
                        responseContent,
                        $"DeviceSpecs API returned non-success status code {(int)response.StatusCode} ({response.ReasonPhrase}). Response content: {responseContent}");
                }

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var deserialized = JsonSerializer.Deserialize<T>(responseContent, options);
                    if (deserialized == null)
                    {
                        throw new DeviceSpecsApiException(
                            response.StatusCode,
                            responseContent,
                            "Deserialization returned null response content.");
                    }
                    return deserialized;
                }
                catch (JsonException ex)
                {
                    throw new DeviceSpecsApiException(
                        response.StatusCode,
                        responseContent,
                        "Failed to deserialize response body.",
                        ex);
                }
            }
        }

        /// <summary>
        /// Disposes the client resources. Disposes the underlying <see cref="HttpClient"/> if it was created internally.
        /// </summary>
        public void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
        }
    }
}
