using RoverCommander.Models;
using RoverCommander.Services;

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
            float soc = _exParams.FixedCapacity.Value;

            if (soc <= 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
            {
                Console.WriteLine("Battery state of charge or configuration is invalid, skipping fixed capacity solver.");
                return;
            }

            // First calculate how much usable energy we have in watt-hours
            float totalWh = _config.Batteries.Sum(b => b.Capacity);
            float usableWh = (soc / 100f) * totalWh;

            // Estimate the total power draw of all motors at maximum voltage
            float voltage = _config.Batteries.Max(b => b.MaxVoltage);
            float totalPowerW = _config.Motors.Sum(m => voltage * m.CurrentRating);

            if (totalPowerW <= 0)
            {
                Console.WriteLine("Motor power calculation returned zero, cannot proceed.");
                return;
            }

            // Simulate operating at 80% of maximum voltage for safety
            float operatingVoltage = voltage * 0.8f;

            // Compute individual wheel speeds for the given operating voltage
            var speeds = _config.Motors.Select(m =>
            {
                float motorRPM = m.KvRating * operatingVoltage;
                float wheelRPM = motorRPM / m.Wheel.GearRatio;
                float speed = (wheelRPM * m.Wheel.Diameter * MathF.PI) / 60f;
                return float.IsFinite(speed) ? speed : 0f;
            }).ToList();

            float avgSpeed = speeds.Average();

            // Compute how long we can run the motors and how far that gets us
            float hoursAvailable = usableWh / totalPowerW;
            float distanceMm = avgSpeed * hoursAvailable * 3600;
            float distanceKm = distanceMm / 1_000_000f;

            await _client.PostFixedCapacityAsync(distanceKm);
        }
    }
}
