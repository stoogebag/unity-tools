# SceneTimeline

A scene-based timeline controller that enables visual editing via Unity's Timeline Editor while storing all data directly in the scene.

## Overview

`SceneTimeline` bridges `TimelineAsset` (for visual editor access) with scene serialization, eliminating the need for separate `.playable` asset files. This component works for both cutscenes and gameplay sequences.

## How It Works

The component auto-creates and manages a `TimelineAsset` that lives entirely within the scene file. When you select the GameObject in the scene, Unity's Timeline Editor window recognizes and allows full visual editing of the timeline data. All data persists in the `.scene` file—no asset files required.

## Setup

1. Create a new GameObject in your scene (e.g., "CUTSCENE A", "ATTACK_SEQUENCE")
2. Add the `SceneTimeline` component to it
3. A `TimelineAsset` is automatically created and assigned
4. A `PlayableDirector` component is automatically added as a dependency
5. Select the GameObject → Timeline Editor window shows and is ready for editing

## Usage

### Basic Playback

```csharp
SceneTimeline timeline = GetComponent<SceneTimeline>();

// Play from the beginning
timeline.Play();

// Pause at current position
timeline.Pause();

// Stop and reset to start
timeline.Stop();
```

### Time Control

```csharp
// Get current playback time
double currentTime = timeline.GetTime();

// Set playback time (for scrubbing)
timeline.SetTime(5.0);

// Get total duration
double duration = timeline.GetDuration();
```

### State Queries

```csharp
// Check if playing
if (timeline.IsPlaying)
{
    Debug.Log("Timeline is playing");
}

// Access underlying components
PlayableDirector director = timeline.Director;
TimelineAsset asset = timeline.TimelineAsset;
```

## Adding Tracks and Clips

In the Timeline Editor window (with the GameObject selected):

1. Right-click in the tracks panel → Create Track (Animation, Audio, Cinemachine, etc.)
2. Drag clips or animations onto the tracks
3. Set track bindings by dragging scene objects onto the binding fields
4. Adjust timing, duration, and clip properties
5. Save the scene (Ctrl+S)

All data automatically persists in the scene file.

## Track Binding

When you add a track to the timeline, you must bind it to a target GameObject:

1. In the Timeline Editor, look at the track binding field
2. Drag a GameObject from the hierarchy onto the binding field
3. The track now animates/controls that GameObject at runtime

Bindings are stored in the `PlayableDirector` component on the same GameObject.

## Limitations

- **Prefab conversion:** If you convert the GameObject to a Prefab, the timeline reference will break (scene-bound objects cannot be referenced by project assets). Keep timelines in scenes or in prefabs that remain in scenes.
- **Nesting:** Use Control tracks to reference other `SceneTimeline` instances in nested sequences.
- **Clone/duplicate:** Duplicating a `SceneTimeline` GameObject (Ctrl+D) now produces a fully independent **deep copy** of the associated `TimelineAsset` — tracks, clips, clip assets and markers are all cloned, and the `PlayableDirector`'s track bindings are remapped to the duplicate's own objects (matching Unity's normal GameObject-duplicate behaviour). Editing one copy no longer affects the other. You can also force a detach from a shared timeline via the **SceneTimeline ▸ Duplicate Timeline** context-menu item.

## Best Practices

- **Naming:** Use descriptive names for your timeline GameObjects ("PlayerDeathCutscene", "BossAttackSequence")
- **Organization:** Group related timeline GameObjects in an empty parent ("CINEMATICS", "GAMEPLAY_SEQUENCES")
- **Bindings:** Always set up track bindings before adding animations—it's harder to fix after
- **Testing:** Always test timelines in Play mode to verify bindings and animation targets are correct

## Implementation Details

`SceneTimeline` uses `ScriptableObject.CreateInstance<TimelineAsset>()` to create the timeline in memory during `OnValidate()`. The `TimelineAsset` is serialized directly into the scene file via Unity's standard serialization, bypassing the asset database entirely.

The component maintains a reference to both the `TimelineAsset` and a `PlayableDirector`, keeping them synchronized for both editor and runtime use.
