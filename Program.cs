using RoverCommander.Services;
using RoverCommander.Solvers;

var client = new RoverSimClient("http://localhost:8080");

Console.WriteLine("Checking server health...");
if (!await client.CheckHealthAsync())
{
    Console.WriteLine("Rover Sim Server is not reachable.");
    return;
}

Console.WriteLine("Fetching exercise and rover config data...");
var config = await client.GetRoverConfigAsync();
var exercises = await client.GetExercisesAsync();

// Solving Fixed Distance
var fixedDistanceSolver = new FixedDistanceSolver(client, config, exercises);
await fixedDistanceSolver.SolveAsync();

// Solving Fixed Capacity
var fixedCapacitySolver = new FixedCapacitySolver(client, config, exercises);
await fixedCapacitySolver.SolveAsync();


