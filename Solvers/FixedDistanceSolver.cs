using RoverCommander.Models;
using RoverCommander.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoverCommander.Solvers
{
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
            float distance = _exParams.FixedDistance?.Value ?? 0;

            if (distance <= 0 || !_config.Motors.Any() || !_config.Batteries.Any())
            {
                Console.WriteLine("Invalid config or distance. Skipping.");
                return;
            }

            float maxVoltage = _config.Batteries.Max(b => b.MaxVoltage);
            float slowestSpeed = float.MaxValue;

            foreach (var motor in _config.Motors)
            {
                if (motor.KvRating <= 0 || motor.Wheel.GearRatio <= 0 || motor.Wheel.Diameter <= 0)
                    continue;

                float rpm = motor.KvRating * maxVoltage;
                float wheelRPM = rpm / motor.Wheel.GearRatio;
                float speed = (wheelRPM * motor.Wheel.Diameter * MathF.PI) / 60f;

                if (speed < slowestSpeed)
                    slowestSpeed = speed;
            }

            float duration = distance / slowestSpeed;
            var commands = new List<MotorCommand>();

            foreach (var motor in _config.Motors)
            {
                if (motor.KvRating <= 0 || motor.Wheel.GearRatio <= 0 || motor.Wheel.Diameter <= 0)
                    continue;

                float wheelCircumference = (float)(Math.PI * motor.Wheel.Diameter);
                float targetWheelRPM = (slowestSpeed * 60f) / wheelCircumference;
                float targetMotorRPM = targetWheelRPM * motor.Wheel.GearRatio;
                float voltage = targetMotorRPM / motor.KvRating;

                voltage = MathF.Min(voltage, maxVoltage);

                commands.Add(new MotorCommand
                {
                    Name = motor.Name,
                    Voltage = voltage
                });

                Console.WriteLine($"{motor.Name} → Speed: {slowestSpeed:F2} mm/s @ {voltage:F2} V");
            }

            Console.WriteLine($"Distance: {distance:F2} mm | Duration: {duration:F2} s");

            await _client.PostFixedDistanceAsync(new FixedDistanceCommand
            {
                Duration = duration,
                MotorCommands = commands
            });
        }
    }
}
