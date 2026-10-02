# Saving / Level Flow (stoogebag)

Generic save/restore infrastructure. Currently lives in `Assets/Boomer/LevelFlow/` (`Boomer.LevelFlow`) as Stage 15's work; **it belongs in stoogebag** — would this be equally useful in a non-boomer game? Yes. Same call as interaction/activation.

This doc records the intended home and the decoupling needed before the move. The move is deferred; the code works as-is.

## Proposed layout

**`Assets/stoogebag/Saving/`** (global namespace, matching `Interaction/` / `Activation/`)

| Type | Notes |
|---|---|
| `ISaveable.cs` | Pure contract. Implements on a `MonoBehaviour` (the key is derived from `transform`). |
| `LevelFlow.cs` | The persistent driver: checkpoint + snapshot, capture/restore, reload. Should end up referencing **no** Boomer type and **no** `Checkpoint`. |
| `Checkpoint.cs` | Trigger + respawn pose. |
| `DoorState.cs` | Adapter over stoogebag `Door`. Both already in stoogebag. |

**`Assets/stoogebag/Interaction/CheckpointInteractable.cs`** — the interaction. Must sit in `stoogebag.interaction` (which references root) so it can reach `Interactable`; the rest goes in root `Saving/`, since root cannot reference `.interaction`.

**`Assets/Boomer/LevelFlow/`** — stays: `PlayerState` only (the game's player restorable), the test scene, docs. `LevelFlow.md` remains the Boomer-facing system doc.

## Decoupling needed before the move

Three incidental couplings were introduced; none are required.

1. **`LevelFlow` stores a pose, not the checkpoint.** It should store the active `Checkpoint` reference and read `RespawnPose` at respawn. Simpler and correct — do this regardless of the move.

2. **`Checkpoint` finds the player via `GetComponentInParent<PlayerMotor>()`.** Swap for a serialized `LayerMask` (or tag), matching `TriggerVolume`'s existing `mask` approach. Removes the only Boomer reference in `Checkpoint`.

3. **`LevelFlow` teleports by scanning for a `KinematicCharacterController`'s `CharacterController is PlayerMotor`.** Boomer-specific. Invert it: add `ISaveable.OnRespawn(Vector3, Quaternion)` and let `PlayerState` teleport itself. Then `LevelFlow` never knows a motor exists.

With those three, `LevelFlow` references only `ISaveable`, `Checkpoint`, `UnityEngine` and `SceneManager` — generic.

## The one real constraint

`ISaveable` must be a `MonoBehaviour` surface: keys are derived from the component's hierarchy path (`Key(ISaveable)` → `Component.transform`). That is `UnityEngine`, not Boomer. A pure non-`MonoBehaviour` data interface isn't possible with hierarchy-path keying; alternatively keying could move to an explicit ID component, which is the other open question.

## Still open

- **Keying** — hierarchy path (current, zero authoring) vs an explicit ID component (rename-proof, manual wiring). Path is fine while scenes are hand-built; an explicit ID is the rename-safe option if that becomes a problem.
- **Persistence** — in-memory boxed `object` today. A durable store (Stage 17) requires the captures to be serializable, which the boxed design deliberately does not address.
