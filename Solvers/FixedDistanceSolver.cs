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

        // Safeguard: Ensure non-zero gear ratio and valid motor config
        if (_config.GearRatio <= 0 || _config.Motors.Count == 0)
        {
            Console.WriteLine("⚠️ Invalid configuration: Check gear ratio and motors.");
            return;
        }

        // Choose a safe voltage (80% of max battery voltage)
        float voltage = _config.Batteries[0].Voltage * 0.8f;

        var motorSpeeds = _config.Motors.Select(m =>
        {
            float wheelRpm = (m.Kv * voltage) / _config.GearRatio;

            float wheelSpeedMmPerS = (wheelRpm * _config.WheelDiameter * MathF.PI) / 60.0f;

            // fallback to 0 if math failed
            if (float.IsNaN(wheelSpeedMmPerS) || float.IsInfinity(wheelSpeedMmPerS))
                wheelSpeedMmPerS = 0f;

            return new { m.Name, Voltage = voltage, Speed = wheelSpeedMmPerS };
        }).ToList();

        float averageSpeed = motorSpeeds.Average(m => m.Speed);
        if (averageSpeed <= 0)
        {
            Console.WriteLine("⚠️ Computed average speed is 0. Cannot move.");
            return;
        }

        float duration = targetDistance / averageSpeed;

        var command = new
        {
            duration = float.IsInfinity(duration) || float.IsNaN(duration) ? 0 : duration,
            motor_commands = motorSpeeds.Select(m => new { name = m.Name, voltage = m.Voltage }).ToList()
        };

        Console.WriteLine("🚀 Submitting fixed distance command...");
        await _client.PostFixedDistanceAsync(command);
    }
}
