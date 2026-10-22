# SURGE Drive — Proposed Verification Plan

**Status:** Proposed test cases only. **No tests have been run or claimed to pass.**

## Capture for every in-game run
KSP and Making History versions; OS; mod SHA/build; installed mod list; test craft; vessel mass; EC capacity/generation; flight scene and altitude; throttle; physics timestep (if available); KSP log and screenshots. Prefer a clean install and a dedicated test save.

| Test | Setup | Expected/observation to check | Trace |
|---|---|---|---|
| T-001 | One drive, throttle 0, EC available | No SURGE Δv or drive EC demand | F-003 |
| T-002 | 20 m/s², 50% throttle, EC available | ~10 m/s² commanded, step Δv≈10Δt | F-001 |
| T-003 | One active drive, EC depleted | No SURGE Δv after zero EC supply | F-004, Issue #1 |
| T-004 | Restore EC after T-003 | Document auto/manual restart behavior | Design decision |
| T-005 | EC below demand | Applied increment scales with actual fraction | F-005 |
| T-006 | One drive, 200 m/s², abundant EC | ~200 m/s² commanded | F-001 |
| T-007 | Two aligned 200 m/s² drives | Investigate ~400 m/s² and compare measured Δv | F-006, Issue #2 |
| T-008 | Four aligned 200 m/s² drives | Investigate ~800 m/s² and saturation | F-006, Issue #2 |
| T-009 | Two equal opposed drives | Record vector cancellation and EC consumption | F-006 |
| T-010 | Change mass while retaining settings/EC | Same commanded acceleration | F-002 |
| T-011 | Deactivate via PAW/action group | Zero SURGE increment; correct state | F-003 |
| T-012 | No target, zero closing rate, receding target | No misleading valid flip marker | F-010 |
| T-013 | Largely lateral target motion | Document flip-marker geometry error | F-011 |
| T-014 | Launch/reload scenes | No unintended residual throttle | Lifecycle |
| T-015 | Dock/undock/stage drive modules | No duplicate impulses/stale marker references | Future refactor |

## Issue #1 reproduction
Build craft with battery, SURGE drive, and no sustained generation (or switchable generator). In vacuum record velocity, EC remaining, EC requested and delivered, throttle and `isActive` before and after depletion. Control for gravity/other propulsion—zero **SURGE** acceleration does not imply zero *total* acceleration. Compare against existing `ecReceived <= 0` guard; do not claim it solves the issue without an in-game run.

## Issue #2 reproduction
Compare 1/2/4/8 aligned drives with adequate EC. Capture requested and received EC **for each drive**, requested acceleration, UI acceleration, and measured world-velocity change per tick. Repeat with EC starvation and opposed directions. Distinguish power-sharing, direction/frame errors, UI-only saturation and physics integration before diagnosing a cause.

## Future automation
Extract scalar EC calculations, vector sums, and flip timing into testable pure code; run unit tests and documentation lint in CI. License restrictions prohibit checking in KSP/Unity binaries. Test math is not a substitute for a KSP flight test.

## Completion criteria for later physics PRs
Record baseline reproduction, pass deterministic unit cases (or explain unavailable harness), supply before/after in-game measurements and logs, and document maintainer-approved changes to gameplay semantics.
