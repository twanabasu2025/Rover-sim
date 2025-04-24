using System;
using System.Net.Http;
using System.Threading.Tasks;
using RoverCommander.Models;
using RoverCommander.Services;
using RoverCommander.Solvers;

namespace RoverCommander
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Starting Rover Commander Client...\n");

            // Initialize HTTP client to talk to simulator
            using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
            var roverSimClient = new RoverSimClient(httpClient);

            // Fetch current rover configuration
            Console.WriteLine("📦 Fetching rover config...");
            RoverConfig config = await roverSimClient.GetRoverConfigAsync();

            // Fetch exercise parameters
            Console.WriteLine("🧪 Fetching exercise parameters...");
            ExerciseParameters exParams = await roverSimClient.GetExerciseParametersAsync();

            // Run each solver
            Console.WriteLine("\n--- 🛞 Solving Fixed Distance ---");
            var fixedDistanceSolver = new FixedDistanceSolver(roverSimClient, config, exParams);
            await fixedDistanceSolver.SolveAsync();

            Console.WriteLine("\n--- 🔋 Solving Fixed Capacity ---");
            var fixedCapacitySolver = new FixedCapacitySolver(roverSimClient, config, exParams);
            await fixedCapacitySolver.SolveAsync();

            Console.WriteLine("\n--- ☀️ Solving Fixed Irradiance ---");
            var fixedIrradianceSolver = new FixedIrradianceSolver(roverSimClient, config, exParams);
            await fixedIrradianceSolver.SolveAsync();

            Console.WriteLine("\n--- 🌅 Solving Variable Irradiance ---");
            var variableIrradianceSolver = new VariableIrradianceSolver(roverSimClient, config, exParams);
            await variableIrradianceSolver.SolveAsync();

            Console.WriteLine("\n✅ All solvers executed.");
        }
    }
}
