# SURGE Lab v2 — Flight Dynamics Workbench

A standalone, dependency-free browser simulator. Open [index.html](index.html) in a browser. If GitHub Pages is enabled for the fork's `gh-pages` branch, the deployment URL is [jvholmes87.github.io/ksp-surge-drive](https://jvholmes87.github.io/ksp-surge-drive/).

## What v2 actually simulates

- **2D inertial flight:** ship and target positions/velocities, a plotted trajectory and moving-target track, integrated with a fixed 0.1-second numerical step.
- **Drive array:** 1–8 independently configurable engines, orientation relative to ship heading, enable switches, throttle and each drive's configurable 0–200 m/s².
- **Current-source equations:** per-drive requested EC/s = acceleration setting × throttle; delivered/requested EC scales acceleration; oriented acceleration vectors sum; zero EC produces no SURGE acceleration. The existing C# code calls `RequestResource` and `ChangeWorldVelocity` independently per drive.
- **Two *illustrative* power distribution strategies:** sequential per-drive budget (approximation for existing per-module requests) versus proportional allocation from a proposed shared bus.
- **Reactor and battery:** continuous EC generation, battery capacity, max discharge rate, depletion, recharge, power-limited acceleration and coasting.
- **Controls:** configurable target position/velocity, ship velocity/heading, manual guidance, approximate flip/approach and velocity-match guidance, presets, pause/step/reset and emergency cut.
- **Telemetry:** position and velocity components, relative/closing velocity, world acceleration vector, g-equivalent acceleration, approximate stopping distance, per-drive allocated EC and time-series plots.

## Engineering provenance

Main mod: `src/WarpEngineModule.cs`, read October 2026. Power-loss branch `fix/issue-1-power-loss-guard` contributes the zero/partial-power protection and bounded ratio concept; `fix/issue-2-multidrive-diagnostics` contributes the explicit vector and per-drive EC telemetry; `test/surge-physics-math-baseline` contributes scalar math tests. These are **separate unmerged branches**, not upstream features. SURGE Lab v2 interprets these calculations and adds a *proposed* 2D numerical model. It is not linked to the game's actual running assembly.

## Critical limitations

Not KSP, a validated physics simulation, or a verified fix to Issue #2. There is no KSP `RequestResource` graph, true resource crossfeed, physics callback ordering, patched conics, gravitational potential, orbital dynamics, attitude controls, collisions, drag or heat. Shared power is a hypothetical controller design; sequential budget is a simplification of stock part-by-part demand. Guidance is a heuristic and can miss targets. Stopping estimates assume constant acceleration; the ring drive is fiction, not a physical propulsion claim.

## Deployment and review

No build step, external libraries, or API calls. The feature is isolated in `tools/surge-lab` and can be merged independently. Run in a desktop or mobile browser and validate behavior, especially power depletion, opposed drives and intercept cases. GitHub source preview will **not execute** HTML; use Pages or open the HTML file directly.
