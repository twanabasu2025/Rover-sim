using System;
using System.Linq;
using System.Threading.Tasks;
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
            float irradiance = _exParams.FixedIrradiance.Value;
            double totalSolarPower = _config.SolarPanels
                .Sum(panel => panel.Area * panel.Efficiency * irradiance);

            double voltage = _config.Batteries.First().MaxVoltage;
            double motorPowerW = _config.Motors.Sum(m => m.CurrentRating * voltage);

            double ratio = totalSolarPower / motorPowerW;

            double minKv = _config.Motors.Min(m => m.KvRating);
            double gearRatio = _config.Motors.Average(m => m.Wheel.GearRatio);
            double wheelDiameter = _config.Motors.Average(m => m.Wheel.Diameter);

            double motorRPM = minKv * voltage;
            double wheelRPM = motorRPM / gearRatio;
            double wheelSpeed = (wheelRPM * wheelDiameter * Math.PI) / 60.0;

            double sustainableSpeed = wheelSpeed * ratio;

            Console.WriteLine($"Fixed Irradiance → Speed: {sustainableSpeed:F2} mm/s | Solar: {totalSolarPower:F1} W | MotorPower: {motorPowerW:F1} W");

            await _client.PostFixedIrradianceAsync((float)sustainableSpeed);
        }
    }
}
