# Dialogue

The dialogue module: playing single lines ("barks"), sequences of lines, scripted cutscene dialogue, and branching conversations. Also where the cutscene *timeline* runner's dialogue integration lives.

Status: the model is being unified (Stage 16). Items below are marked **[built]** or **[planned]**. Design detail lives in `Planning/STAGE 16 - Dialogue Model.md`, `Planning/STAGE 16 - Samples and Prefabs.md`, `Planning/STAGE 16 - Timeline Runner and Controls.md`.

## The model

| Thing | Kind | What |
|---|---|---|
| `DialogueLine` | serializable, **[built]** | The unit: `Speaker`, `Text`, `Clip`, `LingerSeconds`. |
| `DialogueSpeaker` | scene component, **[built]** | A character: `Name`, `Portrait`, `AudioSource`. A narrator is one with an empty name. |
| `DialogueSequence` | serializable inline, **[built]** | Ordered `List<DialogueLine>`. **A bark is a 1-line sequence.** Custom drawer still to come. |
| `DialogueSequencePlayer` | component, **[built]** | Plays a `DialogueSequence` through a `SimpleDialogueWindow`. Implements `IAdvanceable`; pushes itself onto `PlaybackControls`. Single-stage advance (see TODOS). |
| `SceneTimeline` | component (`stoogebag.Timeline`), **[built]** | Plays one Timeline. Implements `IAdvanceable`. Holds on `IHoldPoint` clips; owns 2x and focus. |
| `DialogueTimelineClip` / `DialogueTimelineBehaviour` | Timeline clip, **[built]** | A line inside a Timeline. `DialogueTimelineClip : IHoldPoint` (`WantsHold = PauseTimeline`). |
| `PlaybackControls` | scene singleton (root `stoogebag`), **[built]** | Owns advance/skip input: two serialized `InputActionReference`s read in `Update`. Holds a **stack** of `IAdvanceable`; forwards to the top. Enable/disable the actions with the component. |
| `IAdvanceable` / `IHoldPoint` | interfaces (root `stoogebag`), **[built]** | Driver contract (`Advance`/`SkipAll`) / timeline-clip marker (`WantsHold`/`OnAdvance`). |
| VIDE | third-party, **[built]** | Branching conversations only. |

Presence is the toggle: empty `Text` = no subtitle, null `Clip` = silent, null `Portrait` = none.

## Presentation templates

| Template | Status | Notes |
|---|---|---|
| `SimpleDialogueWindow` | **[built]** | `Window` subclass: Febucci typewriter on TMP + optional label + optional portrait + VO `AudioSource` + continue indicator. Speaker-driven — reads `DialogueLine.Speaker` for name/portrait; `followSpeaker` tracks a world-space speaker. Base prefab: `Prefabs/SimpleDialogueWindow.prefab`. |
| `BarkWindow` | **[built]** | `Prefabs/BarkWindow.prefab` — world-space bubble (`UIFollow` on), speaker label + optional portrait, **no** continue indicator (barks are transient). |
| `NarrationWindow` | **[built]** | `Prefabs/NarrationWindow.prefab` — screen-space, bottom-centre, no portrait, **no** continue indicator. Narrator label. |
| `ExaminableWindow` | **[built]** | `Prefabs/ExaminableWindow.prefab` — screen-space, centred, **keeps** the continue indicator (you dismiss it). |
| `SubtitleWindow` | **[built]** | `Prefabs/SubtitleWindow.prefab` — screen-space, bottom-centre cutscene subtitle. **Keeps** the continue indicator (the cutscene holds per line and the player advances it). |
| Legacy per-NPC panel | **[deprecated]** | `DialoguePanel` on `Prefabs/Dialogue.prefab` — one panel per speaker, matched by name. Superseded by `SimpleDialogueWindow`; retire when the timeline path is migrated. |
| `DialogueUIContainer` | **[built]** | VIDE's full conversation UI (`Prefabs/DialogueUIContainer.prefab`): portraits, text, choices. |

## Drivers

- **`SceneTimeline`** — **[built]** — a timeline; dialogue lines ride on a `DialogueTimelineTrack`.
- **`DialogueSequencePlayer`** — **[built]** — a plain sequence, no timeline. Barks/narration/examinable.
- **VIDE** — **[built]** — branching conversations, via `DialogueTrigger` + `DialogueUIContainer`.

