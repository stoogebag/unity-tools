using System;
using System.Collections.Generic;
using stoogebag.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Scene-scoped owner of advance/skip input. Holds a stack of IAdvanceable targets so nested
/// playback (e.g. a cutscene timeline that opens a conversation) works: the top of the stack
/// is the current target. Drivers push themselves on play and pop on finish.
/// </summary>
public class PlaybackControls : Singleton<PlaybackControls>
{
    private readonly Stack<IAdvanceable> _stack = new Stack<IAdvanceable>();

    private readonly Subject<Unit> _onStarted = new Subject<Unit>();
    private readonly Subject<Unit> _onEnded = new Subject<Unit>();

    [SerializeField] private InputActionReference advanceAction;
    [SerializeField] private InputActionReference skipAllAction;

    public IAdvanceable Current => _stack.Count > 0 ? _stack.Peek() : null;
    public bool IsPlaying => _stack.Count > 0;
    public IObservable<Unit> OnStarted => _onStarted;
    public IObservable<Unit> OnEnded => _onEnded;

    private void OnEnable()
    {
        advanceAction?.action?.Enable();
        skipAllAction?.action?.Enable();
    }

    private void OnDisable()
    {
        advanceAction?.action?.Disable();
        skipAllAction?.action?.Disable();
    }

    private void Update()
    {
        if (advanceAction != null && advanceAction.action != null && advanceAction.action.WasPressedThisFrame())
            Advance();

        if (skipAllAction != null && skipAllAction.action != null && skipAllAction.action.WasPressedThisFrame())
            SkipAll();
    }

    public void Push(IAdvanceable target)
    {
        if (target == null || _stack.Contains(target)) return;
        _stack.Push(target);
        _onStarted.OnNext(Unit.Default);
    }

    public void Pop(IAdvanceable target)
    {
        if (_stack.Count == 0) return;
        if (!ReferenceEquals(_stack.Peek(), target)) return;
        _stack.Pop();
        if (_stack.Count == 0) _onEnded.OnNext(Unit.Default);
    }

    public void Advance() => Current?.Advance();

    public void SkipAll() => Current?.SkipAll();

    private void OnDestroy()
    {
        _onStarted.Dispose();
        _onEnded.Dispose();
    }
}
