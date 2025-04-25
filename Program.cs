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
            Console.WriteLine("Rover Commander: Running Fixed Distance and Fixed Capacity solvers only");

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };

            var client = new RoverSimClient(httpClient);

            Console.WriteLine("Loading rover configuration...");
            var config = await client.GetRoverConfigAsync();

            Console.WriteLine("Loading simulation parameters...");
            var exParams = await client.GetExerciseParametersAsync();

            Console.WriteLine("Solving Fixed Distance...");
            var distanceSolver = new FixedDistanceSolver(client, config, exParams);
            await distanceSolver.SolveAsync();

            Console.WriteLine("Solving Fixed Capacity...");
            var capacitySolver = new FixedCapacitySolver(client, config, exParams);
            await capacitySolver.SolveAsync();

            Console.WriteLine("All solvers executed.");
        }
    }
}
