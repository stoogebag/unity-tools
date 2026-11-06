# Risks and open questions

Ordered by how much they can hurt. §1 is the one to re-check before shipping, not a gate on building.

---

## 1. The plugin ships SMPL-X data — recorded assumption, not a blocker

**What.** The MetaHuman Animator Markerless Motion Capture plugin bundle includes
`SMPLX_NEUTRAL_2020_locked_head_array_f32`, along with `chmr_backbone`, `chmr_head`, `SKM_SMPL`,
`IK_SMPL` and `RTG_SMPL_MH`. The solver is built on SMPL/SMPL-X. `[sourced — UE forum thread,
12 Oct 2026, listing the plugin's files]`

**Why it matters.** The original SMPL-X licence from the Max Planck Institute does not permit
commercial use or redistribution. This is the exact trap that ruled out GVHMR, WHAM, 4D-Humans and
Motion2MixamoRig. Epic's licence does not automatically override MPI's — that depends on whether
Epic struck a deal with MPI.

**Why we are proceeding anyway.** Meshcapade was not merely a user of SMPL/SMPL-X — it was the
company that **sold the commercial SMPL/SMPL-X licences**, and its founders are the inventors of
SMPL. Epic acquired Meshcapade. The licensing position is therefore in-house at Epic. A free,
commercial-licensed plugin on Fab is the rights-holder exercising rights it owns, not a company
gambling on compliance. The decisive fact is the acquisition, not the plugin's licence text.
`[sourced — Meshcapade acquisition, platform shutdown 18 Apr 2026; Meshcapade's own pages describe
them as the inventors of SMPL and the commercial licensing channel]`

**What remains, and why it does not apply here.** The residual question is about **redistributing
SMPL-X data** — the model, the mesh, the `.npz` files. This pipeline never does that. The output is
baked bone animation on our own rig; SMPL-X stays on the capture machine and never ships. That is
not a loophole, it is simply a different artefact.

**Assume:** Epic has cleared the commercial use of the solver and its bundled data.
**Re-check before shipping** if either of these ever changes: we start redistributing SMPL-X data, or
Epic marks the plugin non-commercial or pulls it.

`[assumed — reasonable inference from the acquisition, not a quoted licence term]`

---

## 2. Experimental

Epic's own words: *"Experimental features are not production-ready and may change or be removed in
future releases."* `[sourced]` The plugin is 1.0.0 Beta, Windows-only for body capture. A future UE
release could change the API the harness depends on. Pin the engine version and expect churn.

---

## 3. Ingest frame-rate bug — the one known manual step

Multiple reports of ingest producing an unusable asset:

> "Invalid Frame Rate Error... Capture data frame rate is zero. Image sequence frame rate numerator
> equal zero."
> "SetFootageCaptureData: footage capture data has invalid frame rate; assignment rejected."

Source clip in that report: H.264 MP4, 1080×1920, 30 FPS, constant frame rate. `[sourced]`

**Workaround, from another user in the same thread:** open the Capture Data asset, expand Metadata,
set `FrameRate` manually — it comes in as 0. `[sourced]`

Until patched, this defeats "no human intervention". The harness should either write that field, or
detect frame rate 0 and fail loudly rather than silently produce a bad solve.

---

## 4. GPU sensitivity, and one confounded report

A single forum report, **not a controlled comparison**: `[sourced]`

| Machine | Result |
|---|---|
| UE 5.8.0, RTX 5090, Ryzen, 128 GB | completed successfully |
| UE 5.8.2, RTX 5070, Ryzen 9 9950X3D, 32 GB, Studio driver 616.56 | repeatable GPU/display reset, black screen, at Stage 4 (DirectML) |

**Four variables differ — engine version, GPU, VRAM, RAM.** The reporter's own wording is a
hypothesis: *"I suspect a compatibility issue involving one of the following: MetaHuman Animator
Markerless Motion Capture plugin and UE 5.8.2 / ONNX Runtime/DirectML and the RTX 5070 / A regression
between UE 5.8.0 and 5.8.2."* `[sourced]`

**Do not read this as "5.8.2 is broken".** An earlier version of this file did, recommended pinning
5.8.0, and was wrong twice over: the evidence doesn't support a version cause, and 5.8.0 cannot be
installed anyway.

**What is actually evidenced:** the crash is in `onnxruntime.dll` / `DmlExecutionProvider`, at Stage 4,
not VRAM (6.9 GB used of an 11.2 GB budget, `bIsOOM=0`). That is a DirectML/ONNX path, which is the
same path the AMD driver workaround in §5 addresses.

**Engine versions.** The Launcher only offers the current patch for 5.8 — **5.8.3** as of 22 Sep 2026.
Its only MetaHuman fix is *"applying ML mask weights as true blending weights to RigLogic ML outputs
instead of just treating it as an on/off switch"*, unrelated to the markerless plugin or DirectML.
No one has reported 5.8.3 on this workflow either way. `[sourced]`

**What to do.** Install 5.8.3. If Stage 4 crashes, treat it as the hardware/driver case in §5, not an
engine-version case.


---

## 5. AMD is supported, with one driver caveat

**Corrected.** An earlier draft of this file said NVIDIA was required and that AMD was untested.
That was wrong. Epic's requirements name AMD cards directly and the only platform constraint is
Windows. `[sourced — Epic hardware requirements]`

**The known AMD failure mode** `[sourced — UE forum guide]`:

> "If you are using an AMD Radeon RX 6000 series card (like the RX 6700 XT) and experiencing
> consistent crashes or the 'D3D Device Removed / GPU Crashed' error at Frame 0 when running
> MetaHuman Performance..."

Root cause, per that post: newer AMD Adrenalin drivers conflict with how UE's Neural Network Engine
(`NNERuntimeORTDml`) talks to the hardware over DirectML. The driver panics, freezes the UI, and
Windows force-resets the graphics driver.

