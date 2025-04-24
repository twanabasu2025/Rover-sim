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
            Console.WriteLine("Starting Rover Commander Client...");

            // Set up the HTTP client to communicate with the simulator
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };

            var roverSimClient = new RoverSimClient(httpClient);

            // Fetch the rover's hardware configuration
            Console.WriteLine("Getting rover configuration...");
            RoverConfig config = await roverSimClient.GetRoverConfigAsync();

            // Fetch the values used for this run's simulation exercises
            Console.WriteLine("Getting exercise parameters...");
            ExerciseParameters exParams = await roverSimClient.GetExerciseParametersAsync();

            // Solve fixed distance challenge
            Console.WriteLine("Solving: Fixed Distance...");
            var fixedDistanceSolver = new FixedDistanceSolver(roverSimClient, config, exParams);
            await fixedDistanceSolver.SolveAsync();

            // Solve fixed capacity challenge
            Console.WriteLine("Solving: Fixed Capacity...");
            var fixedCapacitySolver = new FixedCapacitySolver(roverSimClient, config, exParams);
            await fixedCapacitySolver.SolveAsync();

            // Solve fixed irradiance challenge
            Console.WriteLine("Solving: Fixed Irradiance...");
            var fixedIrradianceSolver = new FixedIrradianceSolver(roverSimClient, config, exParams);
            await fixedIrradianceSolver.SolveAsync();

            // Solve variable irradiance (Martian day) challenge
            Console.WriteLine("Solving: Variable Irradiance...");
            var variableIrradianceSolver = new VariableIrradianceSolver(roverSimClient, config, exParams);
            await variableIrradianceSolver.SolveAsync();

            Console.WriteLine("All solvers have completed.");
        }
    }
}
