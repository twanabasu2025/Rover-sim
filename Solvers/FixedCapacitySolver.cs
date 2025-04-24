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
                Console.WriteLine("⚠️ Invalid SOC or missing config.");
                return;
            }

            float totalWh = _config.Batteries.Sum(b => b.Capacity);
            float usableWh = (soc / 100f) * totalWh;
            float voltage = _config.Batteries.Max(b => b.MaxVoltage);
            float totalPowerW = _config.Motors.Sum(m => voltage * m.CurrentRating);

            if (totalPowerW <= 0)
            {
                Console.WriteLine("⚠️ Total motor power is zero.");
                return;
            }

            float driveVoltage = voltage * 0.8f;

            var speeds = _config.Motors.Select(m =>
            {
                float rpm = m.KvRating * driveVoltage;
                float wheelRPM = rpm / m.Wheel.GearRatio;
                float speed = (wheelRPM * m.Wheel.Diameter * MathF.PI) / 60f;
                return float.IsFinite(speed) ? speed : 0f;
            }).ToList();

            float avgSpeed = speeds.Average();
            float hours = usableWh / totalPowerW;
            float mm = avgSpeed * hours * 3600;
            float km = mm / 1_000_000f;

            await _client.PostFixedCapacityAsync(km);
        }
    }
}
