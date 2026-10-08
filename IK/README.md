# IK

Bolt-on inverse kinematics for an existing avatar. `AvatarIkRig` lives on its own GameObject (a child or sibling of the avatar) and binds [FinalIK](https://www.root-motion.com/finalikdox/) to the avatar **from afar** — the avatar prefab itself carries no IK components and no knowledge of this system.

See `IK.unity` in this folder for a working, self-contained example.

## Requirements

- **FinalIK** (`RootMotion`) present in the project.
- The `FINALIK` scripting define set (Project Settings → Player → Scripting Define Symbols).
- `stoogebag` core (for `stoogebag.Extensions`).

The `stoogebag.ik` assembly is gated behind `FINALIK`: projects without FinalIK simply don't compile this folder, and nothing else in stoogebag depends on it.

## Quick start

1. Add an empty GameObject under (or beside) the avatar and name it `ik targets`.
2. Add the **`AvatarIkRig`** component to it.
3. Leave every field empty and press Play.

That's it. On `Awake` the rig finds the avatar, creates its solvers, and wires up the target transforms.

### What you get

```
Avatar (Animator, humanoid or mixamo-named)
└── ik targets (AvatarIkRig)      ← the bolt-on
    ├── body
    ├── leftHand
    ├── rightHand
    ├── leftFoot
    ├── rightFoot
    └── look
```

The target children are created automatically at each bone's rest pose, so binding is a no-op until you move a target or raise a weight. You can also supply your own targets instead of letting the rig create them.

## How binding works

`Bind()` runs automatically on `Awake` (toggle with `bindOnAwake`). It:

1. Resolves the avatar `Animator` — the serialized `avatar` if set, otherwise the nearest `Animator` found on a parent, then children, then a sibling under the parent, then anywhere under the scene root.
2. Finds or creates `FullBodyBipedIK` and `LookAtIK`. An existing solver anywhere on the avatar is **adopted** (not duplicated); otherwise one is added to the rig object itself.
3. Auto-detects `BipedReferences` from the avatar root (`BipedReferences.AutoDetectReferences`), which works for humanoid avatars via `Animator.GetBoneTransform` and for generic/mixamo rigs by bone naming.
4. Binds the five body effectors and the look chain (`spine` / `head` / `eyes`), using the avatar root as the look-at root so the head axis is correct.

Because the solvers are adopted-or-created on the rig, the avatar can be completely untouched. A rig placed on an avatar that already has FinalIK solvers (e.g. a legacy prefab) will reuse them.

## Using the rig

### Point the eyes somewhere

```csharp
var rig = GetComponent<AvatarIkRig>();

rig.LookTarget = playerHead;   // a Transform to face
rig.LookWeight  = 1f;          // 0 = off, 1 = full
```

`LookWeight` maps to the look solver's master weight. Fade it for smooth engagement:

```csharp
float t = 1f - Mathf.Exp(-sharpness * Time.deltaTime); // frame-rate independent
rig.LookWeight = Mathf.Lerp(rig.LookWeight, targetWeight, t);
```

Set `rig.LookTarget = null` to stop tracking (the head returns to the animation pose as the weight drops).

### Move a hand or foot

Hands, feet and the body are plain FinalIK effectors, reached through `FullBody`:

```csharp
var solver = rig.FullBody.solver;

solver.rightHandEffector.target = gripPoint;
solver.rightHandEffector.positionWeight = 1f;   // weight defaults to 0 — set it to engage

solver.leftFootEffector.target = footPlacement;
solver.leftFootEffector.positionWeight = 1f;
```

Effector `positionWeight`/`rotationWeight` default to `0`, so assigning a target alone does nothing until you raise the weight. Lower it to blend back out.

### Temporarily disable IK

```csharp
rig.LookAt.enabled = false;   // stop solving the look chain
rig.FullBody.enabled = false; // stop the whole body solve
```

## Component reference

### Serialized fields

| Field | Purpose |
|---|---|
| `avatar` | Avatar `Animator`. Leave empty to auto-resolve. |
| `targetContainer` | Parent for auto-created targets. Defaults to the rig's own transform. |
| `lookTarget`, `bodyTarget`, `leftHandTarget`, `rightHandTarget`, `leftFootTarget`, `rightFootTarget` | Optional explicit target transforms. Empty = auto-create. |
| `bindOnAwake` | Bind automatically in `Awake` (default on). Turn off to control timing yourself. |

### Properties

| Member | Purpose |
|---|---|
| `Avatar` | The resolved `Animator`. |
| `FullBody` | The `FullBodyBipedIK` in use. |
| `LookAt` | The `LookAtIK` in use. |
| `LookTarget` | Get/set the look target `Transform` (setting also updates the solver). |
| `LookWeight` | Get/set the look master weight (clamped 0–1). |

### Methods

| Member | Purpose |
|---|---|
| `Bind()` | Resolve the avatar and (re)wire all solvers and targets. Runs on `Awake`; also available from the component's context menu. |

## Authoring tips

- **Do not put `AvatarIkRig` on the avatar root.** Keep it on its own object so the avatar stays free of IK components.
- **Re-bind after structural changes.** If you swap the avatar, add bones, or change targets, call `Bind()` again (or use the context-menu **Bind**).
- **`bindOnAwake` off** if something else must run first (e.g. an animation system that changes the skeleton during `Start`); call `Bind()` once the skeleton is ready.
- Targets created by the rig sit at bone rest pose, so they're safe to leave in place — only move them when you want IK to act.

## Troubleshooting

- **Nothing happens on Play.** Check the Console for `AvatarIkRig: no Animator/Avatar found…` or `failed to auto-detect a biped…`. The avatar must have a humanoid `Avatar` or recognisable bone names (`Hips`, `Spine`, `Head`, `Left/Right Hand`, `Foot`, …).
- **Limb snaps to a strange pose.** An effector has a non-zero weight with a target far from the bone. Zero the weight or move the target.
- **`AvatarIkRig` is missing from the Add Component menu.** The `FINALIK` define isn't set, so `stoogebag.ik` didn't compile.

## Implementation notes

`AvatarIkRig` is a thin binder: it owns no per-frame logic of its own. FinalIK's components (`FullBodyBipedIK`, `LookAtIK`) do the solving in their own `LateUpdate`; the rig only resolves references and connects effectors to targets once. The look chain is initiated with `IKSolverLookAt.SetChain(spine, head, eyes, avatarRoot)` and the body with `FullBodyBipedIK.SetReferences(references, rootNode)`, so both solve correctly even though the rig object is not the avatar root.
