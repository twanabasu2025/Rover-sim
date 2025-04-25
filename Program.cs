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
            Console.WriteLine("Rover Commander: Starting all solvers...");

            // Initialize HttpClient to talk with the rover simulation server
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/") // Ensure the rover_sim server is running on this address
            };

            var client = new RoverSimClient(httpClient);

            // Fetch the current rover configuration (motors, batteries, solar panels, etc.)
            Console.WriteLine("Loading rover configuration...");
            var config = await client.GetRoverConfigAsync();

            // Fetch the current exercise parameters (fixed distance, fixed capacity, irradiance values, etc.)
            Console.WriteLine("Loading simulation parameters...");
            var exParams = await client.GetExerciseParametersAsync();

            // Solve the Fixed Distance exercise
            Console.WriteLine("Solving Fixed Distance...");
            var distanceSolver = new FixedDistanceSolver(client, config, exParams);
            await distanceSolver.SolveAsync();

            // Solve the Fixed Capacity exercise
            Console.WriteLine("Solving Fixed Capacity...");
            var capacitySolver = new FixedCapacitySolver(client, config, exParams);
            await capacitySolver.SolveAsync();

            // Solve the Fixed Irradiance exercise
            Console.WriteLine("Solving Fixed Irradiance...");
            var fixedIrradianceSolver = new FixedIrradianceSolver(client, config, exParams);
            await fixedIrradianceSolver.SolveAsync();

            // Solve the Variable Irradiance exercise
            Console.WriteLine("Solving Variable Irradiance...");
            var variableIrradianceSolver = new VariableIrradianceSolver(client, config, exParams);
            await variableIrradianceSolver.SolveAsync();

            Console.WriteLine("All solvers executed successfully.");
        }
    }
}
