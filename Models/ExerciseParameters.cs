using System.Text.Json.Serialization;

namespace RoverCommander.Models
{
    public class ExerciseParameters
    {
        [JsonPropertyName("fixed_distance")]
        public ExerciseValue FixedDistance { get; set; } = new();

        [JsonPropertyName("fixed_capacity")]
        public FixedCapacity FixedCapacity { get; set; } = new();

        [JsonPropertyName("fixed_irradiance")]
        public ExerciseValue FixedIrradiance { get; set; } = new();

        [JsonPropertyName("variable_irradiance")]
        public VariableIrradiance VariableIrradiance { get; set; } = new();
    }

    public class ExerciseValue
    {
        [JsonPropertyName("value")]
        public float Value { get; set; }
    }

    public class FixedCapacity
    {
        [JsonPropertyName("state_of_charge")]
        public float StateOfCharge { get; set; }
    }

    public class VariableIrradiance
    {
        [JsonPropertyName("peak_value")]
        public float PeakValue { get; set; }
    }
}
