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
        float usableWh = _config.Batteries.Sum(b => b.Capacity) * (_exParams.FixedCapacity / 100f);

        // Use same voltage and gear approach
        float voltage = _config.Batteries[0].Voltage * 0.8f;

        var motorSpeeds = _config.Motors.Select(m =>
        {
            float wheelRpm = (m.Kv * voltage) / _config.GearRatio;
            float wheelSpeed = (wheelRpm * _config.WheelDiameter * MathF.PI) / 60.0f;
            float power = voltage * m.CurrentRating;
            return new { Speed = wheelSpeed, Power = power };
        }).ToList();

        float avgSpeed = motorSpeeds.Average(x => x.Speed); // mm/s
        float totalPower = motorSpeeds.Sum(x => x.Power);   // watts

        float travelTime = usableWh * 3600f / totalPower; // seconds
        float distanceMm = avgSpeed * travelTime;
        float distanceKm = distanceMm / 1_000_000f;

        Console.WriteLine($"Submitting fixed capacity distance: {distanceKm:F2} km");
        await _client.PostFixedCapacityAsync(distanceKm);
    }
}
