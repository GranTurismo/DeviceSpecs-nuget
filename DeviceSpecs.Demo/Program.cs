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

            // 1. Verify direct instantiation
            Console.WriteLine("[1/3] Testing Direct Instantiation...");
            using var client = new DeviceSpecsClient(apiKey);
            await RunTestAsync(client);

            // 2. Verify Dependency Injection Registration
            Console.WriteLine("\n[2/3] Testing Dependency Injection Registration...");
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDeviceSpecsClient(options =>
            {
                options.ApiKey = apiKey;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var diClient = serviceProvider.GetRequiredService<IDeviceSpecsClient>();
            await RunTestAsync(diClient);

            Console.WriteLine("\n[3/3] Demo program completed successfully.");
            Console.WriteLine("=============================================");
        }

        private static async Task RunTestAsync(IDeviceSpecsClient client)
        {
            try
            {
                Console.WriteLine("Attempting to fetch specs for 'Google Pixel 9 Pro'...");
                var specs = await client.GetSpecsAsync(Brands.Google, "Pixel 9 Pro");
                
                if (specs != null)
                {
                    Console.WriteLine($"-> Success! Found: {specs.Manufacturer} {specs.Model}");
                    Console.WriteLine($"   Chipset: {specs.Chipset}");
                    Console.WriteLine($"   Battery: {specs.BatteryRaw}");
                }
                else
                {
                    Console.WriteLine("-> Device not found (404).");
                }
            }
            catch (DeviceSpecsApiException ex)
            {
                Console.WriteLine($"-> Handled expected API Exception ({(int)ex.StatusCode} {ex.StatusCode}):");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Server Response: {ex.ResponseContent.Trim()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-> Unexpected error: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
