# DeviceSpecs Client SDK (.NET)

[![NuGet Version](https://img.shields.io/nuget/v/DeviceSpecs.svg)](https://www.nuget.org/packages/DeviceSpecs)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A strongly-typed, high-performance C# / .NET client library for the **[DeviceSpecs API](https://ds.gtgroup.dev)**. Access structured, clean, and normalized specifications, chipsets, and physical metrics for over 12,000 mobile devices.

Sourced dynamically and formatted in standardized JSON, perfect for database seeding, mobile application backends, and AI/LLM model context windows.

---

## 🌐 Official Website, API Portal & Live Testing

For API key generation, interactive testing, endpoint documentation, and subscription management, visit our official portal:

👉 **[https://ds.gtgroup.dev](https://ds.gtgroup.dev)**

### Key Resources on [ds.gtgroup.dev](https://ds.gtgroup.dev):
* 🔑 **API Key Management**: Obtain a free developer API key or upgrade your plan.
* 🧪 **Interactive API Playground**: Test queries, endpoints, and filters directly in your browser.
* 📖 **API Documentation**: Browse full spec models, filter parameters, and dataset details.

---

## 🚀 Installation

Install the package via the .NET CLI:

```bash
dotnet add package DeviceSpecs
```

Or via Package Manager Console:

```powershell
Install-Package DeviceSpecs
```

---

## 🛠️ Usage Quick Start

### 1. Direct Instantiation
```csharp
using System;
using System.Threading.Tasks;
using DeviceSpecs;

class Program
{
    static async Task Main(string[] args)
    {
        // Obtain your API key from https://ds.gtgroup.dev
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

### 2. Dependency Injection Setup (.NET Core / 8 / 10)
In `Program.cs`:

```csharp
using DeviceSpecs;

var builder = WebApplication.CreateBuilder(args);

// Register the DeviceSpecs client using DI extension helper
builder.Services.AddDeviceSpecsClient(options =>
{
    options.ApiKey = builder.Configuration["DeviceSpecs:ApiKey"] 
                     ?? throw new InvalidOperationException("API Key is missing!");
});
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
For maximum flexibility, append custom key-value property filters:

```csharp
var customQuery = client.Where()
    .WithCustomFilter("memory.available_storage_gb", "gte", 256)
    .WithCustomFilter("physical.ip_rating", "contains", "IP68");

// Print generated URL query string for debugging or logging
Console.WriteLine(customQuery.ToQueryString());

var results = await customQuery.ExecuteAsync();
```

---

## 📖 API Reference

| Method | Description |
| :--- | :--- |
| `client.Where()` | Initiates a fluent query builder chain. |
| `client.GetSpecsAsync(manufacturer, model)` | Fetches raw & normalized specifications for a device model. |
| `client.GetDeviceByModelNumberAsync(modelNumber)` | Fetches device specifications by hardware variant identifier (e.g. `SM-S928B`). |
| `client.GetDevicesByManufacturerAsync(manufacturer)` | Returns all devices for a given manufacturer. |
| `client.GetDevicesByChipsetAsync(chipset)` | Returns all devices powered by a specific SoC / chipset. |

For detailed interactive API testing and live requests, visit **[ds.gtgroup.dev](https://ds.gtgroup.dev)**.

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
