using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPuzzleManager : MonoBehaviour
{
    [SerializeField] private List<LeverController> levers;
    [SerializeField] private List<int> leverStates;
    [SerializeField] private bool correctCombination;
   
    private void Awake()
    {
        leverStates = new List<int>();
    }
    void Start()
    {
        for (int i = 0; i < levers.Count; i++) 
        {
            leverStates.Add(UnityEngine.Random.Range(0, 2));
        }
    }
    public void CheckLeversCombination() 
    {
        correctCombination = true;
        for (int i = 0; i < levers.Count; i++)
        {
            if (levers[i].State != leverStates[i]) correctCombination = false;
        }
        if (correctCombination) Debug.Log("Lever Puzzle Resolve");
    }
}
