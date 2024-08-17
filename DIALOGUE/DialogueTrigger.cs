using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using VIDE_Data;

[RequireComponent(typeof(Interactable))]
public class DialogueTrigger :MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Interactable>().OnInteractionObservable.Subscribe(async interactor =>
        {
            //GetComponentInChildren<PlayableDirector>().Play();
            await RunDialogue();
        });
        uiManager =        FindObjectOfType<VIDEUIManagerStooge>(true);
        block = false;
    }

    [Button]
    private async UniTask RunDialogue()
    {
        
        VIDEUIManagerStooge uiManager = FindObjectOfType<VIDEUIManagerStooge>(true);
        uiManager.NPC_audioSource = GetComponent<DialogueSpeaker>().AudioSource;
        
        await GetComponentInChildren<SkippableTimeline>().Play();

        Running = false;
    }

    private static bool block = false;
    private void Update()
    {
        if (Running)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(block) return;
                if(VD.isActive) uiManager.CallNext();
                Block();
            }

            //if (!VD.isActive) Running = false;
        }
        
    }
    
    async UniTask Block()
    {
        block = true;
        await UniTask.Delay(100);
        block = false;
    }

    public void TriggerDialogue()
    {
        if (!Running)
        {
            Running = true;
            uiManager.NPC_audioSource = GetComponent<DialogueSpeaker>().AudioSource;
            uiManager.Interact(GetComponent<VIDE_Assign>());
        }
    }

    private VIDEUIManagerStooge uiManager;

    public bool Running;

    public bool ConditionMet()
    {
        return !VD.isActive;
    }
}

public abstract class TimelineConditionProvider : MonoBehaviour
{
    public abstract bool ConditionMet();
}