using RoverCommander.Models;
using RoverCommander.Services;
using System.Text.Json;
using System.Text;

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
            // ✅ Corrected: .Value to access the float inside ExerciseValue
            float distance = _exParams.FixedDistance.Value;

            if (distance <= 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
            {
                Console.WriteLine("⚠️ Invalid distance or rover configuration.");
                return;
            }

            float maxVoltage = _config.Batteries.Max(b => b.MaxVoltage);
            var motorCommands = new List<MotorCommand>();
            float totalSpeed = 0;

            foreach (var motor in _config.Motors)
            {
                float motorRPM = motor.KvRating * maxVoltage;
                float wheelRPM = motorRPM / motor.Wheel.GearRatio;
                float wheelCircumference = (float)(Math.PI * motor.Wheel.Diameter);
                float speed = (wheelRPM * wheelCircumference) / 60.0f;

                totalSpeed += speed;

                motorCommands.Add(new MotorCommand
                {
                    Name = motor.Name,
                    Voltage = maxVoltage
                });

                Console.WriteLine($"🌀 {motor.Name} speed estimate: {speed:F2} mm/s");
            }

            float avgSpeed = totalSpeed / _config.Motors.Count;
            float duration = distance / avgSpeed;

            var command = new FixedDistanceCommand
            {
                Duration = duration,
                MotorCommands = motorCommands
            };

            await _client.PostFixedDistanceAsync(command);
        }
    }
}
