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
            var json = JsonSerializer.Serialize(command, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("verify/fixed_distance", content);
            Console.WriteLine("POST /verify/fixed_distance");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Message: {await response.Content.ReadAsStringAsync()}");
        }
        // Submits how far the rover can travel with a given state of battery charge
        public async Task PostFixedCapacityAsync(float distance)
        {
            var json = JsonSerializer.Serialize(distance);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("verify/fixed_capacity", content);
            Console.WriteLine("POST /verify/fixed_capacity");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Message: {await response.Content.ReadAsStringAsync()}");
        }
        // Submits the speed the rover can maintain with steady solar input
        public async Task PostFixedIrradianceAsync(float speed)
        {
            var json = JsonSerializer.Serialize(speed);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("verify/fixed_irradiance", content);
            Console.WriteLine("POST /verify/fixed_irradiance");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Message: {await response.Content.ReadAsStringAsync()}");
        }
        // Submits the estimated distance the rover can travel in one Martian day
        public async Task PostVariableIrradianceAsync(float distance)
        {
            var json = JsonSerializer.Serialize(distance);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("verify/variable_irradiance", content);
            Console.WriteLine("POST /verify/variable_irradiance");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Message: {await response.Content.ReadAsStringAsync()}");
        }
    }
}