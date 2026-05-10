# Surge Drive Mod — Current State Summary

## Lore Basis
Inspired by the SURGE drive from the Bobiverse series (Dennis E. Taylor). The drive envelops the entire vessel in a field that acts on every particle equally, producing zero internal g-forces for occupants. Powered by large energy sources (fusion reactors in-lore; Electric Charge in KSP).

---

## Project Structure
```
warpdrive/
├── src/WarpEngineModule.cs       — PartModule implementation
├── libs/                         — KSP + Unity DLL references (not shipped)
├── GameData/WarpDriveMod/
│   ├── Plugins/WarpDriveMod.dll  — compiled output
│   └── warp_engine.cfg           — part definition
├── WarpDriveMod.csproj
└── PLAN.md
```

---

## Part Definition (`warp_engine.cfg`)
- **Name:** `WarpEnginePlate` / display title: **Surge Drive**
- **Model:** EP-50 Engine Plate (`SquadExpansion/MakingHistory`) — requires Making History DLC
- **Mass:** 1 tonne
- **Cost:** 12,000 / Entry cost: 45,000
- **Tech node:** `metaMaterials`
- **Category:** Propulsion
- **Attachment nodes:** Standard size-4 stack top/bottom, plus dynamic engine-cluster nodes (Single / Double / Triple / Quad) via `ModuleDynamicNodes`
- **No shroud**, no decoupler, no fuel resources

---

## PartModule (`WarpEngineModule.cs`)

### Physics Model
- Acceleration is mass-independent — `vessel.ChangeWorldVelocity(deltaV)` applies a direct velocity impulse each physics tick, bypassing F=ma. This is lore-accurate: the field acts on all particles equally.
- Thrust direction: `part.transform.up` — the physical "top" of the engine plate part
- `deltaV = acceleration × throttle × fixedDeltaTime` per tick

### Energy Consumption
- `EC/s = vessel total mass (t) × acceleration setting (m/s²) × throttle × 1.0`
- Drawn each fixed update via `part.RequestResource("ElectricCharge", ...)`
- If EC is insufficient, thrust scales down proportionally to EC received vs. required — no hard cutoff
- EC drain rate displayed live in PAW

### PAW (right-click in flight)
| Field | Description |
|---|---|
| Acceleration | Slider 0–200 m/s², default 20, step 1 |
| Warp Throttle | Read-only display of current main throttle % |
| Vessel Accel | Combined g-force from all active Surge Drives on vessel |
| EC Drain | Current power draw in EC/s |
| Enable/Disable Warp Engine | Toggle button, label flips |

### VAB (right-click in editor)
- Acceleration slider only

### Action Groups
- Activate Warp Engine
- Deactivate Warp Engine
- Toggle Warp Engine

### Known Behaviors / Implementation Notes
- `part.force_activate()` required in `OnStart` — without it KSP does not call `OnFixedUpdate` or `OnUpdate` on custom PartModules
- A one-frame coroutine resets the main throttle to 0 on scene load (otherwise it initializes at 50% due to axis group initialization)
- Multiple Surge Drives on one vessel stack additively — `Vessel Accel` display sums all active instances

---

## Open / Future Items
- Throttle reset to 0% on launch pad is a coroutine workaround — may need revisiting if it proves unreliable
- EC consumption not yet tested in-game
- Higher acceleration tiers (200+ m/s²) intended to require reactor mods
- Future: heat generation, failure states, visual effects, UI throttle window
