# Houdini Pipeline

Greybox a piece of level in Unity, send it to Houdini, cook it, and bring the
result back as a sibling GameObject tree with production prefabs swapped in.

This is an editor-only module inside the `stoogebag` package. It is portable:
nothing here is specific to the host project except the `.hip` you point it at.

## Requirements

- Unity 2022.3 or newer.
- The Unity USD package `com.unity.formats.usd` (Editor assemblies). The editor
  module only compiles when this package is present.
- Houdini installed, providing `hython` (`hython.exe` on Windows).
- `UniTask` and `stoogebag` (already present in this repository).
- No Odin dependency.

## What is in here

| Path | What it is |
|---|---|
| `HoudiniTag.cs` | Component marks a greybox object/type: `tags` + the `productionPrefab` to swap in. |
| `ProcessedAsset.cs` | Marker added to the imported tree; points back at the source object. |
| `Editor/USDModelImporter.cs` | The pipeline: export, run Houdini, import, prefab swap. |
| `Editor/HoudiniPipelineEditor.cs` | Inspector with path pickers and action buttons. |
| `Python~/process_pipeline.py` | Runs inside `hython`; loads the hip and cooks the export node. |
| `Documentation~/how-to-use.md` | Short how-to plus the gotcha list. |
| `Documentation~/houdini-pipeline.md` | Detailed reference: contract, internals, failure modes. |

## Quick start

1. Add a `USDModelImporter` component to the greybox root you want to process.
2. Point it at your `.hip` and the node that holds `unity_control`.
3. Put `HoudiniTag` components on the greybox objects (and/or their types), set
   `tags` and the matching `productionPrefab`.
4. Press **Export + Process** in the inspector.

Artifacts land under `Assets/HoudiniPipeline/` by default; every path is
configurable per component, so different objects can use different hips and
output folders.

Start with [`Documentation~/how-to-use.md`](Documentation~/how-to-use.md).
