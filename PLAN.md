# KSP Warp Engine Mod — Project Plan (VS Code + Codex Workflow)

## Overview
This project creates a Kerbal Space Program (KSP) PartModule that turns the EP-50 Engine Plate into a warp engine.

Core behavior:
- No fuel usage
- Direct velocity manipulation
- Throttle-controlled warp acceleration
- Configurable max warp speed (default 100 m/s per second)

---

# 1. Project Goals

## MVP Features
- Custom PartModule: WarpEngineModule
- Throttle-controlled warp acceleration
- Max warp speed parameter (default 100)
- Works only in flight scene
- Reuses EP-50 engine plate model
- No fuel system

## Non-goals
- No custom models
- No shaders
- No advanced UI (yet)
- No orbital rewrite system

---

# 2. Development Environment

## Required Tools
- .NET SDK (installed via brew)
- VS Code
- C# Dev Kit extension

Install .NET:
brew install dotnet

Verify:
dotnet --version

---

## KSP Installation (macOS Steam)

KSP location:
~/Library/Application Support/Steam/steamapps/common/Kerbal Space Program/

Managed DLL location:
KSP.app/Contents/Resources/Data/Managed/

Required DLLs:
- Assembly-CSharp.dll
- UnityEngine.dll
- UnityEngine.CoreModule.dll

---

# 3. Project Structure

KSP_WarpMod/
├── src/
│   └── WarpEngineModule.cs
├── libs/
│   ├── Assembly-CSharp.dll
│   ├── UnityEngine.dll
│   └── UnityEngine.CoreModule.dll
├── GameData/
│   └── WarpDriveMod/
│       ├── Plugins/
│       └── warp_engine.cfg
├── WarpDriveMod.csproj
└── README.md

---

# 4. Setup Steps

## Step 4.1 — Create workspace

mkdir KSP_WarpMod
cd KSP_WarpMod
code .

---

## Step 4.2 — Copy KSP DLLs

Copy from KSP Managed folder into:

KSP_WarpMod/libs/

Files required:
- Assembly-CSharp.dll
- UnityEngine.dll
- UnityEngine.CoreModule.dll

---

## Step 4.3 — Create project file (csproj)

Target framework: net472

Project settings:
- OutputPath → GameData/WarpDriveMod/Plugins
- References to KSP Unity + Assembly-CSharp DLLs

---

# 5. Core Implementation

## WarpEngineModule (PartModule)

Behavior:
- Reads throttle (warpThrottle)
- Computes warp speed = maxWarpSpeed × throttle
- Applies velocity directly to vessel
- Runs in OnFixedUpdate only during flight

Key logic:
- forward direction = vessel.transform.up
- deltaV = forward × warpSpeed × fixedDeltaTime
- vessel.ChangeWorldVelocity(deltaV)

---

# 6. Part Configuration

File: warp_engine.cfg

Responsibilities:
- Reuse EP-50 engine plate model
- Attach WarpEngineModule
- Set cost, tech node, mass

Key module binding:
MODULE
{
    name = WarpEngineModule
    maxWarpSpeed = 100
}

---

# 7. Build System

Build command:
dotnet build

Output:
GameData/WarpDriveMod/Plugins/WarpDriveMod.dll

---

# 8. Installation

Copy entire folder:
GameData/WarpDriveMod/

Into:
KSP/GameData/

---

# 9. Testing Procedure

In KSP:

1. Go to VAB
2. Select EP-50 Warp Engine
3. Attach to vessel
4. Launch craft
5. Increase warp throttle

Expected behavior:
- Ship accelerates without fuel
- Velocity increases smoothly
- Direction follows vessel orientation

---

# 10. Debugging

Log file location:
~/Library/Logs/Unity/Player.log

Add debug lines in code:
Debug.Log("Warp throttle: " + warpThrottle);

---

# 11. Common Issues

Issue: Module not found
Fix: Ensure class name matches MODULE name exactly

Issue: No movement
Fix: Ensure warpThrottle > 0

Issue: DLL not updating
Fix: Fully restart KSP after rebuild

---

# 12. Codex Task Breakdown

Task 1:
- Create project structure
- Setup csproj
- Add DLL references

Task 2:
- Implement WarpEngineModule

Task 3:
- Create warp_engine.cfg

Task 4:
- Validate build pipeline

Task 5:
- Test in KSP flight scene

---

# 13. Future Enhancements

- Warp bubble mode (position-based movement)
- EC consumption system
- Heat generation and failure
- UI throttle window
- Maneuver node autopilot warp
- Visual distortion effects

---

# END