## Key files in this folder

| File | Role |
|---|---|
| `DialogueLine.cs` | The line data: `Speaker`, `Text`, `Clip` (`[Recordable]`), `LingerSeconds`. |
| `DialogueSequence.cs` | `DialogueSequence` — an ordered `List<DialogueLine>`. |
| `DialogueSequencePlayer.cs` | Plays a sequence through a `SimpleDialogueWindow`; `IAdvanceable`. |
| `SimpleDialogueWindow.cs` | The shared presenter. Speaker-driven; optional world-space follow. |
| `DialogueMB.cs` | Legacy `DialogueMB` (Cinemachine-gated). |
| `DialogueSpeaker.cs` | The per-character speaker component (`Name`, `Portrait`, `AudioSource`). |
| `DialoguePanel.cs` | Legacy per-NPC window. Superseded by `SimpleDialogueWindow`. |
| `DialogueTrigger.cs` | Bridges an `Interactable` to VIDE (`VIDE_Assign`). |
| `TimelineDialoguePresenter.cs` | Bridges `DialogueTimelineBehaviour` events to a `SimpleDialogueWindow`, so timeline/cutscene lines present as subtitles. One per cutscene window. |
| `CustomTimelineTracks/Dialogue/` | `DialogueTimelineTrack` → `DialogueTimelineClip` → `DialogueTimelineBehaviour` (the timeline line; `DialogueTimelineClip : IHoldPoint`). |
| `AudioRecording.cs` / mic code | **[built]** — recording now goes through the `[Recordable]` attribute (see `Editor/Recordable/Recordable.md`). |

## Samples (`Samples/`)

Empty prefabs to populate, plus the cutscene sample. Each is a starting point; clone and customise.

| File | Seeds | Purpose |
|---|---|---|
| `BarkSample.prefab` | Bark rig | An NPC says a line in the world. |
| `NarrationSample.prefab` | Narration rig | Narration, area / objective / pickup lines. |
| `ExaminableSample.prefab` | Examinable rig | Examine an object → description (+VO). |
| `ConversationSample.prefab` | Conversation rig | Branching conversation with an NPC. |
| `ControlsSample.prefab` | Controls object | Advance/skip input for all of the above. |
| `CutsceneDialogueSample.prefab` | Cutscene rig | Scripted cutscene with dialogue (`PlayableDirector` + `SceneTimeline`). |
| `CutsceneDialogueTimeline.playable` | (the cutscene timeline) | One `DialogueTimelineTrack` with one `DialogueTimelineClip` (`Line`). |
| `DialogueControls.inputactions` | Control scheme | `Advance` (Space/Enter) + `SkipAll` (Escape). Drop-in; edit the bindings to taste. |
| `ControlsAdvance.asset`, `ControlsSkipAll.asset` | (references) | `InputActionReference`s for the two actions — drag onto a `PlaybackControls`. |

## Dependencies

- **Febucci Text Animator** (package `com.febucci.text-animator-unity`, assembly `Febucci.TextAnimatorForUnity.Runtime`) — the typewriter. Use `TypewriterComponent`; `ITypewriterProvider` is internal. The old `TypewriterCore`/`Febucci.TextAnimatorCore.Typing` names are pre-breaking-change and defunct.
- **VIDE** — branching conversations.
- **Cinemachine** — conversation camera (`DialogueCam`) and cutscene camera track.
- UniTask, UniRx.

## Known gaps / TODO

- `DialogueSequence`, `DialogueSequencePlayer` and `SimpleDialogueWindow` are built; no custom `DialogueSequence` drawer yet.
- **Two-stage advance is wired:** `SimpleDialogueWindow` implements `ITypewriterProgress` (root `stoogebag`) and registers with `PlaybackControls`, whose `Advance()` fills an in-progress typewriter first, then advances. Works for both the sequence and timeline drivers. There is an **advance grace**: for `advanceGraceSeconds` (default 0.3s, per-window) after a reveal finishes *on its own*, advance input is swallowed, so a press meant as "fill" that lands just as the line completes doesn't also skip it. A fill the player triggered carries no grace (the press already did the work; the next press advances).
- `Barker` is superseded by `DialogueSequencePlayer`; `PointOfInterest`, `DialogueManager`, `UIManager` are legacy stubs. Cleanup pending (human-led).
- `Editor/Recordable/Recordable.md` covers mic recording.
