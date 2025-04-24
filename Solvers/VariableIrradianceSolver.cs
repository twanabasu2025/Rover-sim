using RoverCommander.Models;
using RoverCommander.Services;

namespace RoverCommander.Solvers
{
    public class VariableIrradianceSolver
    {
        private readonly RoverSimClient _client;
        private readonly RoverConfig _config;
        private readonly ExerciseParameters _exParams;

        public VariableIrradianceSolver(RoverSimClient client, RoverConfig config, ExerciseParameters exParams)
        {
            _client = client;
            _config = config;
            _exParams = exParams;
        }

        public async Task SolveAsync()
        {
            // ✅ Corrected: Access the .Value inside ExerciseValue
            float peakIrradiance = _exParams.VariableIrradiance.Value;

            if (peakIrradiance <= 0 || _config.SolarPanels.Count == 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
            {
                Console.WriteLine("⚠️ Missing config or invalid irradiance.");
                return;
            }

            float totalPanelArea = _config.SolarPanels.Sum(p => p.Area);
            float avgEfficiency = _config.SolarPanels.Average(p => p.Efficiency);
            float averageIrradiance = peakIrradiance * (2f / MathF.PI);
            float solarWatts = totalPanelArea * avgEfficiency * averageIrradiance;

            float voltage = _config.Batteries.Max(b => b.MaxVoltage);
            float currentPerMotor = _config.Motors.Average(m => m.CurrentRating);
            float motorPower = voltage * currentPerMotor * _config.Motors.Count;

            float usableVoltage = voltage * MathF.Min(solarWatts / motorPower, 1.0f);

            var speeds = _config.Motors.Select(m =>
            {
                float motorRPM = m.KvRating * usableVoltage;
                float wheelRPM = motorRPM / m.Wheel.GearRatio;
                float speed = (wheelRPM * m.Wheel.Diameter * MathF.PI) / 60f;
                return float.IsFinite(speed) ? speed : 0f;
            }).ToList();

            float averageSpeed = speeds.Average();

            float solSeconds = (24 * 3600) + (37 * 60) + 22;
            float distanceMm = averageSpeed * solSeconds;
            float distanceKm = distanceMm / 1_000_000f;

            await _client.PostVariableIrradianceAsync(distanceKm);
        }
    }
}
