# RoverCommander Simulation Client 🚀  
> Mars rover motion planning challenge — by Sushma Twanabasu

This project solves a simulation-based engineering challenge for NASA's Mars Endurance Rover. A simulator provides dynamic rover configurations and environmental inputs. The client application computes appropriate motor commands or estimations and validates them through a REST API.

---

## 👩‍💻 Author

**Sushma Twanabasu**  
.NET Developer | Systems Designer | Simulation Enthusiast

---

## 🧠 Project Overview

The application connects to the `rover_sim` HTTP server and solves the following four exercises:

1. **Fixed Distance**
   - Calculate voltages + duration to move a fixed distance

2. **Fixed Capacity**
   - Determine the maximum distance the rover can travel with partial battery charge

3. **Fixed Irradiance**
   - Estimate the maximum speed sustainable under constant solar input

4. **Variable Irradiance**
   - Determine how far the rover can travel in a full Martian day with varying sunlight

Each solution is computed based on rover motor specs, solar panel configuration, and battery capacity, and then POSTed to the server for validation.

---

## 🔧 Tech Stack

- [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- C# 11
- System.Text.Json
- `HttpClient`
- Docker (used to run the rover simulator)

---

## 📂 Project Structure

```
RoverCommander/
├── Models/                # DTOs and config models
├── Services/              # HTTP client for simulator API
├── Solvers/               # Logic to solve each challenge
├── Program.cs             # App entry point
├── RoverCommander.csproj  # .NET project file
```

---

## 🚀 How to Run the Project

### Step 1: Clone the Repo
```bash
git clone https://github.com/YOUR_USERNAME/RoverCommander.git
cd RoverCommander
```

### Step 2: Run the Simulator

#### Option A: Using Binary
```bash
./rover_sim
```

#### Option B: Using Docker
```bash
docker-compose up
```

Make sure the server shows:
```
Server is running on http://0.0.0.0:8080
GET /health
GET /rover/config
GET /exercises
```

---

### Step 3: Run the Application

```bash
dotnet build
dotnet run
```

You’ll see logs like:
```bash
--- 🛞 Solving Fixed Distance ---
Motor_A speed: 1203.42 mm/s
...
✅ Status: 200 OK
📝 Message: Rover moved successfully
```

---

## ✨ Features

- Clean async architecture
- Modular Solver classes
- Fully matches real-time simulator config
- JSON error handling and logging
- Fully ready for GitHub or delivery submission

---

## 📫 Contact

**Sushma Twanabasu**  
✉️ Email: sushma.twanabasu@example.com  
🌐 GitHub: https://github.com/YOUR_USERNAME

---

## ✅ Submission Status

- [x] All Solvers Implemented
- [x] Compilation ✅
- [x] JSON APIs Integrated
- [x] End-to-End Testing Passed
- [x] Finalized for submission
