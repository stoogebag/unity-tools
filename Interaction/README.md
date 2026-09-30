# Interaction

How world interaction works: pointing at things, focusing them, examining them, and interacting.

Live in this folder:

| Type | Role |
|---|---|
| `IInteractor` | The player-side contract. Whatever points at things implements this. |
| `Examinable` | Something that can be focused and examined. |
| `Interactable` (abstract) | Something that can be interacted with. Declares `TryInteract`; holds the performed/failed observables. |
| `SimpleInteractable` | The default concrete interactable — succeeds, does nothing. |
| `FirstPersonInteractor` | A ray-based `IInteractor`. Casts forward from an origin, tracks the target, sends "try". |
| `AnimateOnActivate`, `Cable`, `FloorTriggerZone` | Unrelated helpers that happen to live here. |

## The contract

`Examinable` is an `event` + observable pair (the observable form is the contract, the `event` is the backing field). `Interactable` uses plain UniRx `Subject`s, the same pattern as `Health.OnDamaged`.

```
Examinable   : OnFocus / OnUnfocus / OnExamine / OnUnExamine
Interactable : OnInteractPerformed / OnInteractFailed
```

```csharp
public interface IInteractor
{
    Transform transform { get; }
    GameObject gameObject { get; }
    bool HasKey(string key);
}

public abstract class Interactable : Examinable
{
    public abstract bool TryInteract(in InteractionContext ctx);
    public Subject<InteractionContext> OnInteractPerformed { get; }
    public Subject<InteractionContext> OnInteractFailed { get; }
}
```

The interactor builds an `InteractionContext` (itself + the target) and calls `TryInteract` on the interactable. **The interactable owns the decision**: it evaluates its own conditions, does its effect on success, and pushes `OnInteractPerformed` — or `OnInteractFailed` if it declined. The `bool` return is for the caller's immediate feedback; the observables are for decoupled observers.

The interactor never subscribes and never does actor-side work itself — it only detects and sends "try". Reach (how far you can interact) is the interactor's property, not the object's.

## Minimal setup

1. Put `SimpleInteractable` (or your own `Interactable` subclass) on the object. Set `popupName`.
2. Put a `FirstPersonInteractor` on the player, in its own child GameObject.
3. Wire the interactor's `_origin` to the camera transform, its `_interactAction` / `_inspectAction` to `InputActionReference` assets, and its `_range` (reach).

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

Runs every frame: raycast, resolve `Examinable` via `GetComponentInParent`, raise focus/unfocus when the target changes, and on interact press call `TryInteract` on the target if it is an `Interactable`. `Current` is exposed as `IReadOnlyReactiveProperty<Examinable>` so prompt UI can subscribe instead of polling.

**The `_ignore` mask matters.** With `_mask` = everything, the ray starts inside the interactor's own capsule and can target itself. Excluding the player's layer is wrong if other players should be valid targets — hence a separate `_ignore` mask you can point at just your own body's layer.

## Adding an interactable

Subclass `Interactable` and override `TryInteract`, or subscribe to another interactable's observable:

```csharp
public class MyThing : Interactable
{
    public override bool TryInteract(in InteractionContext ctx)
    {
        if (!Ready) { OnInteractFailed.OnNext(ctx); return false; }
        DoTheThing();
        OnInteractPerformed.OnNext(ctx);
        return true;
    }
}
```

To react to something else being interacted with, hold a reference to it and subscribe:

```csharp
button.OnInteractPerformed
    .Subscribe(_ => Open())
    .AddTo(this);
```

References stay explicit and serializable (hold the `Interactable`, not an interface).

## Multiplayer

**This system is local only, by design.** Focus, examine, and the interaction prompt are per-player state and must never be networked. Two players looking at different things is normal and correct.

Only the *effect* of an interaction may need networking, and that is the interactable's responsibility, not this system's. `FirstPersonInteractor` is owner-gated in FUN-eral: remote player instances have it disabled, along with input, look and camera.

Policy for FUN-eral: all interactions are single-user. Where two players contend for the same object (both grabbing a chair), first-grab-wins or last-grab-wins is resolved by whoever holds authority over that object — not by this system.

## Notes

- The types are in the **global namespace** (the asmdef's `rootNamespace` is empty), matching the rest of this assembly.
- `Examinable.InteractText` is a get-only property with no body. Harmless while nothing reads it; give it a body or delete it before using it.
- `ButtonInteractable` references an `IInteractable` interface that is not defined anywhere. Dead code.
- Everything is behind `#if UNITASK` / `#if UNIRX` / `#if CINEMACHINE` guards supplied by the assembly's define constraints.
