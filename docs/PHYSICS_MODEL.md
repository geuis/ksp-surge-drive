# SURGE Drive — Current Physics Model

**Status:** Documentation of existing `main`; not an implementation change.

## Symbols
- `a_i`: per-drive configurable acceleration (`maxWarpSpeed` in code), m/s².
- `u`: main throttle, 0–1.
- `Δt`: fixed physics step, seconds.
- `n_i`: part-local +up vector in world coordinates.
- `q_i`: requested ElectricCharge (EC) per second.
- `r_i`: actual/requested EC fraction.
- `Δv_i`: resulting velocity impulse, m/s.

## Implemented per-drive equations

```text
q_i            = a_i × u
EC_requested_i = q_i × Δt
r_i            = EC_received_i / EC_requested_i
Δv_i           = n_i × a_i × u × r_i × Δt
```

Each active `WarpEngineModule.OnFixedUpdate` independently calls `part.RequestResource("ElectricCharge", …)` and `vessel.ChangeWorldVelocity(Δv_i)`. The current EC law does **not** scale with vessel mass. EC is a KSP gameplay unit, not joules; the coefficient is a balancing choice, not a physical power law.

A zero-EC response deactivates that module and skips its impulse. Nonzero partial EC scales the impulse. Clamping `r_i` to [0,1] would be prudent in future implementation.

## Multiple drives and axes

Idealized aligned, fully powered drives would have `Δv_total = Σ_i (n_i × a_i × u × Δt)`. Different directions require **vector** addition. The code performs sequential velocity changes, so equivalence to one impulse and available-power handling must be demonstrated in-game. Issue #2 reports an approximately 30g ceiling; its cause is unknown.

## g-force display

The UI expresses acceleration divided by 9.80665 in equivalent `g` units. It does not establish actual internal forces on Kerbals. The fictional Bobiverse field premise is separate from KSP's underlying physics.

## Flip-marker approximation

`UpdateMarker` estimates braking distance `d=v_rel²/(2a)` and time until flip `t=(D-d)/closingRate`. The reported `v_rel` includes lateral speed, and `a` is a maximum summed setting rather than guaranteed available effective deceleration.

Known limitations: straight-line constant-acceleration assumption, changing target geometry and velocity, gravity, incomplete treatment of lateral motion and power shortfalls, and orbital position calculated without future SURGE impulses. This is a guidance aid, **not** an exact intercept or orbital solution.

## Future refactor proposal

Extract testable EC ratio, vector aggregation, and navigation math into pure functions; use one vessel-level controller to snapshot drives, allocate EC, and apply one world-velocity increment. Preserve module serialization and confirm lifecycle, docking, scene changes and timewarp in KSP before changing runtime logic.

Unit math alone cannot certify KSP resource routing, PartModule callbacks, or physics integration. See [TEST_PLAN.md](TEST_PLAN.md).
