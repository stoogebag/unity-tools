# Interaction

How world interaction works: pointing at things, focusing them, examining them, and interacting.

## The model

The interactor casts a ray, finds the `Examinable` under the crosshair, and works out **what pressing interact would do right now**:

1. Every frame, while something is focused, the interactor builds a list of **offered interactions**.
2. Sources are asked in order: first any registered `IInteractionProvider`s (e.g. a carried item), then the focused object if it is an `Interactable`.
3. The first offered `IInteraction` whose `CanPerform` is true becomes `Current` — that is the prompt, and what a press would do.
4. On press, the interactor calls `Perform`. It re-checks `CanPerform` first, so state can change between hover and press.

**Interactions are ephemeral.** They are rebuilt every frame and thrown away. Never subscribe to an `IInteraction`; watch the interactor's `OnPerformed`, or a persistent object's own observables.

**Examine is a separate channel.** Pressing inspect focuses the `Examinable` and raises `OnExamine` / the interactor's `OnExamined`. Examine is not offered and cannot be suppressed.

| Type | Role |
|---|---|
| `IInteractor` | The player-side contract: `transform`, `gameObject`, `HasKey`. |
| `Examinable` | Something that can be focused and examined. |
| `Interactable` (abstract) | An `Examinable` that offers interactions. Declares `OfferInteractions`; carries the performed/failed subjects. |
| `IInteraction` | One thing the player could do: `Text`, `CanPerform`, `Perform`. Ephemeral. |
| `IInteractionProvider` | An extra source of interactions that is not the target (carried item, equipment). |
| `FirstPersonInteractor` | A ray-based `IInteractor`. Tracks the target, resolves `Current`, performs on press. |
| `Door` | Pure motion (`Open`/`Close`/`Toggle`). No interaction of its own — something else offers it. |
| `Carryable`, `ItemCarrier`, `GrabInteraction`/`DropInteraction` | The worked example: an `IInteractionProvider` offers grab/drop for a `Carryable`. |
| `AnimateOnActivate`, `Cable`, `FloorTriggerZone` | Unrelated helpers that happen to live here. |

## Contracts

```csharp
public interface IInteractor
{
    Transform transform { get; }
    GameObject gameObject { get; }
    bool HasKey(string key);
}

public interface IInteraction
{
    string Text { get; }                      // prompt, e.g. "Grab chair"
    bool CanPerform(IInteractor interactor);  // re-checked at press time
    void Perform(IInteractor interactor);     // calls a method on a persistent object
}

public interface IInteractionProvider
{
    bool SuppressOtherInteractions { get; }   // true replaces all other offers (e.g. while carrying)
    void OfferInteractions(IInteractor interactor, Examinable target, List<IInteraction> into);
}

public abstract class Interactable : Examinable
{
    public virtual void OfferInteractions(IInteractor interactor, List<IInteraction> into) { }
    public Subject<InteractionContext> OnInteractPerformed { get; }
    public Subject<InteractionContext> OnInteractFailed { get; }
}
```

`Interactable` is an abstract `MonoBehaviour`, not an interface, so it can be held as a serializable object reference and stays polymorphic in the inspector.

## Minimal setup

1. Put a `FirstPersonInteractor` on the player, in its own child GameObject, and wire `_origin` to the camera, `_interactAction` / `_inspectAction` to `InputActionReference`s, and `_range` (reach).
2. Make something focusable by putting an `Examinable` (or an `Interactable`) on it.
3. To make it do something, subclass `Interactable` and override `OfferInteractions`.

## Making something interactable

Subclass `Interactable`, override `OfferInteractions`, and hand it an `IInteraction`:

```csharp
public class Lever : Interactable
{
    private LeverInteraction _pull;

    private void Awake() => _pull = new LeverInteraction(this);

    public override void OfferInteractions(IInteractor interactor, List<IInteraction> into)
        => into.Add(_pull);
}

public sealed class LeverInteraction : IInteraction
{
    private readonly Lever _lever;
    public LeverInteraction(Lever lever) => _lever = lever;

    public string Text => "Pull";
    public bool CanPerform(IInteractor interactor) => !_lever.IsInMotion;
    public void Perform(IInteractor interactor) => _lever.Toggle();
}
```

The gate lives in `CanPerform`; the effect in `Perform`. If a provider is `SuppressOtherInteractions`, the target's own offers are skipped entirely — used by `ItemCarrier` so that carrying a key means you cannot grab a chair.

## FirstPersonInteractor

Serialized fields:

| Field | Purpose |
|---|---|
| `_origin` | Where the ray starts and what direction it uses. Point at the camera. |
| `_range` | Ray length (interaction reach). |
| `_mask` | Layers to include. |
| `_ignore` | Layers to exclude. **Use this to exclude the interactor's own body.** |
| `_interactAction` | `InputActionReference` — press to interact. |
| `_inspectAction` | `InputActionReference` — press to examine. |

Exposes `Target` (`IReadOnlyReactiveProperty<Examinable>`), `Current` (`IReadOnlyReactiveProperty<IInteraction>`), `OnPerformed`, `OnExamined`, and `RegisterProvider` / `UnregisterProvider`.

**The `_ignore` mask matters.** With `_mask` = everything, the ray starts inside the interactor's own capsule and can target itself. Excluding the player's layer is wrong if other players should be valid targets — hence a separate `_ignore` mask you can point at just your own body's layer.

## Deprecated / needs work (deferred)

- **`AnimateOnInteract` is stale and deprecated.** It predates the offered-interaction model and still calls the removed `Interactable.OnInteractionObservable`, so it is broken (currently compiled out behind `ODIN_INSPECTOR`, which hides it). Rework it onto `IInteraction`, or delete it.
- **Dialogue needs fixing.** `DialogueTrigger` subscribes to `Interactable.OnInteractPerformed`, but a dialogue object does not offer any interaction yet, so nothing resolves and the subject never fires. It needs its own `IInteraction` (or a generic "interact" offer) before dialogue works again.
- **`ButtonInteractable`** references an `IInteractable` interface that is not defined anywhere. Dead code (compiled out behind `UI_SHAPES_KIT`).

## Multiplayer

**This system is local only, by design.** Focus, examine, and the interaction prompt are per-player state and must never be networked. Two players looking at different things is normal and correct.

Only the *effect* of an interaction may need networking, and that is the interactable's responsibility, not this system's. `FirstPersonInteractor` is owner-gated in FUN-eral: remote player instances have it disabled, along with input, look and camera.

Policy for FUN-eral: all interactions are single-user. Where two players contend for the same object (both grabbing a chair), first-grab-wins or last-grab-wins is resolved by whoever holds authority over that object — not by this system.

## Notes

- The types are in the **global namespace** (the asmdef's `rootNamespace` is empty), matching the rest of this assembly.
- `Examinable.InteractText` is a get-only property with no body. Harmless while nothing reads it; give it a body or delete it before using it.
- Everything is behind `#if UNITASK` / `#if UNIRX` / `#if CINEMACHINE` guards supplied by the assembly's define constraints.
