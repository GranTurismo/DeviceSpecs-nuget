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

### 2. Dependency Injection Setup (.NET Core / 8 / 10)
For modern ASP.NET Core or worker applications, register the client in your `Program.cs`:

```csharp
using DeviceSpecs;

var builder = WebApplication.CreateBuilder(args);

// Register the DeviceSpecs client using the DI extension helper
builder.Services.AddDeviceSpecsClient(options =>
{
    options.ApiKey = builder.Configuration["DeviceSpecs:ApiKey"] 
                     ?? throw new InvalidOperationException("API Key is missing!");
});
```

Then inject `IDeviceSpecsClient` into your services or controllers:

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DeviceSpecs;

[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceSpecsClient _client;

    public DevicesController(IDeviceSpecsClient client)
    {
        _client = client;
    }

    [HttpGet("{manufacturer}/{model}")]
    public async Task<IActionResult> GetDeviceDetails(string manufacturer, string model)
    {
        var specs = await _client.GetSpecsAsync(manufacturer, model);
        if (specs == null) return NotFound("Device not found.");
        return Ok(specs);
    }
}
```

---

## 📖 API Reference

### List and Filter Devices
Retrieve a filtered list of mobile devices based on specifications.
```csharp
// Filter devices with battery >= 5000mAh, Snapdragon chip, and RAM between 8GB and 12GB
var devices = await client.GetDevicesAsync(
    batteryGte: 5000,
    chipsetContains: "Snapdragon",
    minRamGb: 8,
    maxRamGb: 12
);
```

### Get Devices by Manufacturer
Retrieve all tracked devices for a specific brand.
```csharp
var appleDevices = await client.GetDevicesByManufacturerAsync("Apple");
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
