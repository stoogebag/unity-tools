# Recordable — editor microphone recording

One implementation of "record from the mic, trim, save as a WAV asset, assign it to an `AudioClip` field, optionally transcribe it with Whisper". It replaces the several near-duplicate versions that had accumulated across the dialogue code (`RecordMicrophone`, the per-field `Record`/`Save`/`Transcribe` methods, and the copies inside `ExaminableWizard` and `VIDE_Editor`).

## Files

| File | Assembly | Job |
|---|---|---|
| `Assets/stoogebag/Recordable/RecordableAttribute.cs` | `stoogebag` (runtime) | `[Recordable]` attribute on `AudioClip` fields. |
| `Assets/stoogebag/Recordable/IRecordablePathProvider.cs` | `stoogebag` (runtime) | Optional interface for custom save paths. |
| `Assets/stoogebag/Editor/Recordable/RecordableAudio.cs` | `stoogebag.editor` (editor) | The actual `Microphone`/`SavWav`/`AudioUtils`/Whisper calls. |
| `Assets/stoogebag/Editor/Recordable/Editor/RecordablePropertyDrawer.cs` | `stoogebag.editor` (editor) | Draws the Record / Stop / Play / Save (and optional Transcribe) buttons. |

The attribute and interface live in a **runtime** assembly because runtime fields (`DialogueLine.Clip`, `DialogueBehaviour.Clip`) decorate themselves with it, and a runtime assembly cannot reference an editor-only one. The drawer and the mic core stay editor-only.

## Usage

Put `[Recordable]` on any `AudioClip` field:

```csharp
[Recordable]
public AudioClip line;

[Recordable(maxLengthSeconds: 30, transcribeIntoField: nameof(Text))]
public AudioClip Clip;
public string Text;
```

The inspector then shows the normal clip slot plus Record / Stop / Play / Save, and — when `transcribeIntoField` is set and Whisper is available — a Transcribe button that writes the result into that string field.

### Attribute options

| Argument | Default | Meaning |
|---|---|---|
| `maxLengthSeconds` | `30` | Maximum recording length passed to `Microphone.Start`. |
| `saveFolder` | `"Resources/audioRecordings"` | Folder (under `Assets/`) to save into. |
| `saveFolderField` | `null` | Sibling string field whose value is appended as a subfolder. |
| `fileNamePrefix` | `"recording"` | Base file name. |
| `fileNameField` | `null` | Sibling string field whose value replaces the prefix. |
| `transcribeIntoField` | `null` | Sibling string field to receive the Whisper transcription. Omit for no Transcribe button. |

Saved files are named `{basePath}-{guid}.wav`.

## Save paths

`GetSaveBasePath` resolves in this order:

1. If the object that owns the field implements `IRecordablePathProvider`, use `GetRecordableSavePath(fieldName)`.
2. Otherwise `saveFolder` + optional `saveFolderField`, with `fileNameField` (or `fileNamePrefix`).

```
public class MyTimelineClip : MonoBehaviour, IRecordablePathProvider
{
    public string GetRecordableSavePath(string fieldName) => $"timelines/{name}/{fieldName}";
}
```

## Transcription (optional)

Whisper is optional and never a hard dependency:

- The `WHISPER` define comes from a `versionDefines` entry on `stoogebag`, `stoogebag.editor` and `stoogebag.dialogue.editor`, so it is defined only when `com.whisper.unity` is installed.
- All Whisper code sits behind `#if WHISPER`; with the package absent, `RecordableAudio.Transcribe` logs and returns `null`.

## Calling it from a custom editor

The property drawer only covers `[Recordable]` fields. Custom editor UIs that record into their own data (e.g. `VIDE_Editor`'s dialogue nodes) use the core directly:

```csharp
_clip = RecordableAudio.Start(RecordableAudio.GetActiveDevice(), 60);
...
db.playerDiags[id].comment[i].audios = RecordableAudio.Save(_clip, "Resources/audioRecordings/clip");
...
var text = await RecordableAudio.Transcribe(clip);
```

`RecordableAudio` exposes `GetActiveDevice`, `Start`, `Stop`, `Save`, `Play`, `Transcribe`.

## Dependencies

Kept and used: `SavWav` (WAV trim/save), `RecordingSettings` (selected mic device + prefs), `AudioUtils` / `AudioUtility` (editor preview playback).

## Removed

`Editor/RecordMicrophone.cs`, `DIALOGUE/Editor/RecordMicrophone.cs`, `Editor/RecordableAudioClipDrawer.cs` (unused stub), `DIALOGUE/AudioRecording.cs`, and the bespoke record methods on `DialogueLine`, `DialogueBehaviour` and `ExaminableWizard`.
