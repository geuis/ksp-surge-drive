# Issue #1 — Loss-of-power regression procedure

**Status:** Manual verification required. The existing code already included a zero-EC cutoff; this change hardens its input handling and reports the drive's state. It is **not** evidence that the original bug is resolved.

## Environment
Record KSP version, Making History DLC version, installed mods, platform, Git commit, vessel craft file, EC generation/storage and timewarp state. Run once with a single SURGE drive and once with multiple drives.

## Reproduce and measure
1. Assemble craft with a SURGE drive, finite battery EC and *no* functioning EC generation; minimize alternative propulsion.
2. In a vacuum or stable coast, record velocity, throttle, EC, `SURGE State` and `Vessel Accel` before enabling the drive.
3. Enable the drive and set nonzero throttle. Confirm `SURGE State` shows `Accelerating` or `Power limited`.
4. Allow battery to drain with throttle nonzero. At the first fixed update reporting zero received EC, the drive should display `No EC / disabled` and add **no** further SURGE delta-v.
5. Distinguish persistent *velocity* (normal coasting) from continued *acceleration*. Gravity may still change world velocity; compare to a control vessel or disable all other acceleration sources.
6. Refill battery without manually reactivating: verify the drive remains disabled under current semantics. Reactivate and record what happens.
7. Repeat with EC partially limited and with two drives to detect per-part power allocation effects.

## Acceptance criteria
- No SURGE velocity impulse on invalid/zero delivered EC or invalid fixed timestep.
- Effective EC ratio bounded to 0–1, without unpowered acceleration.
- PAW status identifies disabled, starved, partially powered and powered states.
- Baseline and post-change **KSP logs or recordings** show whether the original Issue #1 symptom is reproducible and resolved.
- No claim of passing test is made before an actual in-game run.

## Scope boundary
This PR does not change save-persisted `isActive`, the current manual restart policy, the power-demand equation, or the per-drive independent impulse architecture. [Issue #2](https://github.com/geuis/ksp-surge-drive/issues/2) is separate.
