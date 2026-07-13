using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeviceSpecs.Models
{
    /// <summary>
    /// Represents the complete details and specifications of a mobile device.
    /// </summary>
    public class DeviceSpecsResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("network")]
        public string Network { get; set; } = string.Empty;

        [JsonPropertyName("announceDate")]
        public string AnnounceDate { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("chipset")]
        public string Chipset { get; set; } = string.Empty;

        [JsonPropertyName("androidVersion")]
        public string AndroidVersion { get; set; } = string.Empty;

        [JsonPropertyName("battery_raw")]
        public string BatteryRaw { get; set; } = string.Empty;

        [JsonPropertyName("cpu_raw")]
        public string CpuRaw { get; set; } = string.Empty;

        [JsonPropertyName("displayResolution_raw")]
        public string DisplayResolutionRaw { get; set; } = string.Empty;

        [JsonPropertyName("displaySize_raw")]
        public string DisplaySizeRaw { get; set; } = string.Empty;

        [JsonPropertyName("displayType_raw")]
        public string DisplayTypeRaw { get; set; } = string.Empty;

        [JsonPropertyName("gpu")]
        public string Gpu { get; set; } = string.Empty;

        [JsonPropertyName("internal_raw")]
        public string InternalRaw { get; set; } = string.Empty;

        [JsonPropertyName("mainCameraFeatures")]
        public string MainCameraFeatures { get; set; } = string.Empty;

        [JsonPropertyName("mainCameraSpecs_raw")]
        public string MainCameraSpecsRaw { get; set; } = string.Empty;

        [JsonPropertyName("mainVideoSpecs")]
        public string MainVideoSpecs { get; set; } = string.Empty;

        [JsonPropertyName("selfieCameraFeatures")]
        public string SelfieCameraFeatures { get; set; } = string.Empty;

        [JsonPropertyName("selfieCameraSpecs_raw")]
        public string SelfieCameraSpecsRaw { get; set; } = string.Empty;

        [JsonPropertyName("selfieVideoSpecs")]
        public string SelfieVideoSpecs { get; set; } = string.Empty;

        [JsonPropertyName("sensors")]
        public string Sensors { get; set; } = string.Empty;

        [JsonPropertyName("charging_raw")]
        public string ChargingRaw { get; set; } = string.Empty;

        [JsonPropertyName("price_usd")]
        public decimal? PriceUsd { get; set; }

        [JsonPropertyName("price_eur")]
        public decimal? PriceEur { get; set; }

        [JsonPropertyName("price_gbp")]
        public decimal? PriceGbp { get; set; }

        [JsonPropertyName("colors")]
        public List<string> Colors { get; set; } = new List<string>();

        [JsonPropertyName("variants")]
        public List<DeviceVariant> Variants { get; set; } = new List<DeviceVariant>();

        [JsonPropertyName("normalized_specs")]
        public NormalizedSpecs? NormalizedSpecs { get; set; }
    }

    /// <summary>
    /// Represents a model variant number of a device.
    /// </summary>
    public class DeviceVariant
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("model_number")]
        public string ModelNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// Parent container for the normalized (parsed) specification metrics of a device.
    /// </summary>
    public class NormalizedSpecs
    {
        [JsonPropertyName("display")]
        public DisplayMetrics? Display { get; set; }

        [JsonPropertyName("processor")]
        public CpuMetrics? Processor { get; set; }

        [JsonPropertyName("memory_options")]
        public MemoryMetrics? MemoryOptions { get; set; }

        [JsonPropertyName("battery_and_charging")]
        public PowerMetrics? BatteryAndCharging { get; set; }

        [JsonPropertyName("cameras")]
        public CameraSummary? Cameras { get; set; }

        [JsonPropertyName("physical")]
        public PhysicalMetrics? Physical { get; set; }

        [JsonPropertyName("sound")]
        public SoundMetrics? Sound { get; set; }

        [JsonPropertyName("connectivity")]
        public ConnectivityMetrics? Connectivity { get; set; }

        [JsonPropertyName("benchmarks")]
        public BenchmarkMetrics? Benchmarks { get; set; }

        [JsonPropertyName("eu_label")]
        public EuLabelMetrics? EuLabel { get; set; }
    }

    /// <summary>
    /// Normalized display dimensions and panel properties.
    /// </summary>
    public class DisplayMetrics
    {
        [JsonPropertyName("size_inches")]
        public double? SizeInches { get; set; }

        [JsonPropertyName("panel_type")]
        public string PanelType { get; set; } = string.Empty;

        [JsonPropertyName("refresh_rate_hz")]
        public int? RefreshRateHz { get; set; }

        [JsonPropertyName("peak_brightness_nits")]
        public int? PeakBrightnessNits { get; set; }

        [JsonPropertyName("protection_type")]
        public string ProtectionType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Normalized central processor specifications.
    /// </summary>
    public class CpuMetrics
    {
        [JsonPropertyName("total_cores")]
        public int? TotalCores { get; set; }

        [JsonPropertyName("max_clock_speed_ghz")]
        public double? MaxClockSpeedGhz { get; set; }
    }

    /// <summary>
    /// Normalized system memory (RAM) and storage information.
    /// </summary>
    public class MemoryMetrics
    {
        [JsonPropertyName("available_ram_gb")]
        public List<int> AvailableRamGb { get; set; } = new List<int>();

        [JsonPropertyName("available_storage_gb")]
        public List<int> AvailableStorageGb { get; set; } = new List<int>();

        [JsonPropertyName("has_card_slot")]
        public bool? HasCardSlot { get; set; }

        [JsonPropertyName("storage_technology")]
        public string StorageTechnology { get; set; } = string.Empty;
    }

    /// <summary>
    /// Normalized battery and charging capabilities.
    /// </summary>
    public class PowerMetrics
    {
        [JsonPropertyName("capacities_mah")]
        public List<int> CapacitiesMah { get; set; } = new List<int>();

        [JsonPropertyName("max_wired_charging_w")]
        public int? MaxWiredChargingW { get; set; }

        [JsonPropertyName("has_wireless_charging")]
        public bool? HasWirelessCharging { get; set; }
    }

    /// <summary>
    /// Summarized camera count and resolutions.
    /// </summary>
    public class CameraSummary
    {
        [JsonPropertyName("main_lenses_count")]
        public int? MainLensesCount { get; set; }

        [JsonPropertyName("max_main_resolution_mp")]
        public int? MaxMainResolutionMp { get; set; }

        [JsonPropertyName("selfie_lenses_count")]
        public int? SelfieLensesCount { get; set; }
    }

    /// <summary>
    /// Normalized physical metrics, build materials, and protection rating.
    /// </summary>
    public class PhysicalMetrics
    {
        [JsonPropertyName("height_mm")]
        public double? HeightMm { get; set; }

        [JsonPropertyName("width_mm")]
        public double? WidthMm { get; set; }

        [JsonPropertyName("thickness_mm")]
        public double? ThicknessMm { get; set; }

        [JsonPropertyName("weight_g")]
        public int? WeightG { get; set; }

        [JsonPropertyName("build_materials")]
        public List<string> BuildMaterials { get; set; } = new List<string>();

        [JsonPropertyName("sim_options")]
        public List<string> SimOptions { get; set; } = new List<string>();

        [JsonPropertyName("ip_rating")]
        public string IpRating { get; set; } = string.Empty;

        [JsonPropertyName("is_water_resistant")]
        public bool? IsWaterResistant { get; set; }
    }

    /// <summary>
    /// Normalized audio setup and capabilities.
    /// </summary>
    public class SoundMetrics
    {
        [JsonPropertyName("has_stereo_speakers")]
        public bool? HasStereoSpeakers { get; set; }

        [JsonPropertyName("has_jack_35mm")]
        public bool? HasJack35Mm { get; set; }

        [JsonPropertyName("audio_quality_features")]
        public List<string> AudioQualityFeatures { get; set; } = new List<string>();
    }

    /// <summary>
    /// Normalized wireless, network, and positioning options.
    /// </summary>
    public class ConnectivityMetrics
    {
        [JsonPropertyName("wlan_features")]
        public List<string> WlanFeatures { get; set; } = new List<string>();

        [JsonPropertyName("bluetooth_version")]
        public double? BluetoothVersion { get; set; }

        [JsonPropertyName("bluetooth_features")]
        public List<string> BluetoothFeatures { get; set; } = new List<string>();

        [JsonPropertyName("positioning_tech")]
        public List<string> PositioningTech { get; set; } = new List<string>();

        [JsonPropertyName("has_nfc")]
        public bool? HasNfc { get; set; }

        [JsonPropertyName("has_infrared")]
        public bool? HasInfrared { get; set; }

        [JsonPropertyName("has_radio")]
        public bool? HasRadio { get; set; }

        [JsonPropertyName("usb_type")]
        public string UsbType { get; set; } = string.Empty;

        [JsonPropertyName("usb_has_otg")]
        public bool? UsbHasOtg { get; set; }
    }

    /// <summary>
    /// Benchmark performance values and battery endurance test results.
    /// </summary>
    public class BenchmarkMetrics
    {
        [JsonPropertyName("antutu_score")]
        public string AntutuScore { get; set; } = string.Empty;

        [JsonPropertyName("geekbench_score")]
        public string GeekbenchScore { get; set; } = string.Empty;

        [JsonPropertyName("measured_display_brightness_nits")]
        public int? MeasuredDisplayBrightnessNits { get; set; }

        [JsonPropertyName("measured_loudspeaker_lufs")]
        public double? MeasuredLoudspeakerLufs { get; set; }

        [JsonPropertyName("measured_battery_active_use_hours")]
        public double? MeasuredBatteryActiveUseHours { get; set; }
    }

    /// <summary>
    /// Standard EU environmental and durability labelling class.
    /// </summary>
    public class EuLabelMetrics
    {
        [JsonPropertyName("energy_class")]
        public string EnergyClass { get; set; } = string.Empty;

        [JsonPropertyName("battery_endurance")]
        public string BatteryEndurance { get; set; } = string.Empty;

        [JsonPropertyName("battery_endurance_hours")]
        public double? BatteryEnduranceHours { get; set; }

        [JsonPropertyName("free_fall_class")]
        public string FreeFallClass { get; set; } = string.Empty;

        [JsonPropertyName("repairability_class")]
        public string RepairabilityClass { get; set; } = string.Empty;
    }
}
