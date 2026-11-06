# Animations / Mocap

Turning **recorded video** into **skeletal animation** for humanoid characters, as a repeatable
batch process.

Status: **planning only.** Nothing is installed, nothing is built. Every claim below is tagged
`[sourced]` or `[assumed]` — see `RISKS.md` for the open questions.

> Naming: this folder is lowercase per request. Its neighbours (`HumanoidAnimation`,
> `HoudiniPipeline`) are PascalCase. Rename if the inconsistency bothers you.

---

## 1. The route

**Unreal Engine 5.8 + MetaHuman Animator + Epic's free `MetaHuman Animator Markerless Motion
Capture` plugin.**

| | |
|---|---|
| Input | single-camera video, one performer, head-to-toe in frame `[sourced]` |
| Output | full-body animation incl. hands, on the MetaHuman skeleton `[sourced]` |
| Where it runs | entirely local; footage never leaves the machine, no cloud, no credits `[sourced]` |
| Cost | plugin free on Fab; MetaHuman free under $1M revenue `[sourced]` |
| Automation | Python + Blueprint APIs, documented as suitable for headless/farm/scheduler `[sourced]` |

### Why this one

The requirement was: **batch, video in, Mixamo-standard skeleton out, commercially clean, free.**

- Every hosted tool (Rokoko, Plask, DeepMotion, Move, QuickMagic, AIMoCap, Viggle) gates commercial
  use behind a paid plan `[sourced]`. None give free commercial batch.
- The "do the work free, pay one month later to license it" plan does not work: licence terms are
  fixed at generation time, not at upgrade time `[sourced — QuickMagic ToS, Move.ai EULA §7.3]`.
- Every high-quality open-source route (GVHMR, WHAM, 4D-Humans, Motion2MixamoRig) requires
  SMPL/SMPL-X, which is MPI-gated non-commercial `[sourced]`.
- This is the only option that is local, unmetered, batchable, commercial-permitted, and free.

### What it is not

- **Not a Unity tool.** Unreal Editor is a required stage of the pipeline. There is no equivalent
  inside Unity.
- **Not production-ready.** The plugin is flagged Experimental by Epic: "not production-ready and
  may change or be removed in future releases" `[sourced]`.
- **Not licence-clean in one narrow respect** — the plugin bundles SMPL-X data. Recorded as an
  assumption rather than a gate; see `RISKS.md` §1.

---

## 2. Rejected alternatives

| Option | Why not |
|---|---|
| Rokoko free (30 s/month, FBX, commercial) | Cannot batch video. Command API only drives a running Studio, no video endpoint. 30 s/month makes batch moot. `[sourced]` |
| Plask / DeepMotion / Move One / QuickMagic free tiers | Non-commercial on free, at generation time `[sourced]` |
| RADiCAL | Shut down. Autodesk acquired the tech; portal closed 6 July 2026, data not transferred `[sourced]` |
| Marionette | Download server returns 403, zone suspended. Windows + NVIDIA only; free tier caps export at 3 s `[sourced]` |
| MediaPipe / MotionBERT → BVH → MIT retargeter | Free, uncapped, commercially clean — but landmark-only: no bone twist in the data, needs IK solving, weak on feet and depth. Kept as the fallback, not the primary. `[assumed — not tested]` |
| TechAnim Markerless Mocap (Fab) | $31.64–$63.31 for a GUI harness around a free solver we can drive headlessly ourselves. Declined. `[sourced]` |

---

## 3. Hardware reality

**NVIDIA is not required.** Epic's requirement is Windows + a DirectX 12 GPU with 8 GB VRAM. Their
recommended-GPU line names AMD explicitly: "At least nVIDIA RTX 3070, **AMD RX 6800 XT**, or Apple
M2 Ultra, with 8GB VRAM". The plugin's only platform note is "Only Supported on Windows".
`[sourced — Epic hardware requirements]`

**Everything runs on this box.** Unity, the capture stage, and the output all live here — no second
machine, no synced folder, no handoff.

| | Requirement | This machine |
|---|---|---|
| OS | Windows `[sourced]` | Windows 11 Pro, build 26100 ✓ |
| API | DirectX 12, RHI set to DX12 `[sourced]` | DX12 4.09.00.0904 ✓ |
| GPU | DX12, 8 GB VRAM `[sourced]` | AMD RX 9070 XT, ~16 GB ✓ |
| RAM | 32 GB min, 64 GB recommended `[sourced]` | 63.6 GB ✓ |
| CPU | 16 physical cores / 32 GB — listed for MetaHuman **Creator**, not Animator `[sourced]` | Ryzen 7 8700F, 8c/16t ⚠ |
| Engine | Unreal 5.8.3 — the only patch the Launcher offers; see `RISKS.md` §4 | **not installed** |

The CPU line is the only shortfall against Epic's numbers, and it belongs to MetaHuman Creator —
which we don't use, because we don't need a MetaHuman character, only the animation. `[assumed]`

---

## 4. Pipeline

```
  video folder
      │
      │  ① INGEST      Capture Manager  →  Footage Capture Data assets
      │                 (Live Link Hub, or in-editor API)
      ▼
  capture data assets
      │
      │  ② SOLVE       MetaHuman Performance asset, Body Tracking on,
      │                 Markerless plugin solver (5 passes)
      ▼
  internal animation
      │
      │  ③ EXPORT      Animation Sequences  →  optional FBX
      ▼
  .fbx
      │
      │  ④ INTAKE      Import as Humanoid, retarget, land in the owning system folder
      ▼
  .anim clips for Animancer
