# DeviceSpecs Client SDK (.NET)

A strongly-typed, high-performance .NET client library for the **DeviceSpecs API**. Access structured, clean, and normalized specifications, chipsets, and physical metrics for over 12,000 mobile devices.

Sourced dynamically and formatted in standardized JSON, perfect for database seeding, application schemas, and AI/LLM model context windows.

---

## 🔑 Getting an API Key

To use this library, you need an API key:
1. Go to [ds.gtgroup.dev](https://ds.gtgroup.dev).
2. Register for a free developer key or subscribe to a premium plan.
3. Keep your API key secure.

---

## 🚀 Installation

Install the package via the NuGet CLI:

```bash
dotnet add package DeviceSpecs
```

Or via the Package Manager Console:

```powershell
Install-Package DeviceSpecs
```

---

## 🛠️ Usage Quick Start

### 1. Direct Instantiation
For simple applications or console utilities, instantiate the client directly:

```csharp
using System;
using System.Threading.Tasks;
using DeviceSpecs;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize the client with your key from ds.gtgroup.dev
        using var client = new DeviceSpecsClient("YOUR_API_KEY");

        try
        {
            // Fetch specs for a specific device
            var phone = await client.GetSpecsAsync(Brands.Google, "Pixel 9 Pro");
            
            if (phone != null)
            {
                Console.WriteLine($"Device: {phone.Manufacturer} {phone.Model}");
                Console.WriteLine($"Chipset: {phone.Chipset}");
                Console.WriteLine($"RAM: {string.Join(", ", phone.NormalizedSpecs?.MemoryOptions?.AvailableRamGb ?? new())} GB");
                Console.WriteLine($"Water Resistant: {phone.NormalizedSpecs?.Physical?.IsWaterResistant}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

---

## 🔥 Fluent Deep Filtering Syntax

Construct rich, expressive multi-metric queries starting with `client.Where()`:

```csharp
// Fluent query with expressive filter method names
var flagshipDevices = await client.Where()
    .ManufacturerIs(Brands.Samsung)
    .ChipsetContains("Snapdragon")
    .RamGreaterThanOrEqual(8)
    .RamLessOrEqual(16)
    .DisplayRefreshRateGreaterThanOrEqual(120)
    .DisplaySizeGreaterThanOrEqual(6.5)
    .BatteryGreaterThanOrEqual(5000)
    .HasWirelessCharging()
    .HasStereoSpeakers()
    .PriceUsdLessOrEqual(1200)
    .ExecuteAsync(cancellationToken);
```

### Multi-Brand Filtering
Query across multiple manufacturers effortlessly:

```csharp
var budgetMidrangers = await client.Where()
    .ManufacturerBetween(Brands.Samsung, Brands.Xiaomi, Brands.Oppo)
    .RamGreaterThan(8)
    .HasNfc()
    .PriceUsdLessThan(800)
    .ExecuteAsync(cancellationToken);
```

### Custom & Raw Property Escapes
You can also append custom or raw property filters for maximum flexibility:

```csharp
var customQuery = client.Where()
    .WithCustomFilter("memory.available_storage_gb", "gte", 256)
    .WithCustomFilter("physical.ip_rating", "contains", "IP68");

// View generated query string before executing
Console.WriteLine(customQuery.ToQueryString());

var results = await customQuery.ExecuteAsync();
```

---

## 📖 API Reference

### Fluent Query Builder
```csharp
var devices = await client.Where()
    .ManufacturerIs(Brands.Google)
    .RamGreaterThanOrEqual(12)
    .DisplayRefreshRateGreaterThanOrEqual(120)
    .ExecuteAsync();
```

### Get Devices by Model Number Variant
Retrieve a device matching a specific hardware variant or model string (e.g., "SM-S928B", "A3106").
```csharp
var variant = await client.GetDeviceByModelNumberAsync("SM-S928B");
```

### Get Devices by Manufacturer
Retrieve all tracked devices for a specific brand.
```csharp
var appleDevices = await client.GetDevicesByManufacturerAsync(Brands.Apple);
```

### Get Devices by Chipset
Retrieve all devices powered by a specific SoC (system on chip).
```csharp
var tensorDevices = await client.GetDevicesByChipsetAsync("Google Tensor G4");
```

### Get Specific Device Specifications
Retrieve full raw and normalized specs for a specific manufacturer and model.
```csharp
var pixelSpecs = await client.GetSpecsAsync(Brands.Google, "Pixel 9 Pro");
```

---

## 🛡️ Error Handling

The client throws a strongly-typed `DeviceSpecsApiException` when the API gateway returns a non-success status code.

```csharp
using DeviceSpecs.Exceptions;

try
{
    var specs = await client.GetSpecsAsync("Google", "NonExistentModel");
}
catch (DeviceSpecsApiException ex)
{
    Console.WriteLine($"API Error ({(int)ex.StatusCode}): {ex.Message}");
    Console.WriteLine($"Raw Response: {ex.ResponseContent}");
}
```

---

## 📄 License
This library is licensed under the MIT License.