**Fix: downgrade to AMD Adrenalin 24.6.1**, clean install. The reporter says that "completely fixes
the AI computation breakdown."

So: if Stage 4 dies with `DXGI_ERROR_DEVICE_REMOVED` on this box, that is the first thing to try.
Note that report is RX 6000-series; this box is RX 9070 XT, one generation newer, so the specific
driver history may not apply identically. `[assumed]`

**CPU.** This box is 8 cores / 16 threads against Epic's "16 physical cores". That figure is listed
for MetaHuman **Creator**, which we do not use — we need the animation, not a MetaHuman character.
Treat as fine unless the solve proves slow. `[assumed]`


---

## 6. Result quality — what users actually report

There is no independent benchmark. The plugin's Fab listing shows 20 ratings at 5.0 with no written
reviews, and its 57-post UE forum thread is overwhelmingly installation failures and crashes, not
quality discussion. What follows is everything substantive found, all from that thread.

**Single-camera depth is the ceiling** `[sourced — Zadrelpro1, 8 Jul 2026]`

> "I would love the ability to create mocap from multiple camera angles. **The single camera is just
> not good enough.** When I tested it, it looks fine from one angle, but as you rotate around, there
> are things that are off because the single camera cannot capture the depth information."

The take is valid from the camera's viewpoint and degrades as you orbit the character. In a game the
player sees it from every angle.

**Pose snapping at chunk boundaries** `[sourced — Ciprian_Stanciu, 1 Aug 2026]`

> "There's this variable in the plugin: `static constexpr int InnerWindow = 192;` that controls
> processing chunks and at their borders there's always pose snapping. Increasing this to the number
> of frames of my animation (like 1000) makes the snapping go away."

**No body smoothing by default** `[sourced — same post]`

> "I also added rolling average smoothing as an animation modifier after the animation export because
> **only the facial expressions get smoothing by default**."

So expect to run a smoothing pass yourself. Raw output jitters.

**Scale limit** `[sourced — same post]`

20,000 frames reaches "~340GB committed memory and then dies due to OOM (I only have 96GB)". He
works in chunks of ~3,000 frames — which reintroduces the window snapping above. Roughly: budget
~1 GB of RAM per 60 frames, and keep clips short.

**A total solve failure** `[sourced — Robert_Adrian, 26 Aug 2026]`

> "I wasted 3 hours on a 30 second video process just for the final result to be a moving head, even
> tho I followed absolutely every step, got full body video with good lighting setup."

**Throughput** `[sourced — ZenPigeonGames, 25 Jun 2026]`

A 3-second clip processes in "~2 minutes" on an RX 6750XT. Extrapolate accordingly.

**The only positive quality remark found** `[sourced — Enter_Reality, 17 Jun 2026]`

"As a first release results are pretty good!" — 13 likes. Note Enter_Reality sells a competing batch
plugin, so treat it as non-impartial.

**Consequence for the plan.** The harness gets you files out of UE. It does not get you shippable
animation. Budget for a smoothing/cleanup pass and for short clips. And do not plan on orbiting
camera work.

---

## 7. Root motion will be unreliable

Not specific to this tool — a property of monocular capture in general:

> "it does not provide accurate root translation, as root translation in world-space is a difficult
> problem to solve for monocular video."
> "The root orientation from HMR 2.0 does not necessarily align with the root orientation of the
> mocap markers. The difference is mostly due to a single yaw-rotational offset... Optimizing for
> root orientation is prone to local minima."

`[sourced — arXiv 2407.06114]`

**Design consequence: capture in-place actions only** (shoot, reload, melee, hit, death, taunt,
gesture). Drive locomotion in-game. Do not plan to capture a walk cycle and use its root motion.

---

## 8. UE → Unity retarget

Unity auto-maps bone names to a Humanoid Avatar and you correct the mapping once; after that clips
retarget automatically. `[assumed — standard Unity behaviour, not tested with a MetaHuman skeleton]`

Open detail: the MetaHuman skeleton has no IK bones, which is what caused thumb/finger and
sitting-pose artefacts in **Unreal's own** retargeter. `[sourced]` Unity uses its own retargeter, so
this may be irrelevant — but it is the thing to check first in Phase 4.

---

## 9. Solve time

One forum user, describing the body path: *"Click Processing and wait until all 5 passes are
complete. it might be very long."* `[sourced]` Budget for it; do not plan a tightly-timed batch.

---

## Questions to send Epic (optional, for the file)

Nothing is blocked on this. Worth sending anyway so there is a written answer on record rather than
an inference, and so the position is documented if the game ever ships.

Subject: *MetaHuman Animator Markerless Motion Capture — SMPL-X licence on bundled data*

> The MetaHuman Animator Markerless Motion Capture plugin ships SMPL/SMPL-X data in its bundle,
> including `SMPLX_NEUTRAL_2020_locked_head_array_f32`.
>
> 1. Does the Fab Standard License and the MetaHuman licence cover commercial use of the animation
>    that the plugin produces, given the underlying SMPL-X terms from the Max Planck Institute?
> 2. Does shipping a game containing only the **baked bone animation** (no SMPL-X mesh, no SMPL-X
>    data redistributed) require a separate MPI/Meshcapade commercial licence?
> 3. Meshcapade's own platforms shut down on 18 April 2026 following the Epic acquisition. Who is
>    the correct contact for SMPL-X commercial licensing today?
>
> Context: a small commercial game project, Unreal Engine 5.8 on Windows. We want to confirm the
> commercial position before building a pipeline around the plugin.

Send to Epic's MetaHuman developer support and to Meshcapade/MPI Innovation. Keep the answer in this
folder.
