# stoogebag TODOs

Cross-cutting cleanup for the library. Keep it short; delete entries when done.

## Camera

- **Remove CM2 (`Cinemachine` assembly) usage.** CM3 (`Unity.Cinemachine`) is the target. The legacy `Cinemachine` assembly is still referenced by `stoogebag.dialogue`, `stoogebag.UI` and `stoogebag.cinemachine`, and used by `PointOfInterest` (`CinemachineVirtualCamera`), `CinemachineAxisMouseDown`, `Cinemachine/Editor/AssetMenus` and the `CinemachineCameraShake*` timeline track. Migrate those to CM3, then drop the `Cinemachine` asmdef references. **Deferred — separate day.**
