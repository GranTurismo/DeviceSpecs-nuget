using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceSpecs.Models;

namespace DeviceSpecs
{
    /// <summary>
    /// A fluent query builder for constructing deep, multi-metric device filter requests for the DeviceSpecs API.
    /// </summary>
    public class DeviceQueryBuilder
    {
        private readonly IDeviceSpecsClient? _client;
        private readonly Dictionary<string, string> _queryParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceQueryBuilder"/> class without a client (standalone query builder).
        /// </summary>
        public DeviceQueryBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceQueryBuilder"/> class attached to a specific <see cref="IDeviceSpecsClient"/>.
        /// </summary>
        /// <param name="client">The client used to execute the query.</param>
        public DeviceQueryBuilder(IDeviceSpecsClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Fluent chaining method that returns this query builder instance (e.g. client.Where().ManufacturerIs("Samsung")).
        /// </summary>
        public DeviceQueryBuilder Where() => this;

        /// <summary>
        /// Adds a raw query parameter or custom filter key and value to the query.
        /// </summary>
        /// <param name="key">The filter property key (e.g. "ram", "display.refresh_rate_hz").</param>
        /// <param name="value">The raw string value of the parameter.</param>
        /// <returns>The updated <see cref="DeviceQueryBuilder"/> instance.</returns>
        public DeviceQueryBuilder WithCustomFilter(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(key) && value != null)
            {
                _queryParams[key.Trim()] = value;
            }
            return this;
        }

        /// <summary>
        /// Adds a raw query parameter with a specific operator (e.g. "gt", "gte", "lt", "lte", "contains", "in", "between", "has", "eq").
        /// </summary>
        /// <param name="propertyKey">The target property name (e.g. "ram", "price_usd").</param>
        /// <param name="op">The filter operator (e.g. "gte", "lte", "contains", "between").</param>
        /// <param name="value">The value to filter against.</param>
        /// <returns>The updated <see cref="DeviceQueryBuilder"/> instance.</returns>
        public DeviceQueryBuilder WithCustomFilter(string propertyKey, string op, object? value)
        {
            if (string.IsNullOrWhiteSpace(propertyKey) || value == null) return this;

            string formattedValue = ConvertToString(value);
            string paramKey = string.IsNullOrWhiteSpace(op) || op.Equals("eq", StringComparison.OrdinalIgnoreCase)
                ? propertyKey.Trim()
                : $"{propertyKey.Trim()}_{op.Trim()}";

            _queryParams[paramKey] = formattedValue;
            return this;
        }

        private static string ConvertToString(object value)
        {
            if (value is bool b) return b ? "true" : "false";
            if (value is double d) return d.ToString(CultureInfo.InvariantCulture);
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is decimal dec) return dec.ToString(CultureInfo.InvariantCulture);
            return value.ToString() ?? string.Empty;
        }

        // ==========================================
        //  General / Root Metrics
        // ==========================================

        /// <summary>Filters by exact manufacturer name (e.g., Brands.Samsung, Brands.Apple).</summary>
        public DeviceQueryBuilder ManufacturerIs(string? manufacturer)
            => WithCustomFilter("manufacturer", "eq", manufacturer);

        /// <summary>Filters devices matching any of the specified manufacturer names.</summary>
        public DeviceQueryBuilder ManufacturerIn(params string[] manufacturers)
            => WithCustomFilter("manufacturer", "in", string.Join(",", manufacturers));

        /// <summary>Filters devices matching any of the specified manufacturer names (alias for ManufacturerIn).</summary>
        public DeviceQueryBuilder ManufacturerBetween(params string[] manufacturers)
            => ManufacturerIn(manufacturers);

        /// <summary>Substring filter on manufacturer name.</summary>
        public DeviceQueryBuilder ManufacturerContains(string? manufacturerSubstring)
            => WithCustomFilter("manufacturer", "contains", manufacturerSubstring);

        /// <summary>Exact filter on model name.</summary>
        public DeviceQueryBuilder ModelIs(string? model)
            => WithCustomFilter("model", "eq", model);

        /// <summary>Substring filter on model name (e.g., "Pixel", "Galaxy S24").</summary>
        public DeviceQueryBuilder ModelContains(string? modelSubstring)
            => WithCustomFilter("model", "contains", modelSubstring);

        /// <summary>Exact chipset match filter.</summary>
        public DeviceQueryBuilder ChipsetIs(string? chipset)
            => WithCustomFilter("chipset", "eq", chipset);

        /// <summary>Substring chipset filter (e.g., "Snapdragon", "Tensor", "Dimensity").</summary>
        public DeviceQueryBuilder ChipsetContains(string? chipsetSubstring)
            => WithCustomFilter("chipset", "contains", chipsetSubstring);

