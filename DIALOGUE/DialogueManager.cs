// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using stoogebag.Extensions;
// using stoogebag.Utils;
// using UniRx;
// using UnityEngine;
// using UnityEngine.Playables;
//
// public class DialogueManager : Singleton<DialogueManager>
// {
//
//     public event Action<DialogueLine> OnNarrationStart;
//     public IObservable<DialogueLine> NarrationStartObservable =>
//         Observable.FromEvent<DialogueLine>(h => OnNarrationStart += h, h => OnNarrationStart -= h);
//
//     public event Action<DialogueLine> OnNarrationEnd;
//     public IObservable<DialogueLine> NarrationEndObservable =>
//         Observable.FromEvent<DialogueLine>(h => OnNarrationEnd += h, h => OnNarrationEnd -= h);
//
//
//     public StringAudioSourceDictionary AudioSourcesDic;
//
//     public async UniTask PlayNarration(IInteractor interactor, DialogueMB dialogue)
//     {
//         if (dialogue == null)
//         {
//             Debug.LogWarning("dialogue not found!");
//             return;
//         }
//         else print($"playing narration {dialogue.Lines[0].Text}");
//         
//         foreach (var line in dialogue.Lines)
//         {
//             OnNarrationStart?.Invoke(line);
//             
//             //if(line.Clip != null) await line.Speaker.Play(line);
//           //  else await UniTask.WaitForSeconds(1);
//         }
//         OnNarrationEnd?.Invoke(dialogue.Lines[^1]);
//         
//     }
// }
//
// [Serializable]
// public class StringAudioSourceDictionary : UnitySerializedDictionary<string, AudioSource> { }
