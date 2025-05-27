// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// public class SimpleEditorWindowTemplate : EditorWindow
// {
//     
//     // [MenuItem("stooge/Tools/name")]
//     // public static void ShowWindow()
//     // {
//     //     EditorWindow.GetWindow(typeof(ExaminableWizard));
//     // } 
//
//     private void Awake() => ShowSelection();
//     private void OnSelectionChange() => ShowSelection();
//
//     public bool AutoTranscribe = true;
//     public string Device = "Microphone (NVIDIA Broadcast)";
//
//     private void ShowSelection()
//     {
//         if (Selection.activeTransform)
//             examinable = Selection.activeTransform.GetComponent<SimpleExaminableWithDialogue>();
//
//     }
//     public void OnInspectorUpdate()
//     {
//         // This will only get called 10 times per second.
//         Repaint();
//     }
//
//     private AudioClip _clip;
//
//     private async void OnGUI()
//     {
//         if (Selection.activeTransform == null)
//         {
//             GUILayout.Label($"Nothing selected.", EditorStyles.boldLabel);
//             return;
//         }
//     }
// }