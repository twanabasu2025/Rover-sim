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
            float distance = _exParams.FixedDistance.Value;

            if (distance <= 0 || _config.Motors.Count == 0 || _config.Batteries.Count == 0)
            {
                Console.WriteLine("Distance or config is invalid, skipping fixed distance solver.");
                return;
            }

            // Instead of sending the same voltage to every motor, we compute a voltage for each one
            // that results in the same wheel speed. This avoids the rover drifting or veering.

            float targetSpeed = 500.0f; // in mm/s

            var motorCommands = new List<MotorCommand>();
            float totalSpeed = 0;

            foreach (var motor in _config.Motors)
            {
                float wheelCircumference = (float)(Math.PI * motor.Wheel.Diameter);

                // Convert desired linear speed to wheel RPM
                float targetWheelRPM = (targetSpeed * 60) / wheelCircumference;

                // Convert wheel RPM to motor RPM based on gear ratio
                float targetMotorRPM = targetWheelRPM * motor.Wheel.GearRatio;

                // Calculate the voltage needed to hit that motor RPM using the motor's KV rating
                float voltage = targetMotorRPM / motor.KvRating;

                // Clamp the voltage to the max battery voltage to avoid overdriving the motor
                float maxVoltage = _config.Batteries.Max(b => b.MaxVoltage);
                voltage = MathF.Min(voltage, maxVoltage);

                motorCommands.Add(new MotorCommand
                {
                    Name = motor.Name,
                    Voltage = voltage
                });

                float actualMotorRPM = motor.KvRating * voltage;
                float actualWheelRPM = actualMotorRPM / motor.Wheel.GearRatio;
                float actualSpeed = (actualWheelRPM * wheelCircumference) / 60;

                Console.WriteLine($"{motor.Name} speed: {actualSpeed:F2} mm/s at {voltage:F2} volts");

                totalSpeed += actualSpeed;
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
