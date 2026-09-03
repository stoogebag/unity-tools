# ManagedUpdate System — Design Notes

> Superseded the original source-generator design. This documents the **current** architecture.

## Goal

Replace Unity's per-object `MonoBehaviour.Update()` callbacks with a centralized, manager-driven
loop that crosses the native → managed boundary **once per frame** instead of once per object.

## Current architecture (no codegen)

- **Interfaces** (`IUpdateManaged`, `IFixedUpdateManaged`, `ILateUpdateManaged`) define the contract.
- **`[RequireComponent(typeof(ManagedUpdateLifecycle))]`** enforces the bridge component.
- **`ManagedUpdateLifecycle`** (tiny `MonoBehaviour` on each managed GameObject) registers/unregisters
  the managed components with the driver on enable/disable/destroy. It does **no per-frame work**.
- **`ManagedUpdateDriver`** (single `MonoBehaviour` per scene, the sole frame hook) holds
  `Dictionary<Type, ManagerBase>`. On first sight of a component type it lazily creates
  `new ManagedUpdateManager<T>()` via `Activator.CreateInstance(typeof(ManagedUpdateManager<>).MakeGenericType(type))`.
- **`ManagedUpdateManager<T> : ManagerBase`** holds a homogeneous `List<T>` and invokes the update
  method via a **direct-call delegate** built once with `Delegate.CreateDelegate` (falls back to
  interface dispatch if that fails). No interface dispatch in the hot loop.
- **`Editor/ManagedUpdateBuildValidator`** scans for missing `[RequireComponent]` at player-build time.

## Why codegen was dropped

The original design emitted per-type managers + a registration table from a Roslyn source generator
shipped as an ILRepacked analyzer DLL. Unity failed to load the analyzer
(`CS8033: ... does not contain any analyzers`) because ILRepack internalized `Microsoft.CodeAnalysis`,
so the `[Generator]` attribute / `ISourceGenerator` were not recognized by Unity's Roslyn host.
Generation never ran, so `ManagedUpdateTypeMap.CreateManager` threw at runtime for every managed type.

The lazy-generic approach reproduces the same end result (per-type manager + direct call) with **no
analyzer, no ILRepack, no generated `.cs`, and no stubs to write**.

## File layout

```
Assets/stoogebag/ManagedUpdate/
  stoogebag.ManagedUpdate.Runtime.asmdef
  IUpdateManaged.cs
  IFixedUpdateManaged.cs
  ILateUpdateManaged.cs
  ManagerBase.cs
  ManagedUpdateDriver.cs
  ManagedUpdateLifecycle.cs
  ManagedUpdateManager.cs
  Editor/
    ManagedUpdateBuildValidator.cs
```

(`ManagedUpdateTypeMap`, the `ManagedUpdate.SourceGenerator~` folder, and `Analyzers/` were removed.)

## Component usage

```csharp
[RequireComponent(typeof(ManagedUpdateLifecycle))]
public class Projectile : MonoBehaviour, IFixedUpdateManaged
{
    public void ManagedFixedUpdate() { transform.position += velocity * Time.fixedDeltaTime; }
}
```

## Known limitations / future work

- Execution ordering between manager types is not supported.
- Profiler samples are grouped under `ManagedUpdateDriver`.
- Optional `Profiler.BeginSample` per manager for visibility.
- For massive counts, consider Burst/Jobs (`NativeArray` / `IJob`) in the manager tick.
