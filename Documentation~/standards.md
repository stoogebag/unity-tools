# Unity Platform Standards

These rules apply to all Unity projects.

---

## C# Naming Conventions

### Fields
- **Private fields:** `_camelCase` for backing fields and state (e.g., `_comboState`, `_spawnTime`). Be consistent within a file.
- **Public fields:** `PascalCase` (e.g., `Ready`, `Dead`, `color`).
- **Serialized private fields:** `[SerializeField] private camelCase` — no underscore on serialized fields.

### Methods and Properties
- **Methods:** `PascalCase` (e.g., `RunWave`, `SpawnPlayer`, `PlayScoredAudio`).
- **Properties:** `PascalCase`.

### Classes and Interfaces
- **PascalCase** (e.g., `PlayerHealth`, `IWaveProvider`).

---

## Async Patterns

Use **UniTask** (`Cysharp.Threading.Tasks`) for asynchronous operations:

```csharp
using Cysharp.Threading.Tasks;

private async UniTask Scored(...)
{
    await UniTask.WaitForSeconds(3f);
}
```

---

## Reactive Patterns

Use **UniRx** for observable streams. Prefer observable patterns over polling or event polling:

```csharp
using UniRx;

// Expose C# events as observables
public event Action Died;
public IObservable<Unit> DiedObservable() => Observable.FromEvent(h => Died += h, h => Died -= h);

// BehaviorSubjects for state
private readonly BehaviorSubject<ComboState> _comboState = new BehaviorSubject<ComboState>(null);
public IObservable<ComboState> ComboStateObservable => _comboState.AsObservable();
```

Use reactive patterns and observable streams (`UniRx`) as the default approach for all systems. Only deviate from this if there is a compelling reason to do so (in which case, suggest it to the user).

---

## MonoBehaviour Lifecycle

Use the standard Unity lifecycle methods:
- `Awake()` — for initialization that doesn't depend on other objects
- `Start()` — for initialization that may depend on other objects
- `Update()` / `FixedUpdate()` — for per-frame logic

---

## Inspector Attributes

Projects may use **Odin Inspector** (`Sirenix.OdinInspector`) for custom editor attributes:
```csharp
[Button]
public void CycleColor() { ... }
```

---

## Scene Hierarchy & Encapsulation

Separation of concerns applies to both code architecture and the Unity scene hierarchy.

**Example:** If a GameObject handles character death and spawns particles, the particle spawning logic should live on a separate child GameObject with its own components.

**Another Example:** If a character has dialogue functionality, all dialogue-related code and components should be isolated on a dedicated child GameObject.

This keeps systems modular, testable, and maintainable. Avoid GameObjects with many loosely-related components; instead, break them into focused child objects.

---

## Unity .meta Files

**Take utmost care with `.meta` files at all times.** Every asset and folder in a Unity project has a corresponding `.meta` file that stores GUIDs and import settings. If a `.meta` file is lost or mismatched, all references to that asset break across scenes and prefabs.

- **When moving files:** Always move the `.meta` file alongside the asset. Never move a file without its `.meta` file.
- **When deleting files:** Delete the `.meta` file as well.
- **When creating files:** Unity generates `.meta` files automatically—do not create them manually.
- **Never ignore or discard `.meta` file changes in git.** They are not optional metadata; they are critical to project integrity.

---

## Testing & Verification

See `testing-and-verification.md`.
