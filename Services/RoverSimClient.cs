using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RoverCommander.Models;

namespace RoverCommander.Services;

public class RoverSimClient
{
    private readonly HttpClient _httpClient;

    public RoverSimClient(string baseAddress)
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = TimeSpan.FromSeconds(5)
        };

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "curl/8.11.1");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
    }

    public async Task<bool> CheckHealthAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            try
            {
                Console.WriteLine($"🔍 Attempting health check at {_httpClient.BaseAddress}health");

                var response = await _httpClient.GetAsync("/health");
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"🔎 Status: {(int)response.StatusCode} {response.StatusCode}");
                Console.WriteLine($"📦 Body: {content}");

                if (response.IsSuccessStatusCode ||
                    ((int)response.StatusCode == 418 && content.Contains("Ok", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("✅ Health check passed.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception during health check: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine($"Health check failed. Retrying... ({i + 1}/5)\n");
            await Task.Delay(1000);
        }

        return false;
    }

    public async Task<RoverConfig> GetRoverConfigAsync()
    {
        return await _httpClient.GetFromJsonAsync<RoverConfig>("/rover/config")
            ?? throw new InvalidOperationException("Failed to fetch rover config");
    }

    public async Task<ExerciseParameters> GetExercisesAsync()
    {
        return await _httpClient.GetFromJsonAsync<ExerciseParameters>("/exercises")
            ?? throw new InvalidOperationException("Failed to fetch exercise parameters");
    }

    public async Task PostFixedDistanceAsync(object command)
    {
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        var response = await _httpClient.PostAsJsonAsync("/verify/fixed_distance", command, options);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    public async Task PostFixedCapacityAsync(double distance)
    {
        var response = await _httpClient.PostAsJsonAsync("/verify/fixed_capacity", distance);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}
