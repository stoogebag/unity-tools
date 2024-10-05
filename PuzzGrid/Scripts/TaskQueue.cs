
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TaskQueue
{
    private Queue<Action> _queue = new Queue<Action>();

    public void AddTask(Action action)
    {
        
        
        _queue.Enqueue(action);
        if (Status == QueueStatus.Waiting) Run();
    }

    public async void Run()
    {
        if (_queue.Count == 0)
        {
            //nothing to run.
            Status = QueueStatus.Waiting;
            return;
        }

        Status = QueueStatus.Running;
        var action = _queue.Dequeue();
        Debug.Log("about to run new task.");
        var task = UniTask.RunOnThreadPool(action);

        task.ContinueWith(() =>TaskComplete(task));
    }

    private void TaskComplete(UniTask task)
    {
        Debug.Log("task complete.");
        Debug.Log($"{task.Status}");
        Run();
    }


    public QueueStatus Status;
    
    public enum QueueStatus
    {
        Waiting,
        Running,
        Paused, //todo, paused.
    }

}


