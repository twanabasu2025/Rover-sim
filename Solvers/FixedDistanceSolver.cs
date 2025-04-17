using RoverCommander.Models;
using RoverCommander.Services;

namespace RoverCommander.Solvers;

public class FixedDistanceSolver
{
    private readonly RoverSimClient _client;
    private readonly RoverConfig _config;
    private readonly ExerciseParameters _exParams;

    public FixedDistanceSolver(RoverSimClient client, RoverConfig config, ExerciseParameters exParams)
    {
        _client = client;
        _config = config;
        _exParams = exParams;
    }

    public async Task SolveAsync()
    {
        float targetDistance = _exParams.FixedDistance;

        // Choose a voltage (not too high, just for simplicity)
        float voltage = _config.Batteries[0].Voltage * 0.8f;

        var motorSpeeds = _config.Motors.Select(m =>
        {
            float wheelRpm = (m.Kv * voltage) / _config.GearRatio;
            float wheelSpeedMmPerS = (wheelRpm * _config.WheelDiameter * MathF.PI) / 60.0f;
            return new { m.Name, Voltage = voltage, Speed = wheelSpeedMmPerS };
        }).ToList();

        float averageSpeed = motorSpeeds.Average(m => m.Speed);
        float duration = targetDistance / averageSpeed;

        var command = new
        {
            duration,
            motor_commands = motorSpeeds.Select(m => new { name = m.Name, voltage = m.Voltage }).ToList()
        };

        Console.WriteLine("Submitting fixed distance command...");
        await _client.PostFixedDistanceAsync(command);
    }
}