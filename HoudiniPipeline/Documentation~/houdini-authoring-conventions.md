# Houdini authoring conventions

General authoring conventions for Houdini work that ships to Unity as FBX. These are project-agnostic — the same rules apply to any scene that feeds the pipeline. (The Unity↔Houdini USD round-trip is a separate thing: see `houdini-pipeline.md`.)

## The one rule: every mesh carries a `path`

Unity receives **one FBX with many named meshes**, not many files. The split is driven by a primitive attribute called `path`. Every part, garment or prop is built as its own branch, and the branches are combined with `merge_with_path` (never a plain `Merge`) so each piece is stamped with the name it will have in Unity.

```
branch (named for the part) ─┐
branch ──────────────────────┼─► merge_with_path ─► … ─► rop_fbx (buildfrompath=1, pathattrib=path)
branch ──────────────────────┘
```

## `merge_with_path` — `brian::dev::merge_with_path::1.0`

- **No parameters.** Any number of inputs.
- Output is the union of its inputs, plus per-primitive attributes:
  - `path` — the Unity mesh path.
  - `name` — a surface-name attribute (observed value `surface`).
- **How `path` is derived, per input:**
  - input with no `path` → `path` = that input node's **name**;
  - input that already has a `path` → the input's name is **prepended**: a `hair` path under a `head1` node becomes `head1/hair`.
  - So paths nest to mirror the node hierarchy.

Worked examples (from a character build):

| Node | Inputs | Resulting `path` values |
|---|---|---|
| `…/robe` | shirt, pants1, jacket | `shirt`, `pants1`, `jacket` |
| `…/body/merge_with_path1` | armR, armL, mirror1, legR, head1, torso, mirror2, legL | `armL`, `armR`, `legL`, `legR`, `head1`, `head1/hair`, `mirror1`, `mirror2` |

**Consequence: name your nodes meaningfully.** The node name *is* the Unity mesh name. A `null` SOP named for the part at the end of a branch is the usual terminator.

## FBX export

- **Static / props:** `rop_fbx` and `filmboxfbx` with `buildfrompath = 1` and `pathattrib = path`. This is what splits the geometry by the `path` attribute. Also set `mkpath = 1`.
- **Skinned characters:** `kinefx::rop_fbxcharacteroutput` — set `outputfilepath`, `clipname`, and `restposeattrib = rest_transform`.
- `brian::dev::output_fbx_by_node_name::1.0` is a convenience wrapper.

## Path helpers — `brian::dev::*`

When a path can't come from the node name, use one of these instead of hand-writing the attribute:

| Node | Params | Purpose |
|---|---|---|
| `pathFromNodeName` | `path` | Set `path` from the node's name. |
| `pathFromNodeNamePOINTS` | | Per-point variant. |
| `pathFromProperty` | `path`, `frompr0` ("Attribute From/To") | Derive `path` from another attribute. |
| `pathFromString` | `path` | Set `path` from a literal string. |
| `path_from_point_num` | | Path from a point number. |
| `prepend_path_from_second_input` | | Prefix paths using a second input. |
| `copy_with_centroid_transform_and_path` | | Copy with a centroid transform, carrying the path. |
| `add_to_path_parm` | | Append to an existing path. |
| `name_from_parent_node` | | Name from the parent node. |
| `merge_bounding_boxes` | | Merge bounding boxes (used by level tagging). |

## Unity tags

Levels are split by tag rather than by node name:

- `set_houdini_tag` (`brian::dev::set_houdini_tag::1.0`) — param `path`; tags geometry.
- `get_by_unity_tag` (`brian::dev::get_by_unity_tag::1.0`) — param `tag`; selects geometry carrying a tag.

Typical flow: tag pieces in the level, then gather them with `get_by_unity_tag` for export/placement. (The `HoudiniTag` component on the Unity side and the USD pipeline are documented in `houdini-pipeline.md`.)

## Where the HDAs live

The custom node library (`brian::dev::*`, `brian::*`) is authored in the Syncthing Houdini otls folder (`…/Houdini/otls`). Scenes load these definitions, so an agent editing a scene works against them directly.

## Build conventions

- One node = one clear operation, named for its purpose (the name becomes the Unity mesh name).
- Terminate each branch with a `null` named for the part; combine branches **only** with `merge_with_path`.
- Keep everything procedural and idempotent — re-running a build must not duplicate nodes.
- Units are **meters**, **Y up** — keep props at game scale.
- Give each generator a single **control node** (a `null`/subnet with spare parms) that drives the whole network, so variations come from dialling parameters, not rewiring.

## Primitive gotchas (Houdini 22)

- The cylinder primitive is the **`tube`** SOP (there is no `cylinder` SOP).
- Tube radius is a **2-value tuple** `rad1`/`rad2` (radius at each end); `cap` is a **toggle**; output is a single primitive until `type = poly`.
- `merge` uses `setInput(i, node)` — there is no `setInputs`.
