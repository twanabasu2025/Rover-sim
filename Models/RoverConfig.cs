using System.Text.Json.Serialization;

namespace RoverCommander.Models
{
    public class RoverConfig
    {
        [JsonPropertyName("motors")]
        public List<Motor> Motors { get; set; } = new();

        [JsonPropertyName("batteries")]
        public List<Battery> Batteries { get; set; } = new();

        [JsonPropertyName("solar_panels")]
        public List<SolarPanel> SolarPanels { get; set; } = new();
    }

    public class Motor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("kv_rating")]
        public float KvRating { get; set; }

        [JsonPropertyName("current_rating")]
        public float CurrentRating { get; set; }

        [JsonPropertyName("wheel")]
        public Wheel Wheel { get; set; } = new();
    }

    public class Wheel
    {
        [JsonPropertyName("diameter")]
        public float Diameter { get; set; }

        [JsonPropertyName("gear_ratio")]
        public float GearRatio { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; } = new();
    }

    public class Position
    {
        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }
    }

    public class Battery
    {
        [JsonPropertyName("capacity")]
        public float Capacity { get; set; }

        [JsonPropertyName("max_voltage")]
        public float MaxVoltage { get; set; }
    }

    public class SolarPanel
    {
        [JsonPropertyName("efficiency")]
        public float Efficiency { get; set; }

        [JsonPropertyName("area")]
        public float Area { get; set; }
    }
}
