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
| `DialogueClip` / `DialogueBehaviour` | Timeline clip, **[built]** | A line inside a Timeline. `DialogueClip : IHoldPoint` (`WantsHold = PauseTimeline`). |
| `PlaybackControls` | scene singleton (root `stoogebag`), **[built]** | Owns advance/skip input: two serialized `InputActionReference`s read in `Update`. Holds a **stack** of `IAdvanceable`; forwards to the top. Enable/disable the actions with the component. |
| `IAdvanceable` / `IHoldPoint` | interfaces (root `stoogebag`), **[built]** | Driver contract (`Advance`/`SkipAll`) / timeline-clip marker (`WantsHold`/`OnAdvance`). |
| VIDE | third-party, **[built]** | Branching conversations only. |

Presence is the toggle: empty `Text` = no subtitle, null `Clip` = silent, null `Portrait` = none.

## Presentation templates

| Template | Status | Notes |
|---|---|---|
| `SimpleDialogueWindow` | **[built]** | `Window` subclass: Febucci typewriter on TMP + optional label + optional portrait + VO `AudioSource` + continue indicator. Speaker-driven — reads `DialogueLine.Speaker` for name/portrait; `followSpeaker` tracks a world-space speaker. Prefab: `Prefabs/SimpleDialogueWindow.prefab`. |
| Legacy per-NPC panel | **[deprecated]** | `DialoguePanel` on `Prefabs/Dialogue.prefab` — one panel per speaker, matched by name. Superseded by `SimpleDialogueWindow`; retire when the timeline path is migrated. |
| `DialogueUIContainer` | **[built]** | VIDE's full conversation UI (`Prefabs/DialogueUIContainer.prefab`): portraits, text, choices. |

## Drivers

- **`SceneTimeline`** — **[built]** — a timeline; dialogue lines ride on a `DialogueTrack`.
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
| `CustomTimelineTracks/Dialogue/` | `DialogueTrack` → `DialogueClip` → `DialogueBehaviour` (the timeline line; `DialogueClip : IHoldPoint`). |
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
| `CutsceneDialogueTimeline.playable` | (the cutscene timeline) | One `DialogueTrack` with one `DialogueClip` (`Line`). |
| `DialogueControls.inputactions` | Control scheme | `Advance` (Space/Enter) + `SkipAll` (Escape). Drop-in; edit the bindings to taste. |
| `ControlsAdvance.asset`, `ControlsSkipAll.asset` | (references) | `InputActionReference`s for the two actions — drag onto a `PlaybackControls`. |

## Dependencies

- **Febucci Text Animator** (package `com.febucci.text-animator-unity`, assembly `Febucci.TextAnimatorForUnity.Runtime`) — the typewriter. Use `TypewriterComponent`; `ITypewriterProvider` is internal. The old `TypewriterCore`/`Febucci.TextAnimatorCore.Typing` names are pre-breaking-change and defunct.
- **VIDE** — branching conversations.
- **Cinemachine** — conversation camera (`DialogueCam`) and cutscene camera track.
- UniTask, UniRx.

## Known gaps / TODO

- `DialogueSequence`, `DialogueSequencePlayer` and `SimpleDialogueWindow` are built; no custom `DialogueSequence` drawer yet.
- **Two-stage advance not wired:** the runner owns advance, but finishing the line's typewriter first needs the typewriter, which lives in the dialogue UI — and `stoogebag.dialogue` can't reference `stoogebag.Timeline` (Timeline already references dialogue). Open.
- `Barker` is superseded by `DialogueSequencePlayer`; `PointOfInterest`, `DialogueManager`, `UIManager` are legacy stubs. Cleanup pending (human-led).
- `Editor/Recordable/Recordable.md` covers mic recording.
