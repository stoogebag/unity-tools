#if CINEMACHINE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEditor;
using UnityEngine;
#if WHISPER
using Whisper;
#endif

public class ExaminableWizard : EditorWindow
{

    private SimpleExaminableWithDialogue examinable;

    [MenuItem("stooge/Tools/Examinables")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ExaminableWizard));
    } 

    private void Awake() => ShowSelection();
    private void OnSelectionChange() => ShowSelection();

    public bool AutoTranscribe = true;
    public string Device = "Microphone (NVIDIA Broadcast)";

    private void ShowSelection()
    {
        if (Selection.activeTransform)
            examinable = Selection.activeTransform.GetComponent<SimpleExaminableWithDialogue>();

    }
    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }

    private AudioClip _clip;
    
    private async void OnGUI()
    {
        if (Selection.activeTransform == null)
        {
            GUILayout.Label($"Nothing selected.", EditorStyles.boldLabel);
            return;
        }
        GUILayout.Label($"{Selection.activeTransform.name}", EditorStyles.boldLabel);
        
        if (examinable == null)
        {
            if (GUILayout.Button("create"))
            {
                if (Selection.activeTransform != null)
                {
                    var go = Selection.activeTransform.gameObject;
                    var ex = go.AddComponent<Examinable>();
                    var dm = go.AddComponent<DialogueMB>();
                    var e = go.AddComponent<SimpleExaminableWithDialogue>();
                    ex.popupName = go.name;
                    ShowSelection();
                }
            }
        }
        else
        {

            var dmb = examinable.GetComponent<DialogueMB>();
            var ex = examinable.GetComponent<Examinable>();
            ex.popupName = EditorGUILayout.TextField("Name", ex.popupName);
            dmb.Lines[0].Text = EditorGUILayout.TextField("Description", dmb.Lines[0].Text);
            dmb.Lines[0].Clip = (AudioClip)EditorGUILayout.ObjectField("Clip", dmb.Lines[0].Clip, typeof(AudioClip), false);


            var _transcribe = false;
    
    
            if (GUILayout.Button("record")){
                _clip = Microphone.Start(Device, false, 30, 44100);
                
            }

            if (GUILayout.Button("save"))
            {
                var guid = Guid.NewGuid();
                var wavPath = $"Resources\\audioRecordings\\clip-{guid}";
        
        
                var clipTrimmed = SavWav.TrimSilence(_clip, 0.001f);
                SavWav.Save(wavPath, clipTrimmed);
                Thread.Sleep(10); // Wait for 100 milliseconds
                AssetDatabase.ImportAsset("Assets/" + wavPath + ".wav",  ImportAssetOptions.ForceSynchronousImport);
                Thread.Sleep(10); // Wait for 100 milliseconds
                var myclip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/" + wavPath + ".wav");
                dmb.Lines[0].Clip = myclip;

                if (AutoTranscribe) _transcribe = true;
                
            }
#if ULTIMATE_EDITOR_ENHANCER
            if (GUILayout.Button("play"))
            {
                if(   dmb.Lines[0].Clip != null) AudioUtilsRef.PlayClip(dmb.Lines[0].Clip);
            }
#endif

#if WHISPER
            if (GUILayout.Button("transcribe") || _transcribe)
            {
                
                
                var manager = GameObjectExtensions.FindObjectOfTypeInActiveScene<WhisperManager>();
                if (manager == null) return;

                await manager.InitModel();
            
                var res = await manager.GetTextAsync(   dmb.Lines[0].Clip);
        
                if (res == null) 
                    Debug.Log("no output text!");

                dmb.Lines[0].Text = res.Result.Trim(' ');
            }
#endif
            
            
            
        }
    }


    private void OnWizardCreate()
    {
        if (examinable == null)
        {
            if (Selection.activeTransform != null)
            {
                Selection.activeTransform.gameObject.AddComponent<SimpleExaminableWithDialogue>();
                ShowSelection();
            }
        }
    }
}

#endif