using System.Net.Http;
using System.Net.Http.Json;
using RoverCommander.Models;

namespace RoverCommander.Services;

public class RoverSimClient
{
    private readonly HttpClient _httpClient;

    public RoverSimClient(string baseAddress)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    public async Task<bool> CheckHealthAsync()
    {
        var response = await _httpClient.GetAsync("/health");
        return response.IsSuccessStatusCode;
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
        var response = await _httpClient.PostAsJsonAsync("/verify/fixed_distance", command);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    public async Task PostFixedCapacityAsync(double distance)
    {
        var response = await _httpClient.PostAsJsonAsync("/verify/fixed_capacity", distance);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}