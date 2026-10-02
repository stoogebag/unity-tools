#if CINEMACHINE

using EditorTools.Recordable.Editor;
using UnityEditor;
using UnityEngine;

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

    private void ShowSelection()
    {
        if (Selection.activeTransform)
            examinable = Selection.activeTransform.GetComponent<SimpleExaminableWithDialogue>();
    }

    public void OnInspectorUpdate()
    {
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

            EditorGUI.BeginChangeCheck();
            ex.popupName = EditorGUILayout.TextField("Name", ex.popupName);
            dmb.Lines[0].Text = EditorGUILayout.TextField("Description", dmb.Lines[0].Text);
            dmb.Lines[0].Clip = (AudioClip)EditorGUILayout.ObjectField("Clip", dmb.Lines[0].Clip, typeof(AudioClip), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ex);
                EditorUtility.SetDirty(dmb);
            }

            var transcribe = false;

            if (GUILayout.Button("record"))
                _clip = RecordableAudio.Start(RecordableAudio.GetActiveDevice(), 30);

            if (GUILayout.Button("save"))
            {
                dmb.Lines[0].Clip = RecordableAudio.Save(_clip, "Resources/audioRecordings/clip");
                EditorUtility.SetDirty(dmb);
                if (AutoTranscribe) transcribe = true;
            }

            if (GUILayout.Button("play"))
                RecordableAudio.Play(dmb.Lines[0].Clip);

            if (GUILayout.Button("transcribe") || transcribe)
            {
                var text = await RecordableAudio.Transcribe(dmb.Lines[0].Clip);
                if (!string.IsNullOrEmpty(text))
                {
                    dmb.Lines[0].Text = text;
                    EditorUtility.SetDirty(dmb);
                }
            }
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
