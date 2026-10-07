# Game State (stoogebag)

Generic game-state infrastructure that is equally useful in any Unity game: the save/restore contract, the snapshot machinery, and the objective state machines.

**`Assets/stoogebag/GameState/`** — namespace `stoogebag.GameState`

| Type | Notes |
|---|---|
| `ISaveable.cs` | Pure contract: `object CaptureState()` / `void RestoreState(object)`. Implemented on a `MonoBehaviour` (keys derive from the transform's hierarchy path). |
| `SaveManager.cs` | Static snapshot machinery: `FindSaveables()`, `Key()`, `Path()`, `CaptureAll()`, `RestoreAll()`. No lifecycle of its own — a persistent driver (Boomer's `LevelFlow`) owns the snapshot and calls it. |
| `Objective.cs` | Base — the `ObjectiveState { Unstarted, Running, Complete }` state machine (`State`, `IsRunning` / `IsComplete`, `Progress` / `ProgressText`, `Begin()` / `Satisfy()` / `Unsatisfy()`, `oneShot`), implementing `ISaveable`. |
| `SourceObjective.cs` | Satisfied while a `finish` `ActivationSource` is active; reverts unless `oneShot`. |
| `ObjectiveGroup.cs` | An objective made of children (All / Any / Count, `ordered`). |
| `SurviveObjective.cs` | Satisfied after a duration. |

## What stays in Boomer

`Assets/Boomer/LevelFlow/` keeps the death-loop driver (`LevelFlow` — checkpoint identity, respawn pose, scene reload, one-frame-later player teleport) and its scene-side components (`Checkpoint`, `CheckpointInteractable`, `LevelFlowInput`, `LevelFlowDebug`) plus `PlayerState` and `DoorState` as `ISaveable` implementers. `LevelFlow` delegates the generic capture/restore mechanics to `SaveManager`.

`Assets/Boomer/Objectives/` keeps the Boomer-coupled concrete objectives — `EncounterObjective` (watches `Encounter.OnCompleted`) and `CountObjective` (counts `Health.OnDied` / `Pickup.OnPickedUp`) — as subclasses of the `stoogebag.GameState.Objective` base.

## The three decouplings (still open)

`Saving.md` used to list three couplings that must break before `LevelFlow` itself can move into stoogebag. The **minimal slice** moved: the interface + snapshot machinery + objectives base, leaving `LevelFlow`'s Boomer lifecycle untouched. The couplings remain, so `LevelFlow` still:

1. stores a respawn **pose** rather than the `Checkpoint` reference,
2. has `Checkpoint` find the player via `GetComponentInParent<PlayerMotor>()` (Boomer type),
3. teleports by scanning for a `KinematicCharacterMotor` whose `CharacterController is PlayerMotor`.

Until those break, `LevelFlow` is Boomer-bound. Nothing blocks the current arrangement.

## Still open

- **Keying** — hierarchy path (current, zero authoring) vs an explicit ID component (rename-proof, manual wiring).
- **Persistence** — in-memory boxed `object` today. A durable store requires the captures to be serializable, which the boxed design deliberately does not address.
- **`Objective` activation source** — the base is deliberately *not* an `ActivationSource` (an objective has no Active/Settled duality). Completion is live and reverts unless `oneShot`. Promote it only when a real consumer appears.