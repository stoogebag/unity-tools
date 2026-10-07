# Game State

Generic game-state infrastructure with no game-specific assumptions: the save/restore contract and snapshot machinery, and objective state machines that consume existing gameplay events. Namespace `stoogebag.GameState`.

Sits below the interaction/activation families — it is the "what has happened and what still needs to happen" layer, not a *thing* in the world.

## The model

- **`ISaveable`** — the opt-in save hook. A `MonoBehaviour` implements `CaptureState()` / `RestoreState(object)` with a few lines and contains **no checkpoint logic**: no reference to a driver, no idea when or why it is captured. Boxed, in-memory `object` payloads — nothing is serialized to disk.
- **`SaveManager`** — static snapshot machinery: discovers every `ISaveable` in the loaded scenes, keys each by `(type full name, hierarchy path)`, captures the lot into a dictionary, and restores a snapshot back onto the fresh scene. No lifecycle of its own — a **persistent driver** owns the snapshot and calls it.
- **`Objective`** (abstract) — a single state machine: `ObjectiveState { Unstarted, Running, Complete }` exposed as a read-only `State` reactive property, plus `IsUnstarted` / `IsRunning` / `IsComplete`. `Begin()` / `Satisfy()` / `Unsatisfy()` are the transitions; `oneShot` prevents leaving Complete. Virtual `Progress` / `ProgressText` for the HUD. Implements `ISaveable`.
- **`SourceObjective`** — satisfied by an `ActivationSource`: Complete while the `finish` source is active, reverts to Running when it goes inactive, unless `oneShot`. Optional `start` source begins it.
- **`ObjectiveGroup`** — an objective made of children. Begins its children when it begins, and is Complete while they satisfy a rule (All / Any / Count), with an `ordered` flag. Reverts when they stop.
- **`SurviveObjective`** — satisfied after a duration; `Progress` = elapsed/duration.

## Files

| File | What it does |
|---|---|
| `ISaveable.cs` | `object CaptureState()` / `void RestoreState(object)` — the whole contract. |
| `SaveManager.cs` | `FindSaveables()`, `Key()`, `Path()`, `CaptureAll()`, `RestoreAll(snapshot)`. |
| `Objective.cs` | Base: `ObjectiveState { Unstarted, Running, Complete }`, `oneShot`, `Begin`/`Satisfy`/`Unsatisfy`, `ObjectiveStateData` save payload. |
| `SourceObjective.cs` | Complete while a `finish` `ActivationSource` is active; reverts unless `oneShot`. |
| `ObjectiveGroup.cs` | Children + rule (All / Any / Count) + `ordered`; state derived from the children. |
| `SurviveObjective.cs` | Satisfied after a duration. |
| `Saving.md` | The move record and open questions (keying, disk persistence, the `LevelFlow` decouplings). |

## Saving: usage

1. Implement `ISaveable` on a `MonoBehaviour`:

```csharp
public class DoorState : MonoBehaviour, ISaveable
{
    public object CaptureState() => new DoorStateData { Open = _door.IsOpen };
    public void RestoreState(object state)
    {
        if (state is DoorStateData d) { /* apply through existing public APIs */ }
    }
}
```

2. A **persistent driver** (survives scene reloads) calls `SaveManager`:

```csharp
_snapshot = SaveManager.CaptureAll();      // at a checkpoint
SaveManager.RestoreAll(_snapshot);         // after the scene reloads
```

The key is derived from the component's hierarchy path, so a restored `ISaveable` matches by `(type, path)` — no wiring needed. Only components that implement `ISaveable` participate; everything else restores through its own public API.

## Objectives: usage

Drop an objective component in the scene and wire it:

- `SourceObjective` — drag a `TriggerVolume` (or any `ActivationSource`) into `finish`. It is Complete while the source is active, and reverts to Running when it goes inactive unless `oneShot` is ticked. A parent group (or a `start` source) begins it.
- `ObjectiveGroup` — list the children, pick All/Any/Count, tick `ordered` if they must complete in order. It begins its children when it begins.
- `SurviveObjective` — set `duration`.

Watch objectives via `State` (or the `IsRunning` / `IsComplete` helpers) and `Progress` / `ProgressText`.

Custom goals **derive from `Objective`** — call `Satisfy()` / `Unsatisfy()`:

```csharp
public class CountObjective : Objective
{
    private int _count;
    public void Increment() { if (++_count >= target) Satisfy(); }
}
```

`Begin()` / `Satisfy()` / `Unsatisfy()` are the only transitions; `oneShot` makes completion stick.

## Notes

- **Namespace is `stoogebag.GameState`** — a deliberate deviation from the global namespace most of this library uses, to avoid owning a name as generic as `Objective` at global scope.
- **In the root `stoogebag` assembly** (no asmdef here); the folder inherits the root asmdef's UniRx / UniTask references.
- **`Objective` is deliberately *not* an `ActivationSource`.** Sources carry an `Active`/`Settled` duality; an objective has no visual step. Completion is live — it reverts to Running unless `oneShot`.
- **Game-coupled subclasses live outside this folder.** `EncounterObjective` and `CountObjective` sit in the consuming project (they reference that project's `Encounter`, `Health`, `Pickup`), and subclass the base here. This folder must never reference them.
- **Boxed payloads are deliberate.** Disk persistence will require the captures to be serializable, which the boxed design does not address yet — see `Saving.md`.