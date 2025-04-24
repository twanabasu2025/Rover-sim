# RoverCommander

This is a .NET console application that interacts with a simulated Mars rover system. The simulator exposes a REST API with dynamic rover configurations. This application calculates and submits answers to four engineering challenges using the provided data.

---

## Author

Sushma Twanabasu  
Email: sushma.twanabasu@example.com  
GitHub: https://github.com/twanabasu2025

---

## Project Summary

The simulator provides a new rover configuration and challenge inputs on every run. The application performs calculations based on motor specs, wheel dimensions, gear ratios, battery capacity, and solar panel characteristics.

Implemented solvers:

- Fixed Distance: Calculate voltage and duration per motor to move a specified distance.
- Fixed Capacity: Estimate how far the rover can travel given a battery state of charge.
- Fixed Irradiance: Determine max sustainable speed under constant sunlight.
- Variable Irradiance: Estimate total distance traveled in one Martian day with sinusoidal solar input.

---

## Technologies

- .NET 9
- C# 11
- System.Text.Json
- HttpClient
- Docker (used to run the simulator)

---

## Project Structure

```
RoverCommander/
├── Models/                // Data structures
├── Services/              // API communication
├── Solvers/               // Solver logic for each challenge
├── Program.cs             // Entry point
├── RoverCommander.csproj  // Project file
```

---

## How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/twanabasu2025/RoverCommander.git
   cd RoverCommander
   ```

2. Start the simulator:
   - Run the binary directly:
     ```bash
     ./rover_sim
     ```
   - Or use Docker:
     ```bash
     docker-compose up
     ```

3. Run the application:
   ```bash
   dotnet build
   dotnet run
   ```

Each solver will run in sequence and post its result to the simulator. Results will be printed to the terminal.
