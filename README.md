# Rover-sim

A .NET console app that interacts with a Mars rover simulator via HTTP. The simulator provides randomized rover configurations, and this app calculates and posts answers to a set of challenges involving movement, power, and energy estimation.

---

## Author

Sushma Twanabasu  
GitHub: https://github.com/twanabasu2025  
Email: twanabasu1992@gmail.com

---

## What This Project Does

Each time the simulator starts, it exposes a REST API with details about the rover's motors, wheels, batteries, and solar panels. Based on that config, the app solves four challenges:

- Fixed Distance: Calculate how long and at what voltage to run each motor to cover a given distance.
- Fixed Capacity: Estimate the max travel distance using a given battery state of charge.
- Fixed Irradiance: Determine a sustainable speed using constant solar input.
- Variable Irradiance: Estimate the distance the rover can travel in a full Martian day with variable sunlight.

---

## Tech Used

- .NET 9
- C#
- System.Text.Json
- HttpClient
- Docker (to run the simulator)

---

## Folder Structure

```
Rover-sim/
├── Models/         // Data models for simulator input/output
├── Services/       // API client
├── Solvers/        // Code for solving each challenge
├── Program.cs      // Entry point
├── RoverCommander.csproj
```

---

## Running the Project

### 1. Clone the repository
```bash
git clone https://github.com/twanabasu2025/Rover-sim.git
cd Rover-sim
```

### 2. Start the simulator

Option A: Run the binary  
```bash
./rover_sim
```

Option B: Use Docker  
```bash
docker-compose up
```

Make sure the simulator logs say it's running on port 8080.

---

### 3. Run the app
```bash
dotnet build
dotnet run
```

---

## What You’ll See

The app will:
- Fetch the current rover config and challenges
- Run each solver
- Post results to the simulator
- Print out response messages for each task

Example:
```
Solving: Fixed Distance
Motor_A speed: 762.34 mm/s at 18.40 V
Response: 200 OK — Rover moved the correct distance
```

---

## Notes

- The simulator returns random configs every time, so results vary on each run.
- No hardcoded values. Everything is based on the live config.
- All code is async and modular.

## Example Output

Here’s what success looks like when the solution runs:

![Simulation Output](/screenshots/1.png)