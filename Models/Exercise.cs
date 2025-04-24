using System.Text.Json.Serialization;

public class Exercise
{
    [JsonPropertyName("fixed_distance")]
    public float FixedDistance { get; set; }
}
