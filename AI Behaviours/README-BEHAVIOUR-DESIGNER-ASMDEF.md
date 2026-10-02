# Behaviour Designer — assembly definitions

How to wire Behaviour Designer (Opsive) so that asmdef-based code in this library can use it. Written after doing it in GayRobots2026, for reuse in other projects.

## The problem

Behaviour Designer ships **two precompiled DLLs plus a large amount of loose source, with no asmdefs**:

```
Behavior Designer/
  Runtime/
    BehaviorDesigner.Runtime.dll        <- core API (79 types)
    BehaviorTree.cs                     <- loose source
    ExternalBehaviorTree.cs             <- loose source
    Variables/  (Shared* wrappers)      <- loose source
    Tasks/      (542 .cs)               <- loose source
  Editor/
    BehaviorDesigner.Editor.dll
    BehaviorTreeInspector.cs            <- loose source
    ...
```

The DLL holds only the core: `SharedVariable<T>`, `Task`, `TaskStatus`, `Behavior`, the composites/decorators, attributes, the event API.

Everything else — the concrete `Shared*` wrappers (`SharedFloat`, `SharedGameObjectList`, …), `BehaviorTree`, `ExternalBehaviorTree` and all 542 stock task scripts — is **loose source with no asmdef**. Unity therefore drops it into the predefined `Assembly-CSharp`.

That breaks asmdef code, because **an asmdef can never reference `Assembly-CSharp`**. So a library asmdef (like `stoogebag.AI`) that references the DLL can see `Task`/`TaskStatus` but *cannot* see `SharedGameObjectList`, and fails with:

```
error CS0246: The type or namespace name 'SharedGameObjectList' could not be found
```

Adding `BehaviorDesigner.Runtime` to `references` does nothing — a DLL filename in `references` shows greyed out, because `references` accepts **asmdef names only**.

## The fix: give BD's loose source its own asmdefs

Two asmdefs, one for runtime and one for editor.

### `Runtime/BehaviorDesigner.Runtime.asmdef`

```json
{
    "name": "BehaviorDesigner.RuntimeASMDEF",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "overrideReferences": true,
    "precompiledReferences": [ "BehaviorDesigner.Runtime.dll" ],
    "autoReferenced": true,
    "defineConstraints": [],
    "noEngineReferences": false
}
```

### `Editor/BehaviorDesigner.Editor.asmdef`

```json
{
    "name": "BehaviorDesigner.EditorASMDEF",
    "references": [ "BehaviorDesigner.RuntimeASMDEF" ],
    "includePlatforms": [ "Editor" ],
    "excludePlatforms": [],
    "overrideReferences": true,
    "precompiledReferences": [
        "BehaviorDesigner.Editor.dll",
        "BehaviorDesigner.Runtime.dll"
    ],
    "autoReferenced": true,
    "defineConstraints": [],
    "noEngineReferences": false
}
```

Placement matters: the runtime asmdef goes in `Runtime/` (covers `Variables/`, `Tasks/` and the two `.cs` files automatically). The editor asmdef goes in `Editor/` — Unity treats that folder name as editor-only, so `includePlatforms: ["Editor"]` is belt-and-braces, not the mechanism.

## The four things that actually bite

1. **`overrideReferences` is required.** `precompiledReferences` is ignored without it. Default is `false`.

2. **Don't name an asmdef the same as the DLL it wraps.** Unity refuses:
   > Plugin '…/BehaviorDesigner.Runtime.dll' has the same filename as Assembly Definition File '…/BehaviorDesigner.Runtime.asmdef'. Rename the assemblies to avoid hard to diagnose issues and crashes.

   Hence the `…ASMDEF` suffix on the assembly names. The `.asmdef` *filenames* can stay normal (`BehaviorDesigner.Runtime.asmdef`); it's the `"name"` field that must differ from the DLL's filename.

3. **The editor asmdef needs the runtime DLL *as well as* the runtime asmdef.** Referencing the runtime asmdef does **not** transitively grant access to the DLL's types. If editor code names a type defined in `BehaviorDesigner.Runtime.dll`, list that DLL in the editor asmdef's own `precompiledReferences`. This was the last error to fall and the least obvious:
   ```
   error CS0234: The type or namespace name 'Action' does not exist in the
   namespace 'BehaviorDesigner.Runtime.Tasks'
   ```

4. **Force a domain reload after renaming an asmdef.** `recompile` may report success while the editor still runs the old assembly set. Verify by inspecting which assembly actually defines a type — if the answer is `Assembly-CSharp` for something that should be in your new asmdef, Unity hasn't reloaded. Reimport All, or reopen the project.

## Consuming it

Any asmdef that uses BD types references the runtime asmdef **by name**:

```json
"references": [ "BehaviorDesigner.RuntimeASMDEF" ]
```

The Movement and Tactical packs contain no asmdefs, so they stay in `Assembly-CSharp` — which auto-references asmdefs, so they keep compiling with no changes. If you ever asmdef them, they should reference `BehaviorDesigner.RuntimeASMDEF`.

Code with no asmdef (e.g. a template folder without one) also lands in `Assembly-CSharp` and sees everything.

## Optional: gating behind a define

If the project should compile without BD present, gate the consumer on a define the BD asmdef supplies:

```json
"defineConstraints": [ "BEHAVIOR_DESIGNER" ]
```

and set `BEHAVIOR_DESIGNER` in **Project Settings → Player → Scripting Define Symbols** (per build target). The consuming `.cs` should already be wrapped in `#if BEHAVIOR_DESIGNER` so it disappears cleanly when BD is absent. Note this is the consumer's guard, not a substitute for the asmdefs above — the asmdef still can't resolve loose-source types without them.

## Checklist

- [ ] `Runtime/BehaviorDesigner.Runtime.asmdef` — name ≠ DLL filename, `overrideReferences: true`, DLL in `precompiledReferences`
- [ ] `Editor/BehaviorDesigner.Editor.asmdef` — Editor-only, references runtime asmdef, **both** DLLs in `precompiledReferences`
- [ ] Reimport All / reopen the project to force a domain reload
- [ ] Verify the type-location check: `SharedGameObjectList` should now resolve to `BehaviorDesigner.RuntimeASMDEF`, not `Assembly-CSharp`
- [ ] Point consuming asmdefs at `BehaviorDesigner.RuntimeASMDEF`
- [ ] If other packages (DLLs) are also involved, give them the same treatment — this problem is generic to any DLL-plus-loose-source plugin

## Note

This is a workaround for how BD ships. A BD update may overwrite the `Runtime/` and `Editor/` folders and clobber these asmdefs — re-apply after any update. The same two-file pattern applies to any Asset Store plugin that ships source without asmdefs, which is the actual reusable lesson here.
