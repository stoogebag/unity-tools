using System.Collections;
using System.Collections.Generic;
using stoogebag.Utils;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{ 
 
    
    
    public static Quest TalkToNeighbour1 = new Quest()
    {
        Name = "Talk to the mystery voice",
        Description = "Talk to the mystery voice",
        QuestsUnlockedOnCompletion = new List<Quest>{ },
    };

    
    
    
    public void StartQuest(Quest q)
    {
        q.State = Quest.QuestState.InProgress;
    }
    
    public void QuestComplete(Quest quest)
    {
        quest.State = Quest.QuestState.Completed;
        foreach (var q in quest.QuestsUnlockedOnCompletion)
        {
            StartQuest(q);
        }
    }
    
    
    
}

public class Quest
{
    public string Name;
    public string Description;

    public QuestState State;
    
    public enum QuestState
    {
        Unstarted,
        InProgress,
        Completed,
        Failed,
    }
    
    public List<Quest> QuestsUnlockedOnCompletion = new List<Quest>(); 
}
