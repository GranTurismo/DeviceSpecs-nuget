using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeviceSpecs.Models;

namespace DeviceSpecs
{
    /// <summary>
    /// Provides client-side access to the DeviceSpecs API to query mobile phone specifications, chipsets, and physical metrics.
    /// </summary>
    public interface IDeviceSpecsClient
    {
        /// <summary>
        /// Retrieves a list of mobile devices matching the specified filter parameters.
        /// </summary>
        /// <param name="batteryGte">Filter devices with battery capacity greater than or equal to this value (in mAh).</param>
        /// <param name="chipsetContains">Substring match for the chipset name (e.g. "Snapdragon", "Tensor").</param>
        /// <param name="minRamGb">Minimum RAM capacity filter (in GB). Combined with maxRamGb to form a range.</param>
        /// <param name="maxRamGb">Maximum RAM capacity filter (in GB). Combined with minRamGb to form a range.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of device specifications matching the criteria.</returns>
        Task<List<DeviceSpecsResponse>> GetDevicesAsync(
            int? batteryGte = null,
            string? chipsetContains = null,
            int? minRamGb = null,
            int? maxRamGb = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all tracked mobile devices from a specific manufacturer.
        /// </summary>
        /// <param name="manufacturer">The name of the manufacturer (e.g. "Google", "Samsung", "Apple").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of device specifications for the manufacturer.</returns>
        Task<List<DeviceSpecsResponse>> GetDevicesByManufacturerAsync(
            string manufacturer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all mobile devices powered by a specific SoC/chipset.
        /// </summary>
        /// <param name="chipset">The URL-friendly chipset name (e.g. "Tensor-G4", "Snapdragon-8-Gen-3").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of devices powered by the specified chipset.</returns>
        Task<List<DeviceSpecsResponse>> GetDevicesByChipsetAsync(
            string chipset,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the detailed raw and normalized specifications for a specific device model.
        /// </summary>
        /// <param name="manufacturer">The name of the manufacturer (e.g. "Google").</param>
        /// <param name="model">The model name of the device (e.g. "Pixel-9-Pro").</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The detailed specifications of the specified device, or null if the device was not found.</returns>
        Task<DeviceSpecsResponse?> GetSpecsAsync(
            string manufacturer,
            string model,
            CancellationToken cancellationToken = default);
    }
}
