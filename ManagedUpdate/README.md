# ManagedUpdate System

A centralized, manager-driven update loop that replaces Unity's per-object `MonoBehaviour.Update()` callbacks.

## What problem does this solve?

Unity crosses from native C++ into managed C# **once per `MonoBehaviour.Update()` per object per frame**. For a few dozen objects this is negligible; for hundreds or thousands of simple objects the boundary-crossing overhead dominates real frame time.

This system collapses that to **one native → managed crossing per frame**. `ManagedUpdateDriver` (a single `MonoBehaviour`) receives the frame callback, then iterates tight managed lists and dispatches updates directly inside managed code.

## How it works

1. A component opts in by implementing `IUpdateManaged` / `IFixedUpdateManaged` / `ILateUpdateManaged` and carrying `[RequireComponent(typeof(ManagedUpdateLifecycle))]`.
2. `ManagedUpdateLifecycle` is a tiny bridge `MonoBehaviour`. On enable it finds every managed-update component on its `GameObject` and registers them with `ManagedUpdateDriver`; on disable/destroy it unregisters.
3. `ManagedUpdateDriver` (one per scene) holds a `Dictionary<Type, ManagerBase>`. The **first time it sees a component type** it lazily manufactures the matching manager — `new ManagedUpdateManager<T>()` closed over the concrete component type. No registration list, no generated code, no stub files to write.
4. Each frame Unity calls `ManagedUpdateDriver.Update / FixedUpdate / LateUpdate` once; it ticks every per-type manager.
5. Each `ManagedUpdateManager<T>` holds a homogeneous `List<T>` and calls the component's update method **directly** — a reflection-built `Action<T>` delegate, built once per type — so there is no per-item interface dispatch in the hot loop.

```
[MyComponent : IFixedUpdateManaged]
            │  [RequireComponent]
            ▼
[ManagedUpdateLifecycle]  ──register/unregister──▶  [ManagedUpdateDriver]
                                                          │ ticks once/frame
                                                          ▼
                           ManagedUpdateManager<MyComponent>   (List<MyComponent>, direct call)
```

## Why this shape?

- **Interfaces, not a base class** — leaves your component's own base class free.
- **`[RequireComponent]`** — guarantees the bridge is present and centralizes registration in one place.
- **`ManagedUpdateLifecycle`** — the only place that talks to the driver; your components just implement the interface and do their work.
- **Lazy per-type managers instead of a source generator** — the original design used a Roslyn source generator to emit one manager class per component type plus a registration table, shipped as an ILRepacked analyzer DLL. Unity failed to load that analyzer (`CS8033: ... does not contain any analyzers`), so generation never ran and nothing was registered. We replaced it: `ManagedUpdateDriver` closes a generic `ManagedUpdateManager<T>` over each concrete type at runtime via `Activator.CreateInstance`. No codegen, no analyzer, no stub maintenance.
- **Direct-call delegates** — `ManagedUpdateManager<T>` builds an `Action<T>` to the concrete update method once (via `Delegate.CreateDelegate`), giving the same direct-call dispatch the generator produced, without generating any source.

## Lifecycle behaviour

| User action | What happens |
|---|---|
| Component added to GameObject | `[RequireComponent]` adds `ManagedUpdateLifecycle` automatically. |
| GameObject / component enabled | `ManagedUpdateLifecycle.OnEnable` registers the components. |
| GameObject / component disabled | `OnDisable` unregisters. |
| GameObject destroyed | `OnDestroy` unregisters defensively. |
| Object destroyed mid-update | Manager iterates backward and null-checks; the dead entry is removed safely. |
| Scene unloaded | Objects are destroyed; `ManagedUpdateLifecycle` unregisters; `ManagedUpdateDriver` is destroyed with the scene. |

## The contract

```csharp
[RequireComponent(typeof(ManagedUpdateLifecycle))]
public class Projectile : MonoBehaviour, IFixedUpdateManaged
{
    public void ManagedFixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }
}
```

Implement only the buckets you need:

- `IUpdateManaged` → driven by `ManagedUpdateDriver.Update`
- `IFixedUpdateManaged` → driven by `ManagedUpdateDriver.FixedUpdate`
- `ILateUpdateManaged` → driven by `ManagedUpdateDriver.LateUpdate`

A component may implement any combination.

## Build-time validation

`Editor/ManagedUpdateBuildValidator` runs only during a player build (`IPreprocessBuildWithReport`). It scans compiled types and fails the build if any managed-update `MonoBehaviour` lacks `[RequireComponent(typeof(ManagedUpdateLifecycle))]`, catching shipping mistakes with zero runtime cost (the validator lives in an `Editor/` folder and is stripped from players).

## Performance characteristics

| Cost | Location |
|---|---|
| Native → managed boundary | Once per frame in `ManagedUpdateDriver`. |
| Per-object dispatch | Inside the manager tick loop; **direct call** on the concrete type via the cached delegate. |
| Null check | One `== null` per entry per frame; negligible. |
| List growth / shrink | Only on register/unregister, not per-frame. |
| Manager creation | Once per concrete type, via `Activator.CreateInstance` + one `Delegate.CreateDelegate`; not per-frame. |

## Limitations

- **Execution order**: managers currently tick in dictionary insertion order. If one component type must update after another, add an explicit ordering mechanism.
- **Profiler visibility**: all managed updates appear under `ManagedUpdateDriver` rather than per-component.
- **Build-only validation**: missing `[RequireComponent]` is not caught when simply pressing Play in the editor.
- **Reflection at manager creation**: creating a manager uses reflection once per type (not per frame); negligible.
