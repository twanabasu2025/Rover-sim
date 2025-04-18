using RoverCommander.Services;
using RoverCommander.Solvers;

Console.WriteLine(">>");
Console.WriteLine("Checking server health...");

var client = new RoverSimClient("http://localhost:8080");

if (!await client.CheckHealthAsync())
{
    Console.WriteLine("❌ Rover Sim Server is not reachable.");
    return;
}

Console.WriteLine("Fetching exercise and rover config data...");

var config = await client.GetRoverConfigAsync();
var exParams = await client.GetExercisesAsync();

// 🧠 Print config for debugging
Console.WriteLine("\n⚙️ Rover Configuration:");
Console.WriteLine($" - Gear Ratio: {config.GearRatio}");
Console.WriteLine($" - Wheel Diameter: {config.WheelDiameter}");
Console.WriteLine($" - Motors: {config.Motors.Count}");
foreach (var m in config.Motors)
{
    Console.WriteLine($"   • {m.Name} | KV: {m.Kv} | Current: {m.CurrentRating}");
}

Console.WriteLine($" - Batteries: {config.Batteries.Count}");
foreach (var b in config.Batteries)
{
    Console.WriteLine($"   • Capacity: {b.Capacity} Wh | Voltage: {b.Voltage}");
}

Console.WriteLine("\n📐 Exercise Params:");
Console.WriteLine($" - Fixed Distance: {exParams.FixedDistance} mm");
Console.WriteLine($" - Fixed Capacity: {exParams.FixedCapacity}%");

// 🚀 Run Fixed Distance Solver
var fixedDistanceSolver = new FixedDistanceSolver(client, config, exParams);
await fixedDistanceSolver.SolveAsync();

// 🚗 Run Fixed Capacity Solver
var fixedCapacitySolver = new FixedCapacitySolver(client, config, exParams);
await fixedCapacitySolver.SolveAsync();
