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
    [System.Serializable]
    public class LeverData : IDisposable
    {
        [field: SerializeField] public Lever Lever { get; private set; }
        [field: SerializeField] public MeshRenderer Indicator { get; private set; }
        public LeverState TargetState { get; private set; }

        public void Init(Material upMat, Material downMat)
        {
            TargetState = UnityEngine.Random.value > 0.5f ? LeverState.Up : LeverState.Down;
            Indicator.material = TargetState == LeverState.Up ? upMat : downMat;
        }

        public void Dispose()
        {
            Lever = null;
            Indicator = null;
        }
    }
    
    [Header("Levers")]
    [FormerlySerializedAs("leverDatas")] [SerializeField] private List<LeverData> leversData;
    
    [Header("Visuals")]
    [SerializeField] private Material upMaterial;
    [SerializeField] private Material downMaterial;
    
    [Header("Events")]
    [FormerlySerializedAs("onSucces")] [SerializeField] private UnityEvent onSuccess;

    private IObserver<int, LeverState> _observer;

    protected override void Awake()
    {
        base.Awake();

        for (var i = 0; i < leversData.Count; i++)
        {
            leversData[i].Init(upMaterial, downMaterial);
        }
        
        _observer = new LeverObserver(leversData, OnSuccess);
        
        for (var i = 0; i < leversData.Count; i++)
        {
            leversData[i].Lever.OnLeverStateChanged.Attach(_observer);
        }

        for (var i = 0; i < leversData.Count; i++)
        {
            Debug.Log($"Test: target = {leversData[i].TargetState}");
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
        for (var i = 0; i < leversData.Count; i++)
        {
            var data = leversData[i];
            data.Lever.OnLeverStateChanged.Detach(_observer);
            data.Dispose();
        }
        
        
        leversData.Clear();
        onSuccess.RemoveAllListeners();
        _observer.Dispose();

        _observer = null;
        onSuccess = null;
        upMaterial = null;
        downMaterial = null;
        
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
    
    public LeverObserver(List<LeverPuzzle.LeverData> leversData, Action<bool> onSuccess)
    {
        _maxLevers = leversData.Count;
        _onSuccess = onSuccess;
 
        foreach (var leverData in leversData)
        {
            _targetStates[leverData.Lever.ID] = leverData.TargetState;
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
