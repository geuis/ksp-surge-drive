# SURGE Drive — Architecture Baseline

**Status:** Current-state documentation with a non-binding future design option. No runtime code changes.

## Current components

| Component | Responsibility |
|---|---|
| `src/WarpEngineModule.cs` | KSP `PartModule`: throttle, EC, per-tick velocity changes, PAW telemetry, flip estimate |
| `src/WarpMarkerRenderer.cs` | Unity `MonoBehaviour`: map-view flip indicator |
| `GameData/SurgeDriveMod/warp_engine.cfg` | EP-50 part and module configuration |
| `SurgeDriveMod.csproj` | .NET Framework 4.7.2 assembly build and optional deploy |
| `KSPPath.props.example` | Optional per-developer installation path |
| `libs/` | Locally supplied KSP/Unity references; ignored from Git |

## Current runtime flow

```text
Vessel throttle + individual part state
              |
              v
     WarpEngineModule (each part)
          |      |
     Request EC  calculate available fraction
          |      |
          +------+
              |
    ChangeWorldVelocity(Δv) --> KSP vessel
              |
       PAW telemetry
              |
       Primary module only
              v
         Flip estimate
              |
      WarpMarkerRenderer (map)
```

Outside editor, `OnStart` calls `part.force_activate()` and subscribes to `onFlightReady` to reset throttle. `OnDestroy` unsubscribes. Every drive's physics callback requests power and changes vessel velocity independently. `OnUpdate` enumerates drives for displayed vessel acceleration and primary-drive marker selection.

Dependencies: KSP game APIs, Unity GUI/physics classes, Making History EP-50 part model, local game assemblies. The repository must not redistribute proprietary DLLs.

## Proposed architecture (future PR; not accepted)

```text
WarpEngineModule(s) [configuration + orientation + active state]
           |
           v
VesselSurgeController [one vessel tick / EC coordination]
           +--> Pure Physics Calculator
           +--> Telemetry / Diagnostics
           +--> Navigation prediction
           |
           v
   One ChangeWorldVelocity operation
```

Benefits: deterministic aggregation, clearer power accounting, focused diagnostics for [Issue #2](https://github.com/geuis/ksp-surge-drive/issues/2), independently testable math.

Decisions/risks: splitting/docking vessels; resource crossfeed and priority; KSP callback ordering; module lifecycle; stage removal; other mods; timewarp and rails transitions; backwards compatibility of persisted `WarpEngineModule` and `maxWarpSpeed` values.

## Engineering rules

1. Distinguish requested versus effective acceleration and EC.
2. Record reference frames for orientation, relative position and velocity.
3. Do not estimate powered braking from unavailable power.
4. Avoid renaming persisted fields without save compatibility strategy.
5. Unit-test calculations but reproduce engine behavior in the actual game.
6. Do not commit game DLLs.

## Existing issues

- [#1: acceleration continues after power loss](https://github.com/geuis/ksp-surge-drive/issues/1) — current code has a zero-EC guard, but in-game fix not verified.
- [#2: multiple drives limited to ~30g](https://github.com/geuis/ksp-surge-drive/issues/2) — cause unknown.
- README and legacy PLAN have obsolete module paths and physics descriptions.
