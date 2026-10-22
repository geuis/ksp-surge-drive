# SURGE Drive — Proposed Requirements Baseline

**Status:** Proposed for maintainer review; not accepted requirements or claims of passing tests. **Scope:** KSP 1.x implementation on `main`, reviewed 2026-10-23.

"Shall" indicates a verifiable proposed requirement. Implementation changes belong in later PRs.

| ID | Requirement | Verification |
|---|---|---|
| SURGE-F-001 | Enabled drive shall command a velocity increment along its local-up axis proportional to acceleration setting and throttle. | Unit/in-game |
| SURGE-F-002 | Commanded acceleration shall be independent of vessel mass given sufficient ElectricCharge (EC). | Unit/in-game |
| SURGE-F-003 | Disabled drive, zero throttle or zero acceleration shall produce no SURGE increment. | Unit/in-game |
| SURGE-F-004 | A physics step receiving zero EC shall produce no SURGE increment. | Unit/in-game |
| SURGE-F-005 | Partial EC supply shall proportionally scale the increment, with ratio bounded to [0,1]. | Unit/in-game |
| SURGE-F-006 | Multiple drives shall have a documented, deterministic vector-summation and power-allocation policy. | Design decision/in-game |
| SURGE-F-007 | User-facing acceleration shall use m/s². | Inspection |
| SURGE-F-008 | Telemetry shall distinguish requested and effective acceleration. | Inspection/in-game |
| SURGE-F-009 | Telemetry shall not assume unreceived EC. | Unit/in-game |
| SURGE-F-010 | Invalid or unusable flip guidance shall be hidden. | Unit/in-game |
| SURGE-F-011 | Flip recommendations shall disclose simplifying assumptions and not imply exact orbital intercepts. | Inspection/in-game |
| SURGE-I-001 | The mod shall load `WarpEngineModule` from `SurgeDriveMod.dll`. | Build/in-game |
| SURGE-I-002 | DLC dependency on Making History EP-50 shall be documented. | Inspection |
| SURGE-I-003 | KSP and Unity proprietary DLLs shall not be committed or redistributed. | Repository inspection |
| SURGE-Q-001 | Physics and power changes shall include reproducible single-drive, multi-drive, loss/partial-power and disabled-state tests. | Tests/in-game |
| SURGE-Q-002 | Releases shall document supported KSP versions, installation and limitations. | Inspection |

## Decisions for maintainer
- **Power law:** code currently uses EC/s = configured acceleration × throttle **per drive**, while original README claimed mass scaling. Keep or revise via separate implementation PR.
- **Drive stacking:** define handling of opposed and differently oriented drive axes, and power sharing.
- **Power loss:** current zero-EC branch sets `isActive=false`; confirm manual restart versus automatic recovery.
- **Displayed g:** 9.80665 m/s² conversion is *not* proof of inertial loading on occupants.
- **Navigation:** current marker is an approximation; orbital trajectory guidance is out of scope.

## Traceability
| Requirement | Code evidence | Known risk |
|---|---|---|
| F-001–F-005 | `src/WarpEngineModule.cs:OnFixedUpdate` | Issue #1; in-game testing pending |
| F-006, Q-001 | Per-part `OnFixedUpdate` | Issue #2 (~30g) |
| F-007–F-009 | `WarpEngineModule.cs`, `warp_engine.cfg` | `maxWarpSpeed` represents acceleration |
| F-010–F-011 | `UpdateMarker`, `WarpMarkerRenderer` | Approximate flip geometry |
| I-001–I-003 | `SurgeDriveMod.csproj`, `.gitignore`, part config | Outdated documentation paths |

Changes to gameplay semantics or persisted fields require explicit migration planning and new tests. No requirements are asserted as already verified.
