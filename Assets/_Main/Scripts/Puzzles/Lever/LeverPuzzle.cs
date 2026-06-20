using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRGame.Puzzles;

public class LeverPuzzle : Puzzle
{
    [SerializeField] private List<LeverController> levers;
    [SerializeField] private List<int> leverStates;
    [SerializeField] private List<MeshRenderer> leverIndicators;
    [SerializeField] private Material upMaterial;
    [SerializeField] private Material downMaterial;
    [SerializeField] private bool correctCombination;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onSucces;


    protected sealed override void Awake()
    {
        base.Awake();
        leverStates = new List<int>();
    }
    
    void Start()
    {
        for (var i = 0; i < levers.Count; i++) 
        {
            leverStates.Add(UnityEngine.Random.Range(0, 2));
            if (leverStates[i] > 0)
            {
                leverIndicators[i].material = upMaterial;
            }
            else 
            { 
                leverIndicators[i].material = downMaterial;
            }
        }
    }
    
    public void CheckLeversCombination() 
    {
        correctCombination = true;
        for (var i = 0; i < levers.Count; i++)
        {
            if (levers[i].State != leverStates[i]) correctCombination = false;
        }
        if (correctCombination) 
        {
            SolvedState = true;
            onSucces.Invoke();
        }
        else
        {
            SolvedState = false;
        }
    }

    protected override void OnDestroy()
    {
        levers.Clear();
        leverStates.Clear();
        leverIndicators.Clear();
        onSucces.RemoveAllListeners();

        levers = null;
        leverStates = null;
        leverIndicators = null;
        onSucces = null;
        upMaterial = null;
        downMaterial = null;
        
        base.OnDestroy();
    }
}
