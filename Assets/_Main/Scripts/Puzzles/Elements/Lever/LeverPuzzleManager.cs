using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverPuzzleManager : MonoBehaviour, IPuzzleInterface
{
    [SerializeField] private List<LeverController> levers;
    [SerializeField] private List<int> leverStates;
    [SerializeField] private List<MeshRenderer> leverIndicators;
    [SerializeField] private Material upMaterial;
    [SerializeField] private Material downMaterial;
    [SerializeField] private bool correctCombination;
    [SerializeField] private bool isResolved = false;
    [SerializeField] private UnityEvent onSucces;


    private void Awake()
    {
        leverStates = new List<int>();
    }
    void Start()
    {
        for (int i = 0; i < levers.Count; i++) 
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
        for (int i = 0; i < levers.Count; i++)
        {
            if (levers[i].State != leverStates[i]) correctCombination = false;
        }
        if (correctCombination) 
        {
            isResolved = true;
            onSucces.Invoke();
        }
        else isResolved = false;
    }
    public bool GetIsResolved() 
    {
        return isResolved;
    }
}
