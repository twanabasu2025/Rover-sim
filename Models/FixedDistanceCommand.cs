using System.Text.Json.Serialization;

namespace RoverCommander.Models
{
    public class FixedDistanceCommand
    {
        [JsonPropertyName("duration")]
        public float Duration { get; set; }

        [JsonPropertyName("motor_commands")]
        public List<MotorCommand> MotorCommands { get; set; } = new();
    }

    public class MotorCommand
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("voltage")]
        public float Voltage { get; set; }
    }
}
