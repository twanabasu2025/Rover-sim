using System;
using System.Linq;
using System.Threading.Tasks;
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
            float peak = _exParams.VariableIrradiance.PeakValue;
            double area = _config.SolarPanels.Sum(p => p.Area);
            double efficiency = _config.SolarPanels.Average(p => p.Efficiency);

            double solLength = (24 * 3600) + (37 * 60) + 22;
            double steps = 1000;
            double dt = solLength / steps;
            double energyWh = 0;

            for (int i = 0; i < steps; i++)
            {
                double t = i * dt / solLength;
                double irradiance = peak * Math.Sin(Math.PI * t);
                double power = irradiance * area * efficiency;
                energyWh += power * dt / 3600;
            }

            double voltage = _config.Batteries.First().MaxVoltage;
            double powerW = _config.Motors.Sum(m => m.CurrentRating * voltage);
            double runtime = energyWh / powerW;

            double minKv = _config.Motors.Min(m => m.KvRating);
            double gear = _config.Motors.Average(m => m.Wheel.GearRatio);
            double diameter = _config.Motors.Average(m => m.Wheel.Diameter);

            double rpm = minKv * voltage;
            double wheelRPM = rpm / gear;
            double speed = (wheelRPM * diameter * Math.PI) / 60;

            double distance = speed * runtime * 3600;
            double km = distance / 1_000_000;

            km = Math.Floor(km * 1000) / 1000;

            Console.WriteLine($"Variable Irradiance → Distance: {km:F3} km | Energy: {energyWh:F2} Wh | Speed: {speed:F2} mm/s");

            await _client.PostVariableIrradianceAsync((float)km);
        }
    }
}