```

### ① Ingest

Capture Manager takes a **source path to a folder** and searches it plus subfolders automatically;
files go to a queue. Transcodes via FFmpeg into a capture data asset per file.
`[sourced — Epic docs]`

The GUI version is `Tools > Live Link Hub > Capture Manager > Add > Mono Video Ingest`.
`[sourced]`

### ② Solve

On the MetaHuman Performance asset: input type `Monocular Footage`, point it at the capture data
asset, enable **Body Tracking** (that variable is what engages the markerless plugin), optionally
trim the frame range, click Process. Five passes. `[sourced]`

All MetaHumans share a skeleton, so the result maps to any MetaHuman without retargeting.
`[sourced]`

### ③ Export

Export Animation Sequence, or FBX for anything outside UE. `[sourced]`

### ④ Unity intake

Import the FBX, create a Humanoid Avatar (Unity auto-maps bone names; correct the mapping **once**),
then every clip retargets automatically. Output lands as `.anim` clips, which is what the repo
already consumes — see `Boomer/Weapons/Weapon.cs` for the Animancer pattern.

---

## 5. The harness

The point of this folder. Unattended batch, no clicking.

**Shape**

```
mocap_run.ps1  <video-dir>  <out-dir>
```

which shells out to Unreal, and a Python entry point that does the real work.

**Facts this design rests on** `[sourced — Epic "Scripting and Automating MetaHuman Animator"]`:

- The automation surface covers, in order: download takes → **ingest into Footage Capture Data
  assets** → generate calibration → choose calibration → **process a Performance into animation** →
  **export an Animation Sequence or Level Sequence**.
- "Use Python when the run is unattended. Python suits headless and batch invocation from a farm, a
  build step, or a scheduler. In those contexts, a blocking call runs to completion and returns an
  **exit code**."
- Epic's own example: "a Python entry point for the nightly batch".
- Body tracking is just a variable on the Performance asset, and "the process is the same for
  Python". Epic ships **`process_monocular_performance.py`** in the plugin's `Content/Python`
  folder, plus `export_performance.py`.

**Invocation shape** `[sourced]`

```
UnrealEditor-Cmd.exe MyProject.uproject -run=pythonscript -script=".../process_monocular_performance.py -- ..."
```

Editor must be closed on the project — the commandlet locks the `.uproject`. Launcher blocks until
UE exits; **exit code 0 required**. `[sourced]`

An independent public repo (`smorchj/metahuman-to-glb`) runs exactly this shape in a staged pipeline
with a validation gate between stages, so the pattern is proven outside Epic's docs. `[sourced]`

**Non-negotiables for the harness**

- **Hard cap: 192 frames per clip.** The solver processes in windows of
  `static constexpr int InnerWindow = 192` and produces pose snapping at every window border. Under
  192 frames there is no border, so there is no snapping. That is 6.4 s at 30 fps. Aim for ~3 s for
  compute time; treat 192 frames as the correctness limit. Anything longer must be cut before ingest.
  `[sourced — RISKS.md §6]`
- **Cutting is free, stitching is not.** Segments are solved independently, so the seam does not
  match: expect a pose pop plus a root translation/yaw offset. If a performance must exceed 192
  frames, split with ~0.5 s overlap and cross-fade the join — but align root translation and yaw
  first, and slerp the rotations. Blending two unaligned solves slides the character through the
  seam, and blending across differing foot contacts slides the foot. Do not butt-join.
- **No chunking to work around memory.** Chunking above ~3,000 frames reintroduces the same window
  snapping and, per reports, OOMs around 340 GB at 20,000 frames. Keep clips short instead.
- **A smoothing pass is required.** Only facial animation is smoothed by default; body output jitters
  and needs a rolling-average pass after export. This is a step we own. `[sourced — RISKS.md §6]`
- Idempotent: re-running on the same folder must not re-solve finished takes.
- One clip = one output name, derived from the input filename, stable across runs.
- Per-clip failure must not abort the batch; collect failures and report at the end.
- Exit non-zero if any clip failed.
- No GUI interaction anywhere in the happy path.
- Writes only to the output dir and the UE project's capture folder.

---

## 6. Phases

Each phase ends in something checkable. Do not start the next until the previous is confirmed.

### Phase 0 — install the capture stack
On this machine, signed in to Epic: UE **5.8.3** (the only patch the Launcher offers), then Fab →
*MetaHuman Animator Markerless Motion Capture* → Add to My Library → Install to Engine. Then enable
the shipped plugins. GUI only — winget cannot install any of it.
**Check:** UE 5.8.3 launches and all six plugins report enabled.

### Phase 1 — one clip, by hand, GUI
UE 5.8.3 + plugins installed on this machine. One short clip through the GUI: ingest → Process with
Body Tracking → Export Animation Sequence. Export FBX.
**Check:** an FBX that plays back a recognisable version of the source performance.

### Phase 2 — same clip, headless
Reproduce Phase 1 via `UnrealEditor-Cmd.exe -run=pythonscript`, no GUI, exit code 0.
**Check:** byte-comparable output to Phase 1, and `$LASTEXITCODE` is 0.

### Phase 3 — folder batch, headless
`mocap_run.ps1 <video-dir> <out-dir>` over ~5 clips, one of which is deliberately bad.
**Check:** 4 good outputs, 1 reported failure, non-zero exit, no GUI opened, re-run is a no-op.

### Phase 4 — Unity intake
Import an FBX, build the Humanoid Avatar, retarget, land `.anim` clips.
**Check:** clip plays on a stoogebag humanoid in the editor.

### Phase 5 — document and wire
Update this folder; add the intake step where it belongs in the repo.

---

## 7. Verification rules

- **Test the path that ships.** The batch must be verified through `mocap_run.ps1` + `UnrealEditor-Cmd`,
  not through a hand-driven editor session.
- **A green compile is not a result.** Every phase check above is an artefact or an exit code, never
  "it built".
- **State what was exercised.** Do not write "works" for a branch that was not run.
- **One phase at a time.** Stop at the end of each phase and report.

---

## 8. Out of scope

- Face capture. The plugin does it; this plan does not.
- Multi-person capture. Forum reports suggest a workaround for two performers; ignore it.
- Any change to `Assets/Boomer/` (template) — this is a `stoogebag` tool.
- Committing FBX or video binaries before Git LFS is configured.
