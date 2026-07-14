using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;
using VRGame.Puzzles;

public class LeverPuzzle : Puzzle
{
    public Vector3 LeverMapping { get; private set; }
    
    [Header("Levers")]
    [SerializeField] private List<Lever> levers;
    
    [Header("Events")]
    [FormerlySerializedAs("onSucces")] [SerializeField] private UnityEvent onSuccess;

    private LeverState[] _targetStates;
    private IObserver<int, LeverState> _observer;

    protected override void Awake()
    {
        base.Awake();

        _targetStates = new LeverState[levers.Count];
        var mapping = Vector3.zero;
        
        for (var i = 0; i < levers.Count; i++)
        {
            _targetStates[i] = UnityEngine.Random.value > 0.5f ? LeverState.Up : LeverState.Down;
            mapping[i] = _targetStates[i] == LeverState.Up ? 1f : 0f;
        }
        
        LeverMapping = mapping;
        
        _observer = new LeverObserver(levers, _targetStates, OnSuccess);
        
        for (var i = 0; i < levers.Count; i++)
        {
            levers[i].OnLeverStateChanged.Attach(_observer);
        }
    }

    private void OnSuccess(bool success)
    {
        if (success)
        {
            SolvedState = true;
            onSuccess.Invoke();
        }
        else
        {
            SolvedState = false;
        }
    }

    protected override void OnDestroy()
    {
        for (var i = 0; i < levers.Count; i++)
        {
            levers[i].OnLeverStateChanged.Detach(_observer);
        }
        
        levers.Clear();
        onSuccess.RemoveAllListeners();
        _observer.Dispose();

        _observer = null;
        onSuccess = null;
        _targetStates = null;
        
        base.OnDestroy();
    }
}

public class LeverObserver : IObserver<int, LeverState>
{
    private Dictionary<int, LeverState> _targetStates = new();
    private Dictionary<int, bool> _correctState = new();
    private Action<bool> _onSuccess;
    private readonly int _maxLevers;
    private int _correctLevers;
    
    public LeverObserver(List<Lever> levers, LeverState[] targetStates, Action<bool> onSuccess)
    {
        _maxLevers = levers.Count;
        _onSuccess = onSuccess;
 
        for (var i = 0; i < levers.Count; i++)
        {
            _targetStates[levers[i].ID] = targetStates[i];
        }
    }
    
    public void OnNotify(int id, LeverState state)
    {
        var isCorrect = state == _targetStates[id];
        var wasCorrect = _correctState.TryGetValue(id, out var prev) && prev;
        if (isCorrect == wasCorrect) return;
 
        _correctState[id] = isCorrect;
        _correctLevers += isCorrect ? 1 : -1;
 
        if (_onSuccess != null) _onSuccess(_correctLevers >= _maxLevers);
    }

    public void Dispose()
    {
        _targetStates.Clear();
        _correctState.Clear();

        _targetStates = null;
        _correctState = null;
        _onSuccess = null;
    }
}
