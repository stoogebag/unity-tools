# How to use

## Setup

1. Add the `.hip`/`.hiplc` to the project anywhere.
2. In that hip, make sure the configured parent node contains:
   - a `unity_control` node with string parms `input_usd_path`,
     `output_usd_path`, `control_json`; and
   - a USD export node whose name contains `usdexport` (e.g. `usdexport1`).
3. Add a `USDModelImporter` component to the greybox root.
4. Configure it (all fields are per-object):
   - **Houdini Project** — path to the `.hip`.
   - **Houdini Node Path** — parent node that contains `unity_control`.
   - **Hython** — leave empty to resolve from `PATH`, or point at `hython.exe`.
   - **Process Script** — leave empty to auto-find the shipped
     `process_pipeline.py`, or point at your own.
   - **Pipeline Root / USD Output / USD Export** — where artifacts go
     (default `Assets/HoudiniPipeline/...`).
5. Tag the greybox: add `HoudiniTag` to objects and/or reusable types, set
   `tags` (e.g. `GRASS`) and `productionPrefab`.
6. Press **Export + Process**.

Result: a new `<Object>_PRODUCTION` tree appears next to the original, with
matching prefabs swapped in. Press again to regenerate (the previous tree is
destroyed first).

## Buttons

- **Export USD** — write `input.usda` to the export folder only. No Houdini.
- **Import Last Output** — import the last `<Scene>_<Path>.usd` again.
- **Export + Process** — full round trip.

`Tools > stoogebag > Houdini Pipeline > Export Selected USD` exports the
selected object to its importer's export folder (or the default).

## Gotchas

- **Houdini must be reachable.** Leave **Hython** empty to use `PATH`, or set it
  explicitly. If it can't be found the run throws.
- **The hip is loaded fresh every run.** `hou.hipFile.load` replaces the session,
  so unsaved hip edits are lost. Save the hip first.
- **The cook is capped at 30 seconds.** A longer/hung cook is reported as
  failure (or worse, as nothing). Keep the hip fast, or raise the timeout in
  `USDModelImporter.cs`.
- **A chatty cook can wedge the process.** Output streams are drained
  sequentially, so a huge amount of logged output can deadlock the read. Keep
  hip output modest.
- **No cancellation.** The token is not wired to `hython`; the process runs to
  completion in the editor.
- **Tags are inherited and comma-joined.** Every prim gets its own tags plus all
  ancestors' tags, joined with `,`. Don't put commas in a tag.
- **Direct-tag trick.** On import the importer subtracts the nearest tagged
  ancestor's tags to find a prim's "direct" tags. If an object has no tags of its
  own, nothing is swapped in.
- **`productionPrefab` lives on the greybox `HoudiniTag`, not the prefab.** The
  importer reads the mapping from the *original* object's `HoudiniTag`s.
- **Prefab meshes are matched by hierarchy path/name.** Rename a node, or let
  Houdini merge meshes, and the mesh won't populate. Keep prefab child names
  matching the prim names.
- **The source object should own a `UsdAsset` (usually a child).** If present,
  its import options (scale, handedness, materials) are reused; otherwise
  defaults are used.
- **Root-level objects leak `_PRODUCTION` copies.** Cleanup only searches the
  original's parent; if the original is a scene root, re-runs stack up. Parent
  your greybox under something.
- **Names must be unique per scene.** The pipeline folder/file is keyed on
  `<Scene>_<ObjectPath>`; collisions overwrite.
- **Prefab assets have no scene name.** Running this on a prefab asset (not a
  scene object) produces a malformed stamp.
- **`disableSourceOnImport` deactivates the original** after importing. Handy for
  comparing; uncheck to keep it live.
- **Editor-only.** The module needs the USD package, so it is stripped from
  builds and from projects without USD.
