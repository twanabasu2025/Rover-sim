using RoverCommander.Models;
using RoverCommander.Services;

namespace RoverCommander.Solvers
{
    public class FixedIrradianceSolver
    {
        private readonly RoverSimClient _client;
        private readonly RoverConfig _config;
        private readonly ExerciseParameters _exParams;

        public FixedIrradianceSolver(RoverSimClient client, RoverConfig config, ExerciseParameters exParams)
        {
            _client = client;
            _config = config;
            _exParams = exParams;
        }

        public async Task SolveAsync()
        {
            // ✅ Correct: unwrap the value object
            float irradiance = _exParams.FixedIrradiance.Value;

            if (irradiance <= 0 || _config.SolarPanels.Count == 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
            {
                Console.WriteLine("⚠️ Invalid irradiance or incomplete config.");
                return;
            }

            float totalSolarPower = _config.SolarPanels.Sum(p => p.Area * p.Efficiency * irradiance);
            float voltage = _config.Batteries.Max(b => b.MaxVoltage);
            float avgCurrent = _config.Motors.Average(m => m.CurrentRating);
            float powerPerMotor = voltage * avgCurrent;
            float totalMotorPower = powerPerMotor * _config.Motors.Count;

            float powerScale = totalSolarPower / totalMotorPower;
            float usableVoltage = voltage * MathF.Min(powerScale, 1.0f);

            var speeds = _config.Motors.Select(m =>
            {
                float rpm = m.KvRating * usableVoltage;
                float wheelRPM = rpm / m.Wheel.GearRatio;
                float speed = (wheelRPM * m.Wheel.Diameter * MathF.PI) / 60f;
                return float.IsFinite(speed) ? speed : 0f;
            }).ToList();

            float averageSpeed = speeds.Average();

            Console.WriteLine($"☀️ Estimated sustainable speed: {averageSpeed:F2} mm/s");

            await _client.PostFixedIrradianceAsync(averageSpeed);
        }
    }
}
