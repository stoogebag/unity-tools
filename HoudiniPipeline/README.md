# Houdini Pipeline

Greybox a piece of level in Unity, send it to Houdini, cook it, and bring the
result back as a sibling GameObject tree with production prefabs swapped in.

This is an editor module inside the `stoogebag` package: the pipeline logic is
editor-only and stripped from builds, but the config and tagging components are
runtime (a MonoBehaviour must live outside an `Editor` assembly to be attachable
to a GameObject). It is portable: nothing here is specific to the host project
except the `.hip` you point it at.

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
| `USDModelImporter.cs` | Runtime component: the per-object pipeline config (hip, node path, folders). |
| `HoudiniTag.cs` | Component marks a greybox object/type: `tags`, `parameters` + the `productionPrefab` to swap in. |
| `ProcessedAsset.cs` | Marker added to the imported tree; points back at the source object. |
| `Editor/USDModelImporterPipeline.cs` | The pipeline logic — export, run Houdini, import, prefab swap. Editor-only. |
| `Editor/HoudiniPipelineEditor.cs` | Inspector with path pickers and action buttons. |
| `Python~/process_pipeline.py` | Runs inside `hython`; loads the hip and cooks the export node. |
| `Samples/TestHoudiniPipeline.unity` | Sample greybox: tagged enemies and an inherited forest zone. |
| `Documentation~/how-to-use.md` | Short how-to plus the gotcha list. |
| `Documentation~/houdini-pipeline.md` | Detailed reference: contract, internals, failure modes. |
| `Documentation~/houdini-authoring-conventions.md` | General Houdini authoring rules (`merge_with_path`, `path`, FBX export). |

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
