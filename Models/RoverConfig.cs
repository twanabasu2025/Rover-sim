namespace RoverCommander.Models;

public class RoverConfig
{
    public List<Motor> Motors { get; set; }
    public List<Battery> Batteries { get; set; }
    public float GearRatio { get; set; }
    public float WheelDiameter { get; set; }

    public class Motor
    {
        public string Name { get; set; }
        public float Kv { get; set; }  // RPM per Volt
        public float CurrentRating { get; set; } // Amps
    }

    public class Battery
    {
        public float Voltage { get; set; }
        public float Capacity { get; set; } // Wh
    }
}
