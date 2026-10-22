# Issue #2 — Multi-drive ~30g investigation

**Status:** Diagnostic instrumentation; **not** a verified bug fix. No in-game measurements or builds have been completed.

## Added in flight PAW (on first installed SURGE drive)
- **SURGE Drives:** active modules counted on the vessel.
- **SURGE Vector Accel:** magnitude of vector-summed per-drive effective acceleration, m/s².
- **EC Requested / EC Supplied:** sums of per-module rates from the last physics update, EC/s.
- Existing **Vessel Accel** is the sum of scalar effective accelerations, shown in g; it can exceed vector magnitude if drives point differently.

The existing per-drive EC request and `ChangeWorldVelocity` behavior has **not** been refactored; these values help diagnose the issue but do not independently prove the physical result.

## Experimental matrix
1. Use a KSP install with the Making History DLC and known mod versions. Record game, OS, craft, branch SHA, vessel mass, generators, battery EC, throttle, and physics rate.
2. Test in stable spaceflight away from substantial gravity and without other engines. Control for gravity when comparing world velocity.
3. At full throttle and 200 m/s² per drive, compare 1, 2, 4, and 8 drives mounted in the **same direction** and powered by abundant EC.
4. Record drive count, scalar g readout, vector acceleration, EC requested, EC supplied, and independently observed delta-v each second.
5. Repeat with two opposed drives (vector acceleration should approach zero) and two perpendicular drives (vector magnitude differs from scalar sum).
6. Repeat the aligned cases with deliberately limited EC, then restored EC.

| Drives | Aligned ideal command | Equivalent g |
|---:|---:|---:|
| 1 | 200 m/s² | 20.39 |
| 2 | 400 m/s² | 40.79 |
| 4 | 800 m/s² | 81.58 |
| 8 | 1,600 m/s² | 163.15 |

These are arithmetic reference values only, **not measurements**.

## Diagnosis decision tree
- **EC supplied below requested:** investigate stock resource graph, battery connection, flow rules, rates and generator outputs.
- **Vector acceleration below scalar:** inspect individual drive mounting direction/orientation.
- **Both UI accelerations meet expectation but measured delta-v does not:** investigate `ChangeWorldVelocity`, multiple calls per tick, vessel physics, timestep and other plugins.
- **Both UI accelerations plateau despite sufficient EC:** inspect active status, throttle, persisted acceleration settings and module callbacks.
- **Data changes after reload/staging/docking:** inspect module lifecycle, state restoration and reference selection.

## Acceptance for a future corrective PR
Before-and-after recorded KSP reproduction; explicit diagnosis; deterministic test cases; no regression of single-drive or power-loss behavior. If the future solution moves to a vessel-level controller, stage it in a separate PR to keep this diagnostic review small.
