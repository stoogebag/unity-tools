# Interaction

How world interaction works: pointing at things, focusing them, examining them, and interacting.

Live in this folder:

| Type | Role |
|---|---|
| `IInteractor` | The player-side contract. Whatever points at things implements this. |
| `Examinable` | Something that can be focused and examined. |
| `Interactable : Examinable` | Something that can additionally be interacted with. |
| `FirstPersonInteractor` | A ray-based `IInteractor`. Casts forward from an origin, tracks the target. |
| `Door` | An example interactable (uses `HasKey`). |
| `AnimateOnActivate`, `Cable`, `FloorTriggerZone` | Unrelated helpers that happen to live here. |

## The contract

`Examinable` and `Interactable` are event + observable pairs. The observable form is the contract; the `event` is the backing field.

```
Examinable   : OnFocus / OnUnfocus / OnExamine / OnUnExamine
Interactable : OnFocus / OnUnfocus / OnExamine / OnUnExamine / OnInteraction / OnInteractionCancelled
```

```csharp
public interface IInteractor
{
    Transform transform { get; }
    GameObject gameObject { get; }
    bool HasKey(string key);
    void Interacted(Interactable interactable);
}
```

`Interactable.Interact` raises `OnInteraction` and then calls `interactor.Interacted(this)`. Both directions are notified, deliberately: some interactables are self-contained (a `Door` opens itself), others need the interactor to handle it (a pickup that goes to the player's hands). This split is unresolved and the code says so; treat it as a convention rather than a rule.

## Minimal setup

1. Put `Examinable` (or `Interactable`) on the object. Set `popupName`, and set `FocusDistance` / `ExamineDistance` / `InteractDistance` if the defaults (10/10/5) don't suit.
2. Put a `FirstPersonInteractor` on the player, in its own child GameObject.
3. Wire the interactor's `_origin` to the camera transform and its `_interactAction` / `_inspectAction` to `InputActionReference` assets.

Samples for each interactable shape are in `Samples/`:

| Sample | What it shows |
|---|---|
| `Examinable.Sample` | Focus and examine only. `Interact` does nothing. |
| `Interactable.Sample` | Focus, examine, interact. |
| `Interactable.DistanceLimited.Sample` | `InteractDistance` gating — interact fails from far away. |
| `Door.Sample` | The `HasKey` path. |

## FirstPersonInteractor

Serialized fields:

| Field | Purpose |
|---|---|
| `_origin` | Where the ray starts and what direction it uses. Point at the camera. |
| `_range` | Ray length. |
| `_mask` | Layers to include. |
| `_ignore` | Layers to exclude. **Use this to exclude the interactor's own body.** |
| `_interactAction` | `InputActionReference` — press to interact. |
| `_inspectAction` | `InputActionReference` — press to examine. |

Runs every frame: raycast, resolve `Examinable` via `GetComponentInParent`, check `CanFocus`, then raise focus/unfocus when the target changes. `Current` is exposed as `IReadOnlyReactiveProperty<Examinable>` so prompt UI can subscribe instead of polling.

**The `_ignore` mask matters.** With `_mask` = everything, the ray starts inside the interactor's own capsule and can target itself. Excluding the player's layer is wrong if other players should be valid targets — hence a separate `_ignore` mask you can point at just your own body's layer.

## Adding an interactable

Implement the behaviour by subscribing to the observable, or by overriding `Interactable`:

```csharp
GetComponent<Interactable>().OnInteractionObservable
    .Subscribe(interactor => DoTheThing())
    .AddTo(this);
```

If the interactable needs the interactor to do something (hand an item over, open a UI), read it off the callback argument rather than reaching for a singleton.

## Multiplayer

**This system is local only, by design.** Focus, examine, and the interaction prompt are per-player state and must never be networked. Two players looking at different things is normal and correct.

Only the *effect* of an interaction may need networking, and that is the interactable's responsibility, not this system's. `FirstPersonInteractor` is owner-gated in FUN-eral: remote player instances have it disabled, along with input, look and camera.

Policy for FUN-eral: all interactions are single-user. Where two players contend for the same object (both grabbing a chair), first-grab-wins or last-grab-wins is resolved by whoever holds authority over that object — not by this system.

## Notes

- `Examinable` and `Interactable` are in the **global namespace** (the asmdef's `rootNamespace` is empty), matching the rest of this assembly.
- `Examinable.InteractText` is a get-only property with no body. Harmless while nothing reads it; give it a body or delete it before using it.
- `ButtonInteractable` references an `IInteractable` interface that is not defined anywhere. Dead code.
- Everything is behind `#if UNITASK` / `#if UNIRX` / `#if CINEMACHINE` guards supplied by the assembly's define constraints.
