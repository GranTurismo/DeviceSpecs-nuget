using System;
using System.Threading.Tasks;
using DeviceSpecs;
using DeviceSpecs.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceSpecs.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("      DeviceSpecs SDK .NET Demo Console      ");
            Console.WriteLine("=============================================");
            
            // Check if key is passed as argument, otherwise use dummy key
            string apiKey = args.Length > 0 ? args[0] : "DUMMY_API_KEY_FOR_TESTING";
            
            Console.WriteLine($"Configuring client with key: {apiKey}");
            Console.WriteLine("For actual use, obtain a valid key from: https://ds.gtgroup.dev");
            Console.WriteLine();

            // 1. Verify direct instantiation & Fluent Deep Query Builder
            Console.WriteLine("[1/2] Testing Direct Instantiation & Fluent Deep Query Builder...");
            using var client = new DeviceSpecsClient(apiKey);
            await RunTestAsync(client);

            // 2. Verify Dependency Injection Registration
            Console.WriteLine("\n[2/2] Testing Dependency Injection Registration...");
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDeviceSpecsClient(options =>
            {
                options.ApiKey = apiKey;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var diClient = serviceProvider.GetRequiredService<IDeviceSpecsClient>();
            await RunTestAsync(diClient);

            Console.WriteLine("\nDemo program completed successfully.");
            Console.WriteLine("=============================================");
        }

        private static async Task RunTestAsync(IDeviceSpecsClient client)
        {
            // Test 1: Fetch specs for Google Pixel 9 Pro
            Console.WriteLine("\n--> [Test 1] GetSpecsAsync('Google', 'Pixel 9 Pro')...");
            try
            {
                var specs = await client.GetSpecsAsync(Brands.Google, "Pixel 9 Pro");
                if (specs != null)
                {
                    Console.WriteLine($"   Found: {specs.Manufacturer} {specs.Model}");
                    Console.WriteLine($"   Chipset: {specs.Chipset}");
                    Console.WriteLine($"   Battery: {specs.BatteryRaw}");
                }
                else
                {
                    Console.WriteLine("   Device not found (404).");
                }
            }
            catch (DeviceSpecsApiException ex)
            {
                Console.WriteLine($"   API Response (Status {(int)ex.StatusCode}): {ex.ResponseContent.Trim()}");
            }

            // Test 2: Fluent Deep Filtering Query Construction with new Where().ManufacturerIs(...) syntax
            Console.WriteLine("\n--> [Test 2] Constructing Fluent Deep Filter Query (client.Where()...)...");
            var query = client.Where()
                .ManufacturerIs(Brands.Samsung)
                .ChipsetContains("Snapdragon")
                .RamGreaterThanOrEqual(8)
                .RamLessOrEqual(16)
                .DisplayRefreshRateGreaterThanOrEqual(120)
                .DisplaySizeGreaterThanOrEqual(6.5)
                .BatteryGreaterThanOrEqual(5000)
                .HasWirelessCharging()
                .HasStereoSpeakers()
                .PriceUsdLessOrEqual(1200);

            Console.WriteLine($"   Generated Query String:\n   {query.ToQueryString()}");
            Console.WriteLine("   Executing Query...");
            try
            {
                var filteredDevices = await query.ExecuteAsync();
                Console.WriteLine($"   Returned {filteredDevices.Count} matching device(s).");
            }
            catch (DeviceSpecsApiException ex)
            {
                Console.WriteLine($"   API Response (Status {(int)ex.StatusCode}): {ex.ResponseContent.Trim()}");
            }

            // Test 3: Multi-brand query with ManufacturerBetween / ManufacturerIn
            Console.WriteLine("\n--> [Test 3] Multi-brand Query with ManufacturerBetween(Samsung, Xiaomi, Oppo)...");
            var multiBrandQuery = client.Where()
                .ManufacturerBetween(Brands.Samsung, Brands.Xiaomi, Brands.Oppo)
                .RamGreaterThan(8)
                .HasNfc()
                .PriceUsdLessThan(800);

            Console.WriteLine($"   Generated Multi-Brand Query String:\n   {multiBrandQuery.ToQueryString()}");

            // Test 4: Model Number Variant Lookup
            Console.WriteLine("\n--> [Test 4] GetDeviceByModelNumberAsync('SM-S928B')...");
            try
            {
                var modelDevice = await client.GetDeviceByModelNumberAsync("SM-S928B");
                if (modelDevice != null)
                {
                    Console.WriteLine($"   Found Variant: {modelDevice.Manufacturer} {modelDevice.Model}");
                }
                else
                {
                    Console.WriteLine("   Model number variant not found (404).");
                }
            }
            catch (DeviceSpecsApiException ex)
            {
                Console.WriteLine($"   API Response (Status {(int)ex.StatusCode}): {ex.ResponseContent.Trim()}");
            }
        }
    }
}
