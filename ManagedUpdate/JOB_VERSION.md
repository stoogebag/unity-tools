# Job System + Burst — Platform Notes (JOB_VERSION)

Documents platform limitations for the Burst-based ManagedUpdate job path
(JobManager / IJobParallelForTransform).

## Supported — Burst accelerated, multithreaded
- Standalone: Windows (x64/x86), macOS (x64, Apple Silicon), Linux (x64)
- Mobile: Android (ARMv7/ARM64), iOS (ARM64)
- Consoles: PlayStation 4/5, Xbox One/Series, Nintendo Switch
- IL2CPP builds

## Degraded / unsupported
- WebGL: Burst compiler unavailable. Jobs run as managed C# (no Burst
  speedup) inline on the main thread (no worker threads). API is safe but
  the batching benefit is lost — do not rely on this path for WebGL perf.

## Constraints (all platforms)
- Jobs must be unmanaged/blittable: no managed refs, no heap allocs,
  no virtual dispatch, no try/catch in Execute.
- No Unity.Object access inside jobs (only TransformAccess / NativeArray).
- Float results may differ from managed math (native SIMD) — relevant for
  networking determinism (FishNet).
- Burst is AOT-compiled at build time (fine for iOS/consoles).

## Fallback
Components may keep the main-thread ManagedUpdateManager<T> path when Burst
is unavailable; the driver supports both manager types.
