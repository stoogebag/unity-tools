# Game State

Generic game-state infrastructure with no game-specific assumptions: the save/restore contract and snapshot machinery, and objective state machines that consume existing gameplay events. Namespace `stoogebag.GameState`.

Sits below the interaction/activation families — it is the "what has happened and what still needs to happen" layer, not a *thing* in the world.

## The model

- **`ISaveable`** — the opt-in save hook. A `MonoBehaviour` implements `CaptureState()` / `RestoreState(object)` with a few lines and contains **no checkpoint logic**: no reference to a driver, no idea when or why it is captured. Boxed, in-memory `object` payloads — nothing is serialized to disk.
- **`SaveManager`** — static snapshot machinery: discovers every `ISaveable` in the loaded scenes, keys each by `(type full name, hierarchy path)`, captures the lot into a dictionary, and restores a snapshot back onto the fresh scene. No lifecycle of its own — a **persistent driver** owns the snapshot and calls it.
- **`Objective`** (abstract) — a minimal terminal state machine. `IsActive` / `IsComplete` (read-only reactive properties), `OnStarted` / `OnCompleted`, virtual `Progress` (float, for bars) and `ProgressText` (string, e.g. `"3/5"`), protected `Begin()` / `Complete()`. Implements `ISaveable` (captures `(Active, Complete)`). It owns **no activation trigger** — each concrete type decides when it starts.
- **`SourceObjective`** — names a **start** and a **finish** `ActivationSource`; begins when the start goes active, completes when the finish goes active. Covers reach-location (`TriggerVolume`) and activate-object (button).
- **`SurviveObjective`** — completes after a duration; `Progress` = elapsed/duration.

## Files

| File | What it does |
|---|---|
| `ISaveable.cs` | `object CaptureState()` / `void RestoreState(object)` — the whole contract. |
| `SaveManager.cs` | `FindSaveables()`, `Key()`, `Path()`, `CaptureAll()`, `RestoreAll(snapshot)`. |
| `Objective.cs` | Abstract terminal state machine + `ObjectiveState` save payload. |
| `SourceObjective.cs` | Completes when a finish `ActivationSource` goes active. |
| `SurviveObjective.cs` | Completes after a duration. |
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

- `SourceObjective` — drag a `TriggerVolume` (or button `ActivationSource`) into `finish`. The objective begins on enable (or when `start` goes active) and completes when `finish` goes active.
- `SurviveObjective` — set `duration`. Completes on its own.

Watch it from elsewhere via the observables: `IsActive`, `IsComplete`, `OnStarted`, `OnCompleted`, and `Progress` / `ProgressText` for a HUD.

Custom goals **derive from `Objective`** — that is the escape hatch, no interface needed:

```csharp
public class CountObjective : Objective
{
    private int _count;
    protected override void SomethingHappened() { if (++_count >= target) Complete(); }
}
```

`Begin()`/`Complete()` are the only transitions; completion is **terminal** — `Complete()` fires once and never re-runs, then `IsActive` goes false and `IsComplete` goes true.

## Notes

- **Namespace is `stoogebag.GameState`** — a deliberate deviation from the global namespace most of this library uses, to avoid owning a name as generic as `Objective` at global scope.
- **In the root `stoogebag` assembly** (no asmdef here); the folder inherits the root asmdef's UniRx / UniTask references.
- **`Objective` is deliberately *not* an `ActivationSource`.** Sources carry an `Active`/`Settled` duality ("settled" = after the visual finished); a goal has no visual step, and completion is terminal. `IsActive=false` on completion would be indistinguishable from a cancelled run. Promote it only when a real consumer appears.
- **Game-coupled subclasses live outside this folder.** `EncounterObjective` and `CountObjective` sit in the consuming project (they reference that project's `Encounter`, `Health`, `Pickup`), and subclass the base here. This folder must never reference them.
- **Boxed payloads are deliberate.** Disk persistence will require the captures to be serializable, which the boxed design does not address yet — see `Saving.md`.