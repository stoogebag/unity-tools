# Unity UI Architecture — Window System

## Overview

The stoogebag toolkit uses the **old Unity UI** (Canvas/CanvasGroup, not Toolkit) with a custom **Window system** for modular, animation-driven UI screens. Windows can be activated/deactivated with composable animations.

---

## Window Class

**Location**: `Assets/stoogebag/UITools/Windows/Window.cs`

### State Management
- `ActiveState` enum: `Active`, `Inactive`, `Activating`, `Deactivating`

### Core Methods
- `async UniTask Activate()` — Shows window with animations
- `async UniTask Deactivate()` — Hides window with animations
- `async UniTask Toggle()` — Toggles state
- `void DeactivateImmediate()` — Instant hide
- `static UniTask CloseAllWindows(GameObject parent)` — Close all in hierarchy

### Events
- `static IObservable<Window> OnActivatedObservable`
- `static IObservable<Window> OnDeactivatedObservable`

### Configuration
- `isModal` — Semi-transparent blocker behind window
- `closeOnClickOutside` — Click blocker to close
- `firstSelectedOnActivate` — Auto-select UI element
- `rememberSelectedOnReactivate` — Remember last selected

---

## Animation System

**Interface**: `IWindowAnimation`

Animations run in parallel during Activate/Deactivate via `UniTask.WhenAll()`.

### Available Animations

| Animation | Purpose |
|-----------|---------|
| **CanvasGroupFade** | Fade CanvasGroup alpha 0→1 |
| **WindowSlideIn** | Slide from off-screen to final position |
| **WindowScaleInUI** | Scale from 0 to original |
| **PanelFadeIn** | Fade Image component color |
| **ActivateChildren** | Run child Windows in parallel |

---

## WindowManager

**Location**: `Assets/stoogebag/UITools/Windows/WindowManager.cs`

Global singleton registry for named Windows.

### Methods
- `static void Register(Window w)` — Register window
- `static async UniTask Open(string name, bool exclusive)` — Open by name
- `static async UniTask Close(string name)` — Close by name
- `static Window GetWindow(string name)` — Get reference
- `static async UniTask CloseAll()` — Close all registered windows

---

## Helper Classes

### WindowWithClose
- Auto-finds and binds "Close" button
- Button deactivates window on click

### YesNoWindow
- Confirmation dialog template
- Supports validation callbacks

### TemporaryWindow<TInputModel, TDataModel>
- Modal dialog returning data
- `PopupAndAwaitResult()` — Open and await user choice
- Returns `WindowResult` with status and data

### UIPanelsList
- Multi-step wizard interface
- Manages Next/Previous between sub-windows

---

## Helper Utilities

### Window Control
- **OpenWindowOnClick** — Open on button click
- **OpenWindowOnButton** — Open on input action

### Extension Methods (stoogebag.Extensions)
- `FirstOrDefault<T>()` — Find child by name/condition
- `GetComponentInAncestor<T>()` — Search up hierarchy
- `GetComponentInDescendants<T>()` — Search down hierarchy

---

## Dependencies

- **UniTask** — Async/await (`Cysharp.Threading.Tasks`)
- **UniRx** — Observables (`UniRx`)
- **DOTween** — Animation tweening
- **Odin Inspector** — Editor tools (optional)
- **Unity UI** — Canvas, CanvasGroup, Button, etc.

All have conditional compilation guards.

---

## Usage Examples

```csharp
// Simple activation/deactivation
await window.Activate();
await window.Deactivate();
await window.Toggle();

// Instant hide
window.DeactivateImmediate();

// Via WindowManager
await WindowManager.Open("SettingsWindow", exclusive: true);
await WindowManager.Close("SettingsWindow");

// Close all in hierarchy
await Window.CloseAllWindows(parentGameObject);

// Dialog pattern
var result = await yesNoWindow.PopupAndAwaitResult(inputs, data);
if (result.Result == Result.Proceed) { /* ... */ }
```

---

## Architecture Patterns

1. **Async/Await** — UniTask for clean sequential code
2. **Observable** — UniRx events on window state changes
3. **Component** — Modular animation components
4. **Singleton** — WindowManager and UI managers
5. **Separation of Concerns** — UI logic, animations, input handling separate
6. **Modal Blocker** — Invisible raycast-catching overlay for modality

---

## Notes

- Modal blocker uses fixed sorting order (1000) — potential conflict risk
- WindowManager requires manual window registration
- Animation timings per-animation (no global speed control)
- No built-in UI state persistence between scenes
