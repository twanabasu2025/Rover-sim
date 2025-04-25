using System.Net.Http;
using System.Text;
using System.Text.Json;
using RoverCommander.Models;

namespace RoverCommander.Services
{
    public class RoverSimClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public RoverSimClient(HttpClient httpClient)
        {
            _http = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        // Retrieves the current rover configuration from the simulator
        public async Task<RoverConfig> GetRoverConfigAsync()
        {
            var response = await _http.GetAsync("rover/config");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RoverConfig>(json, _jsonOptions)!;
        }

        // Retrieves the exercise parameters like fixed distance and battery capacity
        public async Task<ExerciseParameters> GetExerciseParametersAsync()
        {
            var response = await _http.GetAsync("exercises");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ExerciseParameters>(json, _jsonOptions)!;
        }

        // Submits a command to move the rover a specific distance
        public async Task PostFixedDistanceAsync(FixedDistanceCommand command)
        {
            await PostJsonAsync("verify/fixed_distance", command);
        }

        // Submits how far the rover can travel with a given state of battery charge
        public async Task PostFixedCapacityAsync(float distance)
        {
            await PostJsonAsync("verify/fixed_capacity", distance);
        }

        // Submits the speed the rover can maintain with steady solar input
        public async Task PostFixedIrradianceAsync(float speed)
        {
            await PostJsonAsync("verify/fixed_irradiance", speed);
        }

        // Submits the estimated distance the rover can travel in one Martian day
        public async Task PostVariableIrradianceAsync(float distance)
        {
            await PostJsonAsync("verify/variable_irradiance", distance);
        }

        // 🔁 This private method avoids repeating the same code in each POST method
        private async Task PostJsonAsync<T>(string endpoint, T payload)
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(endpoint, content);

            Console.WriteLine($"POST /{endpoint}");
            Console.WriteLine($"Status: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Message: {body}");
        }
    }
}
