using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    public Queue<Func<UniTask>> Queue = new();

    public void AddAction(Func<UniTask> action)
    {
        
        Queue.Enqueue(action);
    }

    [Button]
    async UniTask Test()
    {
        print("test.");
        await UniTask.WaitForSeconds(1);
        
        print("queuing 1");
        AddAction(()=>UniTask.WaitForSeconds(10));
        print("queuing 2");
        AddAction(()=>UniTask.WaitForSeconds(10));
        print("queuing 3");
        AddAction(()=>UniTask.WaitForSeconds(10));
    }
    
    private UniTask _runningTask;
    bool _currentlyRunningTask = false;
    private bool _paused = false;

    //doesn't literally pause a running action, but prevents new actions from running
    public void Pause()
    {
        _paused = true;
    }
    
    public void Unpause()
    {
        _paused = false;
    }
    
    private void Update()
    {
        if (!_paused && !_currentlyRunningTask)
        {
            if (Queue.Count > 0)
            {
                var taskFunc = Queue.Dequeue();
                RunTask(taskFunc).Forget();
            }
        }
    }

    private async UniTask RunTask(Func<UniTask> func)
    {
        _currentlyRunningTask = true;

        _runningTask = func();
        await _runningTask;
        _currentlyRunningTask = false;
    }
}