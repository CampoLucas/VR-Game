using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;
using VRGame.General.Interactables;
using VRGame.Puzzles;

public class GearPuzzle : Puzzle
{
    [FormerlySerializedAs("snapTriggers")] [SerializeField] private List<GearSnapPoint> snapPoints = new List<GearSnapPoint>();
    [SerializeField] private bool isCorrect = true;
    [SerializeField] private UnityEvent onSucces;

    private IObserver<int, bool> _gearChangedObserver;

    protected sealed override void Awake()
    {
        base.Awake();
        _gearChangedObserver = new ActionObserver<int, bool>(OnCheckGear);

        for (var i = 0; i < snapPoints.Count; i++)
        {
            var snap = snapPoints[i];
            if (!snap) continue;
            snap.OnGearChanged.Attach(_gearChangedObserver);
        }
    }

    public void CheckSolution() 
    {
        isCorrect = true;
        for(var i = 0; i < snapPoints.Count; i++)
        {
            if (snapPoints[i] && snapPoints[i].HasGear) continue;
            isCorrect = false;
            break;
        }
        
        if (isCorrect && !SolvedState)
        {
            SolvedState = true;
            onSucces.Invoke();
        }
        else if (!isCorrect)
        {
            SolvedState = false;
        }
    }

    private void OnCheckGear(int id, bool placed)
    {
        CheckSolution();
    }
    
    protected sealed override void OnDestroy()
    {
        if (_gearChangedObserver == null) return;

        for (var i = 0; i < snapPoints.Count; i++)
        {
            var snap = snapPoints[i];
            if (!snap) continue;
            snap.OnGearChanged.Detach(_gearChangedObserver);
        }
        
        base.OnDestroy();
    }
}
