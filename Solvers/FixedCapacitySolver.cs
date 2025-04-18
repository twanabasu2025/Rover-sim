using RoverCommander.Models;
using RoverCommander.Services;

namespace RoverCommander.Solvers;

public class FixedCapacitySolver
{
    private readonly RoverSimClient _client;
    private readonly RoverConfig _config;
    private readonly ExerciseParameters _exParams;

    public FixedCapacitySolver(RoverSimClient client, RoverConfig config, ExerciseParameters exParams)
    {
        _client = client;
        _config = config;
        _exParams = exParams;
    }

    public async Task SolveAsync()
    {
        float soc = _exParams.FixedCapacity; // in %
        if (soc <= 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
        {
            Console.WriteLine("⚠️ Invalid config or state of charge.");
            return;
        }

        float totalWh = _config.Batteries.Sum(b => b.Capacity); // total watt-hours
        float usableWh = (soc / 100f) * totalWh;

        float batteryVoltage = _config.Batteries[0].Voltage;

        // ✅ Corrected: Use CurrentRating from motor model
        float totalMotorPowerW = _config.Motors.Sum(m => batteryVoltage * m.CurrentRating);
        if (totalMotorPowerW <= 0)
        {
            Console.WriteLine("⚠️ Total motor power is invalid.");
            return;
        }

        // Estimate average speed (like in FixedDistance)
        float voltage = batteryVoltage * 0.8f;
        var wheelSpeeds = _config.Motors.Select(m =>
        {
            float wheelRpm = (m.Kv * voltage) / _config.GearRatio;
            float wheelSpeed = (wheelRpm * _config.WheelDiameter * MathF.PI) / 60.0f;

            if (float.IsNaN(wheelSpeed) || float.IsInfinity(wheelSpeed))
                wheelSpeed = 0f;

            return wheelSpeed;
        }).ToList();

        float averageSpeed = wheelSpeeds.Average();

        float hoursAvailable = usableWh / totalMotorPowerW;
        float travelDistanceMm = averageSpeed * (float)(hoursAvailable * 3600); // mm = mm/s * s
        float travelDistanceKm = travelDistanceMm / 1_000_000f;

        if (float.IsNaN(travelDistanceKm) || float.IsInfinity(travelDistanceKm))
        {
            Console.WriteLine("⚠️ Computed distance is invalid.");
            return;
        }

        Console.WriteLine($"🚗 Submitting fixed capacity distance: {travelDistanceKm} km");
        await _client.PostFixedCapacityAsync(travelDistanceKm);
    }
}
