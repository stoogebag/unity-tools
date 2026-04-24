using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class AsyncEvent<T>
{
    private readonly List<Func<T, UniTask>> _subscribers = new();

    public IDisposable Subscribe(Func<T, UniTask> handler)
    {
        _subscribers.Add(handler);
        return Disposable.Create(() => _subscribers.Remove(handler));
    }

    public async UniTask InvokeAsync(T arg, bool parallel = true)
    {
        if (_subscribers.Count == 0) return;

        if (parallel)
        {
            await UniTask.WhenAll(_subscribers.Select(h => h(arg)));
        }
        else
        {
            foreach (var handler in _subscribers)
            {
                await handler(arg);
            }
        }
    }
}

public class AsyncEvent
{
    private readonly List<Func<UniTask>> _subscribers = new();

    public IDisposable Subscribe(Func<UniTask> handler)
    {
        _subscribers.Add(handler);
        return Disposable.Create(() => _subscribers.Remove(handler));
    }

    public async UniTask InvokeAsync(bool parallel = true)
    {
        if (_subscribers.Count == 0) return;
        
        if(parallel) await UniTask.WhenAll(_subscribers.Select(h => h()));
        else
        {
            foreach (var handler in _subscribers)
            {
                await handler(); // Waits for each to finish before starting next
            }
        }
    }
}