using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DeviceSpecs
{
    /// <summary>
    /// Extension methods for registering DeviceSpecs SDK client in the <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DeviceSpecsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDeviceSpecsClient"/> and its dependencies to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">An action to configure the <see cref="DeviceSpecsOptions"/>.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configureOptions"/> is null.</exception>
        public static IServiceCollection AddDeviceSpecsClient(
            this IServiceCollection services,
            Action<DeviceSpecsOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new DeviceSpecsOptions();
            configureOptions(options);

            // Register Options as a singleton
            services.TryAddSingleton(options);

            // Register DeviceSpecsClient as a typed HttpClient.
            // This integrates with IHttpClientFactory for efficient connection pooling.
            services.AddHttpClient<IDeviceSpecsClient, DeviceSpecsClient>();

            return services;
        }
    }
}
