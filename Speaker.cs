using UnityEngine;

[CreateAssetMenu()]
public class Speaker : ScriptableObject
{
    public string Name;


    public AudioSource GetAudioSource()
    {
        return DialogueManager.Instance.AudioSourcesDic[this.name];
    }

}