        /// <summary>Filters by exact Android OS version.</summary>
        public DeviceQueryBuilder AndroidVersionIs(string? version)
            => WithCustomFilter("androidversion", "eq", version);

        /// <summary>Filters devices with Android version greater than or equal to <paramref name="version"/>.</summary>
        public DeviceQueryBuilder AndroidVersionGreaterThanOrEqual(string? version)
            => WithCustomFilter("androidversion", "gte", version);

        /// <summary>Filters devices with price (USD) exactly equal to <paramref name="priceUsd"/>.</summary>
        public DeviceQueryBuilder PriceUsdIs(double priceUsd)
            => WithCustomFilter("price_usd", "eq", priceUsd);

        /// <summary>Filters devices with price (USD) strictly greater than <paramref name="minPriceUsd"/>.</summary>
        public DeviceQueryBuilder PriceUsdGreaterThan(double minPriceUsd)
            => WithCustomFilter("price_usd", "gt", minPriceUsd);

        /// <summary>Filters devices with price (USD) greater than or equal to <paramref name="minPriceUsd"/>.</summary>
        public DeviceQueryBuilder PriceUsdGreaterThanOrEqual(double minPriceUsd)
            => WithCustomFilter("price_usd", "gte", minPriceUsd);

        /// <summary>Filters devices with price (USD) strictly less than <paramref name="maxPriceUsd"/>.</summary>
        public DeviceQueryBuilder PriceUsdLessThan(double maxPriceUsd)
            => WithCustomFilter("price_usd", "lt", maxPriceUsd);

        /// <summary>Filters devices with price (USD) less than or equal to <paramref name="maxPriceUsd"/>.</summary>
        public DeviceQueryBuilder PriceUsdLessOrEqual(double maxPriceUsd)
            => WithCustomFilter("price_usd", "lte", maxPriceUsd);

