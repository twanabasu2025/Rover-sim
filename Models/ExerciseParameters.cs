using System.Text.Json.Serialization;

namespace RoverCommander.Models
{
    public class ExerciseParameters
    {
        [JsonPropertyName("fixed_distance")]
        public ExerciseValue FixedDistance { get; set; } = new();

        [JsonPropertyName("fixed_capacity")]
        public ExerciseValue FixedCapacity { get; set; } = new();

        [JsonPropertyName("fixed_irradiance")]
        public ExerciseValue FixedIrradiance { get; set; } = new();

        [JsonPropertyName("variable_irradiance")]
        public ExerciseValue VariableIrradiance { get; set; } = new();
    }

    public class ExerciseValue
    {
        [JsonPropertyName("value")]
        public float Value { get; set; }
    }
}
