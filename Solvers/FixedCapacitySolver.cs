using RoverCommander.Models;
using RoverCommander.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoverCommander.Solvers
{
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
            float soc = _exParams.FixedCapacity?.StateOfCharge ?? 0f;
            if (soc <= 0f)
            {
                Console.WriteLine("Invalid state of charge from simulator.");
                return;
            }

            var voltage = _config.Batteries.First().MaxVoltage;
            var totalWh = _config.Batteries.Sum(b => b.Capacity);
            var usableWh = totalWh * (soc / 100f);

            var motors = _config.Motors.Where(m =>
                m.KvRating > 0 && m.CurrentRating > 0 &&
                m.Wheel.GearRatio > 0 && m.Wheel.Diameter > 0).ToList();

            if (!motors.Any())
            {
                Console.WriteLine("No valid motors.");
                return;
            }

            // Calculate average speed in mm/s
            float avgSpeed = motors.Select(m =>
            {
                var motorRPM = m.KvRating * voltage;
                var wheelRPM = motorRPM / m.Wheel.GearRatio;
                return (wheelRPM * m.Wheel.Diameter * MathF.PI) / 60f;
            }).Average();

            float totalPowerW = motors.Sum(m => m.CurrentRating * voltage);
            float durationSec = (float)((usableWh * 3600f) / totalPowerW);
            float distanceMm = avgSpeed * durationSec;
            float distanceKm = distanceMm / 1_000_000f;

            // Floor to 3 decimal places
            distanceKm = MathF.Floor(distanceKm * 1000f) / 1000f;

            Console.WriteLine($"Fixed Capacity → Distance: {distanceKm:F3} km | AvgSpeed: {avgSpeed:F2} mm/s | Power: {totalPowerW:F2} W | Energy: {usableWh:F2} Wh");

            await _client.PostFixedCapacityAsync(distanceKm);
        }
    }
}