        /// <summary>Filters devices within a price (USD) range.</summary>
        public DeviceQueryBuilder PriceUsdBetween(double minPriceUsd, double maxPriceUsd)
            => WithCustomFilter("price_usd", "between", $"{minPriceUsd.ToString(CultureInfo.InvariantCulture)},{maxPriceUsd.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>Filters devices with price (EUR) less than or equal to <paramref name="maxPriceEur"/>.</summary>
        public DeviceQueryBuilder PriceEurLessOrEqual(double maxPriceEur)
            => WithCustomFilter("price_eur", "lte", maxPriceEur);

        /// <summary>Filters devices with price (GBP) less than or equal to <paramref name="maxPriceGbp"/>.</summary>
        public DeviceQueryBuilder PriceGbpLessOrEqual(double maxPriceGbp)
            => WithCustomFilter("price_gbp", "lte", maxPriceGbp);

        /// <summary>Filters devices available in a specific color.</summary>
        public DeviceQueryBuilder ColorContains(string? color)
            => WithCustomFilter("colors", "contains", color);

        // ==========================================
        //  Display Metrics
        // ==========================================

        /// <summary>Filters display size (in inches) strictly greater than <paramref name="minInches"/>.</summary>
        public DeviceQueryBuilder DisplaySizeGreaterThan(double minInches)
            => WithCustomFilter("display.size_inches", "gt", minInches);

        /// <summary>Filters display size (in inches) greater than or equal to <paramref name="minInches"/>.</summary>
        public DeviceQueryBuilder DisplaySizeGreaterThanOrEqual(double minInches)
            => WithCustomFilter("display.size_inches", "gte", minInches);

        /// <summary>Filters display size (in inches) strictly less than <paramref name="maxInches"/>.</summary>
        public DeviceQueryBuilder DisplaySizeLessThan(double maxInches)
            => WithCustomFilter("display.size_inches", "lt", maxInches);

        /// <summary>Filters display size (in inches) less than or equal to <paramref name="maxInches"/>.</summary>
        public DeviceQueryBuilder DisplaySizeLessOrEqual(double maxInches)
            => WithCustomFilter("display.size_inches", "lte", maxInches);

        /// <summary>Filters display size (in inches) within a range.</summary>
        public DeviceQueryBuilder DisplaySizeBetween(double minInches, double maxInches)
            => WithCustomFilter("display.size_inches", "between", $"{minInches.ToString(CultureInfo.InvariantCulture)},{maxInches.ToString(CultureInfo.InvariantCulture)}");

        /// <summary>Substring filter on panel type (e.g. "OLED", "AMOLED", "IPS").</summary>
        public DeviceQueryBuilder DisplayPanelTypeContains(string? panelType)
            => WithCustomFilter("display.panel_type", "contains", panelType);

        /// <summary>Filters display refresh rate (in Hz) equal to <paramref name="hz"/>.</summary>
        public DeviceQueryBuilder DisplayRefreshRateIs(int hz)
            => WithCustomFilter("display.refresh_rate_hz", "eq", hz);

        /// <summary>Filters display refresh rate (in Hz) strictly greater than <paramref name="minHz"/>.</summary>
        public DeviceQueryBuilder DisplayRefreshRateGreaterThan(int minHz)
            => WithCustomFilter("display.refresh_rate_hz", "gt", minHz);

        /// <summary>Filters display refresh rate (in Hz) greater than or equal to <paramref name="minHz"/> (e.g., 90, 120, 144).</summary>
        public DeviceQueryBuilder DisplayRefreshRateGreaterThanOrEqual(int minHz)
            => WithCustomFilter("display.refresh_rate_hz", "gte", minHz);

        /// <summary>Filters display refresh rate (in Hz) within a range.</summary>
        public DeviceQueryBuilder DisplayRefreshRateBetween(int minHz, int maxHz)
            => WithCustomFilter("display.refresh_rate_hz", "between", $"{minHz},{maxHz}");

        /// <summary>Filters display peak brightness (in nits) greater than or equal to <paramref name="minNits"/> (e.g., 1000, 2000).</summary>
        public DeviceQueryBuilder DisplayBrightnessGreaterThanOrEqual(int minNits)
            => WithCustomFilter("display.peak_brightness_nits", "gte", minNits);

        /// <summary>Substring filter on display glass protection (e.g., "Gorilla Glass", "Victus").</summary>
        public DeviceQueryBuilder DisplayProtectionContains(string? protection)
            => WithCustomFilter("display.protection_type", "contains", protection);

        // ==========================================
        //  Processor Metrics
        // ==========================================

        /// <summary>Filters devices with total CPU cores equal to <paramref name="cores"/>.</summary>
        public DeviceQueryBuilder CpuCoresIs(int cores)
            => WithCustomFilter("processor.total_cores", "eq", cores);

        /// <summary>Filters devices with total CPU cores strictly greater than <paramref name="minCores"/>.</summary>
        public DeviceQueryBuilder CpuCoresGreaterThan(int minCores)
            => WithCustomFilter("processor.total_cores", "gt", minCores);

        /// <summary>Filters devices with total CPU cores greater than or equal to <paramref name="minCores"/>.</summary>
        public DeviceQueryBuilder CpuCoresGreaterThanOrEqual(int minCores)
            => WithCustomFilter("processor.total_cores", "gte", minCores);

        /// <summary>Filters devices with CPU max clock speed (GHz) greater than or equal to <paramref name="minGhz"/>.</summary>
        public DeviceQueryBuilder CpuClockSpeedGreaterThanOrEqual(double minGhz)
            => WithCustomFilter("processor.max_clock_speed_ghz", "gte", minGhz);

        // ==========================================
        //  Memory Metrics
        // ==========================================

        /// <summary>Filters devices with RAM (in GB) exactly equal to <paramref name="ramGb"/>.</summary>
        public DeviceQueryBuilder RamIs(int ramGb)
            => WithCustomFilter("memory.available_ram_gb", "eq", ramGb);

        /// <summary>Filters devices with RAM (in GB) strictly greater than <paramref name="minRamGb"/>.</summary>
        public DeviceQueryBuilder RamGreaterThan(int minRamGb)
            => WithCustomFilter("memory.available_ram_gb", "gt", minRamGb);

        /// <summary>Filters devices with RAM (in GB) greater than or equal to <paramref name="minRamGb"/>.</summary>
        public DeviceQueryBuilder RamGreaterThanOrEqual(int minRamGb)
            => WithCustomFilter("memory.available_ram_gb", "gte", minRamGb);

        /// <summary>Filters devices with RAM (in GB) strictly less than <paramref name="maxRamGb"/>.</summary>
        public DeviceQueryBuilder RamLessThan(int maxRamGb)
            => WithCustomFilter("memory.available_ram_gb", "lt", maxRamGb);

        /// <summary>Filters devices with RAM (in GB) less than or equal to <paramref name="maxRamGb"/>.</summary>
        public DeviceQueryBuilder RamLessOrEqual(int maxRamGb)
            => WithCustomFilter("memory.available_ram_gb", "lte", maxRamGb);

        /// <summary>Filters devices with RAM (in GB) within a range.</summary>
        public DeviceQueryBuilder RamBetween(int minRamGb, int maxRamGb)
            => WithCustomFilter("memory.available_ram_gb", "between", $"{minRamGb},{maxRamGb}");

        /// <summary>Filters devices having RAM matching any of the specified values (in GB).</summary>
        public DeviceQueryBuilder RamIn(params int[] ramOptionsGb)
            => WithCustomFilter("memory.available_ram_gb", "in", string.Join(",", ramOptionsGb));

        /// <summary>Filters devices with storage capacity (in GB) equal to <paramref name="storageGb"/>.</summary>
        public DeviceQueryBuilder StorageIs(int storageGb)
            => WithCustomFilter("memory.available_storage_gb", "eq", storageGb);

        /// <summary>Filters devices with storage capacity (in GB) strictly greater than <paramref name="minStorageGb"/>.</summary>
        public DeviceQueryBuilder StorageGreaterThan(int minStorageGb)
            => WithCustomFilter("memory.available_storage_gb", "gt", minStorageGb);

        /// <summary>Filters devices with storage capacity (in GB) greater than or equal to <paramref name="minStorageGb"/>.</summary>
        public DeviceQueryBuilder StorageGreaterThanOrEqual(int minStorageGb)
            => WithCustomFilter("memory.available_storage_gb", "gte", minStorageGb);

        /// <summary>Filters devices with storage capacity (in GB) strictly less than <paramref name="maxStorageGb"/>.</summary>
        public DeviceQueryBuilder StorageLessThan(int maxStorageGb)
            => WithCustomFilter("memory.available_storage_gb", "lt", maxStorageGb);

        /// <summary>Filters devices with storage capacity (in GB) less than or equal to <paramref name="maxStorageGb"/>.</summary>
        public DeviceQueryBuilder StorageLessOrEqual(int maxStorageGb)
            => WithCustomFilter("memory.available_storage_gb", "lte", maxStorageGb);

        /// <summary>Filters devices with storage capacity (in GB) within a range.</summary>
        public DeviceQueryBuilder StorageBetween(int minStorageGb, int maxStorageGb)
            => WithCustomFilter("memory.available_storage_gb", "between", $"{minStorageGb},{maxStorageGb}");

        /// <summary>Filters devices having storage capacity matching any of the specified values (in GB).</summary>
        public DeviceQueryBuilder StorageIn(params int[] storageOptionsGb)
            => WithCustomFilter("memory.available_storage_gb", "in", string.Join(",", storageOptionsGb));

        /// <summary>Filters devices by expandable storage card slot presence.</summary>
        public DeviceQueryBuilder HasCardSlot(bool hasSlot = true)
            => WithCustomFilter("memory.has_card_slot", "has", hasSlot);

        /// <summary>Filters devices by storage technology (e.g. "UFS 4.0", "UFS 3.1").</summary>
        public DeviceQueryBuilder StorageTechnologyContains(string? tech)
            => WithCustomFilter("memory.storage_technology", "contains", tech);

        // ==========================================
        //  Power & Battery Metrics
        // ==========================================

        /// <summary>Filters devices with battery capacity (in mAh) equal to <paramref name="capacityMah"/>.</summary>
        public DeviceQueryBuilder BatteryIs(int capacityMah)
            => WithCustomFilter("power.capacities_mah", "eq", capacityMah);

        /// <summary>Filters devices with battery capacity (in mAh) strictly greater than <paramref name="minCapacityMah"/>.</summary>
        public DeviceQueryBuilder BatteryGreaterThan(int minCapacityMah)
            => WithCustomFilter("power.capacities_mah", "gt", minCapacityMah);

        /// <summary>Filters devices with battery capacity (in mAh) greater than or equal to <paramref name="minCapacityMah"/>.</summary>
        public DeviceQueryBuilder BatteryGreaterThanOrEqual(int minCapacityMah)
            => WithCustomFilter("power.capacities_mah", "gte", minCapacityMah);

        /// <summary>Filters devices with battery capacity (in mAh) strictly less than <paramref name="maxCapacityMah"/>.</summary>
        public DeviceQueryBuilder BatteryLessThan(int maxCapacityMah)
            => WithCustomFilter("power.capacities_mah", "lt", maxCapacityMah);

        /// <summary>Filters devices with battery capacity (in mAh) less than or equal to <paramref name="maxCapacityMah"/>.</summary>
        public DeviceQueryBuilder BatteryLessOrEqual(int maxCapacityMah)
            => WithCustomFilter("power.capacities_mah", "lte", maxCapacityMah);

        /// <summary>Filters devices with battery capacity (in mAh) within a range.</summary>
        public DeviceQueryBuilder BatteryBetween(int minMah, int maxMah)
            => WithCustomFilter("power.capacities_mah", "between", $"{minMah},{maxMah}");

        /// <summary>Filters devices with maximum wired charging power (in Watts) greater than or equal to <paramref name="minWatts"/>.</summary>
        public DeviceQueryBuilder WiredChargingGreaterThanOrEqual(int minWatts)
            => WithCustomFilter("power.max_wired_charging_w", "gte", minWatts);

        /// <summary>Filters devices by wireless charging presence.</summary>
        public DeviceQueryBuilder HasWirelessCharging(bool hasWireless = true)
            => WithCustomFilter("power.has_wireless_charging", "has", hasWireless);

        // ==========================================
        //  Camera Metrics
        // ==========================================

        /// <summary>Filters devices with main camera lens count greater than or equal to <paramref name="minLenses"/>.</summary>
        public DeviceQueryBuilder MainCameraLensesGreaterThanOrEqual(int minLenses)
            => WithCustomFilter("cameras.main_lenses_count", "gte", minLenses);

        /// <summary>Filters devices with primary camera max resolution (in MP) strictly greater than <paramref name="minMp"/>.</summary>
        public DeviceQueryBuilder MainCameraResolutionGreaterThan(int minMp)
            => WithCustomFilter("cameras.max_main_resolution_mp", "gt", minMp);

        /// <summary>Filters devices with primary camera max resolution (in MP) greater than or equal to <paramref name="minMp"/> (e.g., 50, 108, 200).</summary>
        public DeviceQueryBuilder MainCameraResolutionGreaterThanOrEqual(int minMp)
            => WithCustomFilter("cameras.max_main_resolution_mp", "gte", minMp);

        /// <summary>Filters devices with selfie camera lens count greater than or equal to <paramref name="minLenses"/>.</summary>
        public DeviceQueryBuilder SelfieCameraLensesGreaterThanOrEqual(int minLenses)
            => WithCustomFilter("cameras.selfie_lenses_count", "gte", minLenses);

        // ==========================================
        //  Physical Metrics
        // ==========================================

        /// <summary>Filters devices with total weight (in grams) strictly less than <paramref name="maxWeightGrams"/>.</summary>
        public DeviceQueryBuilder WeightGramsLessThan(int maxWeightGrams)
            => WithCustomFilter("physical.weight_g", "lt", maxWeightGrams);

        /// <summary>Filters devices with total weight (in grams) less than or equal to <paramref name="maxWeightGrams"/>.</summary>
        public DeviceQueryBuilder WeightGramsLessOrEqual(int maxWeightGrams)
            => WithCustomFilter("physical.weight_g", "lte", maxWeightGrams);

        /// <summary>Exact filter on IP rating.</summary>
        public DeviceQueryBuilder IpRatingIs(string? ipRating)
            => WithCustomFilter("physical.ip_rating", "eq", ipRating);

        /// <summary>Substring filter on IP ingress rating (e.g. "IP68", "IP67").</summary>
        public DeviceQueryBuilder IpRatingContains(string? ipRating)
            => WithCustomFilter("physical.ip_rating", "contains", ipRating);

        /// <summary>Filters devices by water resistance specification.</summary>
        public DeviceQueryBuilder IsWaterResistant(bool isWaterResistant = true)
            => WithCustomFilter("physical.is_water_resistant", "has", isWaterResistant);

        /// <summary>Filters devices by water resistance specification (alias for IsWaterResistant).</summary>
        public DeviceQueryBuilder HasWaterResistance(bool isWaterResistant = true)
            => IsWaterResistant(isWaterResistant);

        /// <summary>Substring filter on build materials (e.g. "Glass", "Titanium", "Aluminum").</summary>
        public DeviceQueryBuilder BuildMaterialContains(string? material)
            => WithCustomFilter("physical.build_materials", "contains", material);

        /// <summary>Substring filter on SIM options (e.g. "eSIM", "Dual SIM").</summary>
        public DeviceQueryBuilder SimOptionContains(string? simOption)
            => WithCustomFilter("physical.sim_options", "contains", simOption);

        // ==========================================
        //  Sound Metrics
        // ==========================================

        /// <summary>Filters devices by stereo speakers presence.</summary>
        public DeviceQueryBuilder HasStereoSpeakers(bool hasStereo = true)
            => WithCustomFilter("sound.has_stereo_speakers", "has", hasStereo);

        /// <summary>Filters devices by 3.5mm headphone jack presence.</summary>
        public DeviceQueryBuilder Has35mmJack(bool hasJack = true)
            => WithCustomFilter("sound.has_jack_35mm", "has", hasJack);

        /// <summary>Filters devices by 3.5mm headphone jack presence (alias for Has35mmJack).</summary>
        public DeviceQueryBuilder HasHeadphoneJack(bool hasJack = true)
            => Has35mmJack(hasJack);

        // ==========================================
        //  Connectivity Metrics
        // ==========================================

        /// <summary>Filters devices with Bluetooth version greater than or equal to <paramref name="minVersion"/> (e.g. 5.3, 5.4).</summary>
        public DeviceQueryBuilder BluetoothVersionGreaterThanOrEqual(double minVersion)
            => WithCustomFilter("connectivity.bluetooth_version", "gte", minVersion);

        /// <summary>Filters devices by NFC presence.</summary>
        public DeviceQueryBuilder HasNfc(bool hasNfc = true)
            => WithCustomFilter("connectivity.has_nfc", "has", hasNfc);

        /// <summary>Filters devices by Infrared (IR Blaster) presence.</summary>
        public DeviceQueryBuilder HasInfrared(bool hasInfrared = true)
            => WithCustomFilter("connectivity.has_infrared", "has", hasInfrared);

        /// <summary>Filters devices by FM radio presence.</summary>
        public DeviceQueryBuilder HasRadio(bool hasRadio = true)
            => WithCustomFilter("connectivity.has_radio", "has", hasRadio);

        /// <summary>Filters devices by USB OTG support.</summary>
        public DeviceQueryBuilder HasUsbOtg(bool hasOtg = true)
            => WithCustomFilter("connectivity.usb_has_otg", "has", hasOtg);

        /// <summary>Filters devices by USB OTG support (alias for HasUsbOtg).</summary>
        public DeviceQueryBuilder UsbHasOtg(bool hasOtg = true)
            => HasUsbOtg(hasOtg);

        /// <summary>Substring filter on USB connector type (e.g. "Type-C 3.2", "Type-C 2.0").</summary>
        public DeviceQueryBuilder UsbTypeContains(string? usbType)
            => WithCustomFilter("connectivity.usb_type", "contains", usbType);

        // ==========================================
        //  Benchmarks & EU Label Metrics
        // ==========================================

        /// <summary>Substring or minimum match on AnTuTu score.</summary>
        public DeviceQueryBuilder AntutuScoreGreaterThanOrEqual(int minScore)
            => WithCustomFilter("benchmarks.antutu_score", "gte", minScore);

        /// <summary>Substring or minimum match on GeekBench score.</summary>
        public DeviceQueryBuilder GeekbenchScoreGreaterThanOrEqual(int minScore)
            => WithCustomFilter("benchmarks.geekbench_score", "gte", minScore);

        /// <summary>Filters devices by EU energy efficiency class (e.g. "A", "B").</summary>
        public DeviceQueryBuilder EuEnergyClassIs(string? energyClass)
            => WithCustomFilter("eulabel.energy_class", "eq", energyClass);

        /// <summary>Filters devices by EU repairability index class (e.g. "A", "B", "C").</summary>
        public DeviceQueryBuilder EuRepairabilityClassIs(string? repairabilityClass)
            => WithCustomFilter("eulabel.repairability_class", "eq", repairabilityClass);

        // ==========================================
        //  Backward-Compatible Aliases (Where... prefix)
        // ==========================================

        /// <summary>Alias for ManufacturerIs.</summary>
        public DeviceQueryBuilder WhereManufacturer(string? manufacturer) => ManufacturerIs(manufacturer);

        /// <summary>Alias for ManufacturerIn.</summary>
        public DeviceQueryBuilder WhereManufacturerIn(params string[] manufacturers) => ManufacturerIn(manufacturers);

        /// <summary>Alias for ModelContains.</summary>
        public DeviceQueryBuilder WhereModelContains(string? modelSubstring) => ModelContains(modelSubstring);

        /// <summary>Alias for ChipsetIs.</summary>
        public DeviceQueryBuilder WhereChipset(string? chipset) => ChipsetIs(chipset);

        /// <summary>Alias for ChipsetContains.</summary>
        public DeviceQueryBuilder WhereChipsetContains(string? chipsetSubstring) => ChipsetContains(chipsetSubstring);

        /// <summary>Alias for AndroidVersionIs.</summary>
        public DeviceQueryBuilder WhereAndroidVersion(string? version) => AndroidVersionIs(version);

        /// <summary>Alias for PriceUsdGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WherePriceUsdGte(double minPriceUsd) => PriceUsdGreaterThanOrEqual(minPriceUsd);

        /// <summary>Alias for PriceUsdLessOrEqual.</summary>
        public DeviceQueryBuilder WherePriceUsdLte(double maxPriceUsd) => PriceUsdLessOrEqual(maxPriceUsd);

        /// <summary>Alias for PriceUsdBetween.</summary>
        public DeviceQueryBuilder WherePriceUsdBetween(double minPriceUsd, double maxPriceUsd) => PriceUsdBetween(minPriceUsd, maxPriceUsd);

        /// <summary>Alias for PriceEurLessOrEqual.</summary>
        public DeviceQueryBuilder WherePriceEurLte(double maxPriceEur) => PriceEurLessOrEqual(maxPriceEur);

        /// <summary>Alias for PriceGbpLessOrEqual.</summary>
        public DeviceQueryBuilder WherePriceGbpLte(double maxPriceGbp) => PriceGbpLessOrEqual(maxPriceGbp);

        /// <summary>Alias for ColorContains.</summary>
        public DeviceQueryBuilder WhereColor(string? color) => ColorContains(color);

        /// <summary>Alias for DisplaySizeGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereDisplaySizeGte(double minInches) => DisplaySizeGreaterThanOrEqual(minInches);

        /// <summary>Alias for DisplaySizeLessOrEqual.</summary>
        public DeviceQueryBuilder WhereDisplaySizeLte(double maxInches) => DisplaySizeLessOrEqual(maxInches);

        /// <summary>Alias for DisplaySizeBetween.</summary>
        public DeviceQueryBuilder WhereDisplaySizeBetween(double minInches, double maxInches) => DisplaySizeBetween(minInches, maxInches);

        /// <summary>Alias for DisplayPanelTypeContains.</summary>
        public DeviceQueryBuilder WhereDisplayPanelTypeContains(string? panelType) => DisplayPanelTypeContains(panelType);

        /// <summary>Alias for DisplayRefreshRateGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereDisplayRefreshRateGte(int minHz) => DisplayRefreshRateGreaterThanOrEqual(minHz);

        /// <summary>Alias for DisplayBrightnessGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereDisplayBrightnessGte(int minNits) => DisplayBrightnessGreaterThanOrEqual(minNits);

        /// <summary>Alias for DisplayProtectionContains.</summary>
        public DeviceQueryBuilder WhereDisplayProtectionContains(string? protection) => DisplayProtectionContains(protection);

        /// <summary>Alias for CpuCoresGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereCpuCoresGte(int minCores) => CpuCoresGreaterThanOrEqual(minCores);

        /// <summary>Alias for CpuClockSpeedGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereCpuClockSpeedGte(double minGhz) => CpuClockSpeedGreaterThanOrEqual(minGhz);

        /// <summary>Alias for RamGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereRamGbGte(int minRamGb) => RamGreaterThanOrEqual(minRamGb);

        /// <summary>Alias for RamLessOrEqual.</summary>
        public DeviceQueryBuilder WhereRamGbLte(int maxRamGb) => RamLessOrEqual(maxRamGb);

        /// <summary>Alias for RamBetween.</summary>
        public DeviceQueryBuilder WhereRamGbBetween(int minRamGb, int maxRamGb) => RamBetween(minRamGb, maxRamGb);

        /// <summary>Alias for RamIn.</summary>
        public DeviceQueryBuilder WhereRamGbIn(params int[] ramOptionsGb) => RamIn(ramOptionsGb);

        /// <summary>Alias for StorageGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereStorageGbGte(int minStorageGb) => StorageGreaterThanOrEqual(minStorageGb);

        /// <summary>Alias for StorageBetween.</summary>
        public DeviceQueryBuilder WhereStorageGbBetween(int minStorageGb, int maxStorageGb) => StorageBetween(minStorageGb, maxStorageGb);

        /// <summary>Alias for HasCardSlot.</summary>
        public DeviceQueryBuilder WhereHasCardSlot(bool hasSlot = true) => HasCardSlot(hasSlot);

        /// <summary>Alias for StorageTechnologyContains.</summary>
        public DeviceQueryBuilder WhereStorageTechnology(string? tech) => StorageTechnologyContains(tech);

        /// <summary>Alias for BatteryGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereBatteryCapacityGte(int minCapacityMah) => BatteryGreaterThanOrEqual(minCapacityMah);

        /// <summary>Alias for BatteryBetween.</summary>
        public DeviceQueryBuilder WhereBatteryCapacityBetween(int minMah, int maxMah) => BatteryBetween(minMah, maxMah);

        /// <summary>Alias for WiredChargingGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereWiredChargingGte(int minWatts) => WiredChargingGreaterThanOrEqual(minWatts);

        /// <summary>Alias for HasWirelessCharging.</summary>
        public DeviceQueryBuilder WhereHasWirelessCharging(bool hasWireless = true) => HasWirelessCharging(hasWireless);

        /// <summary>Alias for MainCameraLensesGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereMainCameraLensesGte(int minLenses) => MainCameraLensesGreaterThanOrEqual(minLenses);

        /// <summary>Alias for MainCameraResolutionGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereMainCameraResolutionGte(int minMp) => MainCameraResolutionGreaterThanOrEqual(minMp);

        /// <summary>Alias for SelfieCameraLensesGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereSelfieCameraLensesGte(int minLenses) => SelfieCameraLensesGreaterThanOrEqual(minLenses);

        /// <summary>Alias for WeightGramsLessOrEqual.</summary>
        public DeviceQueryBuilder WhereWeightGramsLte(int maxWeightGrams) => WeightGramsLessOrEqual(maxWeightGrams);

        /// <summary>Alias for IpRatingContains.</summary>
        public DeviceQueryBuilder WhereIpRatingContains(string? ipRating) => IpRatingContains(ipRating);

        /// <summary>Alias for IsWaterResistant.</summary>
        public DeviceQueryBuilder WhereIsWaterResistant(bool isWaterResistant = true) => IsWaterResistant(isWaterResistant);

        /// <summary>Alias for BuildMaterialContains.</summary>
        public DeviceQueryBuilder WhereBuildMaterialContains(string? material) => BuildMaterialContains(material);

        /// <summary>Alias for SimOptionContains.</summary>
        public DeviceQueryBuilder WhereSimOptionContains(string? simOption) => SimOptionContains(simOption);

        /// <summary>Alias for HasStereoSpeakers.</summary>
        public DeviceQueryBuilder WhereHasStereoSpeakers(bool hasStereo = true) => HasStereoSpeakers(hasStereo);

        /// <summary>Alias for Has35mmJack.</summary>
        public DeviceQueryBuilder WhereHas35mmJack(bool hasJack = true) => Has35mmJack(hasJack);

        /// <summary>Alias for BluetoothVersionGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereBluetoothVersionGte(double minVersion) => BluetoothVersionGreaterThanOrEqual(minVersion);

        /// <summary>Alias for HasNfc.</summary>
        public DeviceQueryBuilder WhereHasNfc(bool hasNfc = true) => HasNfc(hasNfc);

        /// <summary>Alias for HasInfrared.</summary>
        public DeviceQueryBuilder WhereHasInfrared(bool hasInfrared = true) => HasInfrared(hasInfrared);

        /// <summary>Alias for HasRadio.</summary>
        public DeviceQueryBuilder WhereHasRadio(bool hasRadio = true) => HasRadio(hasRadio);

        /// <summary>Alias for HasUsbOtg.</summary>
        public DeviceQueryBuilder WhereUsbHasOtg(bool hasOtg = true) => HasUsbOtg(hasOtg);

        /// <summary>Alias for UsbTypeContains.</summary>
        public DeviceQueryBuilder WhereUsbTypeContains(string? usbType) => UsbTypeContains(usbType);

        /// <summary>Alias for AntutuScoreGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereAntutuScoreGte(int minScore) => AntutuScoreGreaterThanOrEqual(minScore);

        /// <summary>Alias for GeekbenchScoreGreaterThanOrEqual.</summary>
        public DeviceQueryBuilder WhereGeekbenchScoreGte(int minScore) => GeekbenchScoreGreaterThanOrEqual(minScore);

        /// <summary>Alias for EuEnergyClassIs.</summary>
        public DeviceQueryBuilder WhereEuEnergyClass(string? energyClass) => EuEnergyClassIs(energyClass);

        /// <summary>Alias for EuRepairabilityClassIs.</summary>
        public DeviceQueryBuilder WhereEuRepairabilityClass(string? repairabilityClass) => EuRepairabilityClassIs(repairabilityClass);

        // ==========================================
        //  Helper Methods & Execution
        // ==========================================

        /// <summary>
        /// Returns all accumulated query parameters as a key-value dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, string> GetParameters() => _queryParams;

        /// <summary>
        /// Converts the accumulated query parameters into a URL relative query string (e.g., "?manufacturer=Samsung&amp;ram_gte=8").
        /// </summary>
        public string ToQueryString()
        {
            if (_queryParams.Count == 0) return string.Empty;

            var list = _queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            return "?" + string.Join("&", list);
        }

        /// <summary>
        /// Executes the query using the attached <see cref="IDeviceSpecsClient"/> and returns matching device specifications.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of device specifications matching the filter criteria.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the query builder was initialized without an attached <see cref="IDeviceSpecsClient"/>.</exception>
        public Task<List<DeviceSpecsResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("Cannot execute query because no IDeviceSpecsClient instance was supplied to this DeviceQueryBuilder. Pass an IDeviceSpecsClient instance to constructor or use client.Where().");
            }

            return _client.GetDevicesAsync(this, cancellationToken);
        }
    }
}
