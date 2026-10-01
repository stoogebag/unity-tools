# PurrNet (stoogebag)

Shared PurrNet setup. A drop-in network bootstrap any project with PurrNet installed can use.

## Contents

- `PurrNetBootstrap.prefab` — the generic network spine, nothing project-specific.
- `stoogebag.networking.purrnet.asmdef` — home for shared PurrNet components.

### PurrNetBootstrap

```
PurrNetBootstrap
  PurrNet   [UDPTransport, NetworkManager]
```

| Piece | Value |
|---|---|
| `UDPTransport` | port 5000, max 100 connections, native sockets, address `127.0.0.1` |
| `NetworkManager` | tick 20, `Unsafe` network rules, `AlwaysVisible` visibility, no prefab registry, no authenticator |
| Start flags | `0` — nothing auto-starts |

It carries no player, no spawner and no prefab registry on purpose. Those are game-specific and stay in the project.

## Package gating

The asmdef is gated on the `PURRNET` symbol:

```json
"defineConstraints": [ "PURRNET" ],
"versionDefines": [
    { "name": "dev.purrnet.purrnet", "expression": "", "define": "PURRNET" }
]
```

`versionDefines` defines `PURRNET` whenever `dev.purrnet.purrnet` is present (empty expression = any version); `defineConstraints` excludes the assembly when it is not. A project without PurrNet skips this assembly instead of failing on the missing `PurrNet.Runtime` reference.

Two things to know:

- Prefabs cannot be conditionally compiled. A project without PurrNet still imports `PurrNetBootstrap.prefab` and shows **Missing Script** on the `PurrNet` child. That resolves on its own once PurrNet is installed and the domain reloads — the prefab stores the script GUID, so no re-wiring is needed.
- The constraint only covers code inside this folder. Code in another assembly that references PurrNet types must be wrapped in `#if PURRNET`.

## Minimal setup

1. Drag `PurrNetBootstrap` into the scene.
2. Make a player prefab with a `NetworkIdentity` (and `NetworkTransform` if it moves).
3. Create a `NetworkPrefabs` asset (Create > PurrNet > NetworkPrefabs), add the player prefab to its `prefabs` list, then assign it to the `NetworkManager`'s `_networkPrefabs`.
4. Add a child `Spawner` under `PurrNetBootstrap` with a PurrNet `PlayerSpawner`; set `_playerPrefab` to the player prefab and assign spawn-point transforms to `spawnPoints`.
5. Press Play and start manually — PurrNet toolbar `S`/`C` buttons, or `NetworkManager.StartServer()` / `StartClient()` from code.

Auto-start is off deliberately: MPPM clones cannot be auto-detected by PurrNet, so every instance would otherwise try to host. The project-specific wiring (camera, owner-gated systems, registry) is documented in `Assets/Core/Networking/README.md`.
