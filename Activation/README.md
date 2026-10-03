# Activation

How one thing switches on/off and other things react — buttons, levers, powered panels, and chains of effects. Sits below [`../Interaction/`](../Interaction/README.md): interaction decides *when* something is pressed; activation is what a press *turns on*, and who reacts to that.

## The model

- **`ActivationSource`** — an on/off state:
  - `IsActive` — the intent. Flips immediately when something is pressed.
  - `IsSettled` — after the visual has finished. A source with no visual mirrors `IsActive`.
- **`ActivationTransition : ActivationSource`** — a visual step. Watches its upstream sources, plays its effects, and re-emits `Settled` when they finish. Because it *is* an `ActivationSource`, things can wait on it and it can chain.
- **Effects** — cosmetics with a duration, driven by a 0..1 progress (see below).
- **`ActivationReaction`** — a terminal consequence: watches one or more sources (all-active) and fires `OnActivated` / `OnDeactivated`.
- **`TriggerVolume : ActivationSource`** — drives its source from a trigger collider. `IsActive` mirrors presence; `TriggerCount` / `Occupants` and `HasBeenTriggered` / `Inside` expose history and occupancy.

A consumer chooses whether it watches a source's `IsActive` (react now) or `IsSettled` (react after the visual). That is a property of the *consequence*, not of the source.

## Files

| File | What it does |
|---|---|
| `ActivationSource.cs` | The on/off state (`IsActive`, `IsSettled`, `SetActive`). The referencable thing reactions watch. Abstract class so it is a serializable inspector reference. |
| `ActivationTransition.cs` | The visual step. Watches sources, plays registered effects forward / backward, re-emits `Settled`, and cancels/reverses cleanly. |
| `ITransitionEffect.cs` | `ITransitionEffect` (the effect contract) plus `TransitionEffect`, the base that owns `Duration`, `Ease`, `Progress` and the progress loop. |
| `MoveEffect.cs` | Moves a transform between two pose markers (hinge = same position; slide = same rotation). |
| `ColorEffect.cs` | Lerps a renderer colour via a `MaterialPropertyBlock` (no material instancing). |
| `TimerEffect.cs` | A beat with no visual — its duration alone delays `Settled`. |
| `ActivationReaction.cs` | Consequence base: `sources` list, `waitForSettled`, all-active (AND), `OnActivated` / `OnDeactivated`. |
| `TriggerVolume.cs` | An `ActivationSource` driven by a trigger collider: `IsActive` mirrors presence; `TriggerCount` / `Occupants` / `HasBeenTriggered` / `Inside`. |

`DoorReaction` (a concrete `ActivationReaction`) lives in [`../Interaction/`](../Interaction/README.md) because it needs `Door`.

## Effects

Effects register **upward**: on enable, each finds its nearest ancestor `ActivationTransition` and registers; on disable it unregisters. The transition never scans for them, so you just drop an effect component on (or under) the transition object.

They are **progress-based**, not tween-based: each owns a 0..1 value and `Apply`s it, and `Play` animates that value toward 0 or 1 (via UniTask). A killed tween leaves half-applied state; a progress value can't — which is what makes cancellation and mid-flight reversal safe.

Built-ins: `MoveEffect`, `ColorEffect`, `TimerEffect`. Add `LightEffect`, `EnableEffect`, `PlayClipEffect`, … the same way.

## Intended setup

```
Button (root)     [InteractableButton] [ActivationSource] [collider]
  Cap (child)     [ActivationTransition sources=Button/ActivationSource]
                  [MoveEffect / ColorEffect / TimerEffect]     <- effects, found upward
DoorReaction      [DoorReaction sources=Button/Cap waitForSettled=true]   (on the door)
```

1. The **button** carries an `ActivationSource`; pressing toggles it.
2. The **cap** (a child) has an `ActivationTransition` whose `sources` list holds the button's `ActivationSource`, plus whatever effects you want. It plays them and settles.
3. A **consequence** — a `DoorReaction` (or your own `ActivationReaction`) — lists the source it watches:
   - the button's `ActivationSource` to fire on the press, or
   - the cap's `ActivationTransition` to fire once the animation has settled.

## Cancellation

The transition cancels the running effect run and starts a new one toward the other target whenever its sources change, and on disable/destroy. Because effects are progress-based, a half-played run reverses from where it is — the door example toggles back and forth cleanly with no stuck state and no double-fire.

## Notes

- **Global namespace** (the assembly's `rootNamespace` is empty).
- **Assembly split is deliberate:** this folder lives in the root `stoogebag` assembly; the interaction family (`InteractableButton`, `Door`, `DoorReaction`) lives in `stoogebag.interaction`, which references this one. Activation does not depend on interaction — so it can serve other triggers (pressure plates, timers, quest flags) without dragging interaction in.
