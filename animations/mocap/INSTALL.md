# Install

Only the parts that are non-obvious, or that fail.

---

## Engine install options — MetaHuman Creator Core Data is required

In the Launcher, on the 5.8 tile: **Launch → Options → tick MetaHuman Creator Core Data → Apply**,
then install. ~5.5 GB.

It is **not** optional. Without it the Fab plugin fails to install, and the symptom is that the
install appears to do nothing. From the solved thread on that exact failure:

> "You need to have MetaHuman Creator Core Data installed"
> "Installing MetaHuman Creator Core Data fixes the Markerless Motion Capture Plugin installation issue."

If you already installed the engine, reopen **Options** on the 5.8 tile and add it — no reinstall
needed.

## Engine version

Install **5.8.3** (22 Sep 2026). The Launcher only offers the current patch for 5.8, so in practice
that is the only choice.

An earlier version of this file said 5.8.0. That came from one report where a clip completed on
5.8.0 + RTX 5090 and crashed on 5.8.2 + RTX 5070. It was **not actionable** — older patches aren't
selectable in the Launcher — and 5.8.3 doesn't change the picture either way: its only MetaHuman fix
is "applying ML mask weights as true blending weights to RigLogic ML outputs", nothing about the
markerless plugin or DirectML.

So: install 5.8.3 and see. If Stage 4 crashes, the AMD fallback below is the lever.

## Fab plugin

Listing: `fab.com/listings/4095b8e0-3eff-44f1-acb4-cb40b99228b9` → **Add to My Library** → Launcher →
Library → **Fab Library** → **Install to Engine → 5.8.3**.

The install fails in specific, repeatable ways:

| Symptom | Fix |
|---|---|
| `Http-403` / `IS-EO-Lnc-BldInfo:Http-403` | Delete `%localappdata%\EpicGamesLauncher\Saved\webcache*`, sign out, sign back in |
| `MD-0011-0` | Update the Launcher, retry |
| Appears to do nothing | **MetaHuman Creator Core Data missing** — see above. If that's already installed, try in-editor Fab: `Window > Fab` |
| Blocked outright | Unsigned `UnrealEditor-Segmentation.dll` — Smart App Control (already off here) or third-party AV |

The in-editor route installs **per-project**, not globally. It lands at
`<Project>\Plugins\Marketplace\MetaHumanBodyTracker`; copy that folder into
`<UE install>\Engine\Plugins\MetaHuman\` to make it available everywhere.

## Don't install

- **MetaHuman Animator Depth Processing** — iPhone depth / stereo HMC only. Body-from-mono doesn't use it.

## Do set up

- **An identity stand-in MetaHuman.** Not a finished character, but you need one as the export target:
  the export step asks you to "select the skeletal mesh of your assembled MetaHuman", and Epic's own
  guidance for characters still in production is to "solve against an identity stand-in MetaHuman
  first". This is why Core Data is required.

## Plugins to enable

Ships with the engine: **MetaHuman Creator** (to make the stand-in), **MetaHuman Animator**,
**Live Link Hub**, **Capture Manager Editor**, **Python Editor Script Plugin**, **Editor Scripting
Utilities**. Plus the Fab plugin above.

## AMD fallback

Only if the solve black-screens with `DXGI_ERROR_DEVICE_REMOVED` in `onnxruntime.dll` /
`DmlExecutionProvider`.

1. Clean-install **Adrenalin 24.6.1**.
2. `Change device installation settings` → **No**, so Windows Update doesn't put it back.
3. If the editor then dies on a ray-tracing driver assertion: `r.RayTracing=False` in
   `Config/DefaultEngine.ini`.

Users who did this report the DirectML breakdown stops entirely. Reported on RX 6000-series; the
9070 XT is untested.

## Verify

1. All six plugins report enabled.
2. A MetaHuman Performance asset shows a **Body Tracking** checkbox that ticks without complaining
   about a missing plugin.
3. One sub-192-frame clip exports an Animation Sequence that visibly matches the performance.

## Where the UE project lives

Not under `Assets/` — that's the Unity project. Outside the repo, or inside and gitignored.
