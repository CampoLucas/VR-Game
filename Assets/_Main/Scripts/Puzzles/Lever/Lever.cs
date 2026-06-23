using System;using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;

public enum LeverState
{
    Default,
    Up,
    Down
}

public class Lever : MonoBehaviour
{
    public int ID => id;
    public LeverState State { get; private set; }

    /// <summary>
    /// A broadcast from when the state of the lever is changed.
    /// </summary>
    public ISubject<int, LeverState> OnLeverStateChanged { get; private set; } = new Subject<int, LeverState>();

    [Header("Settings")]
    [SerializeField] private int id = 0;
    
    [Header("Lever")]
    [SerializeField] private GameObject lever;
    [FormerlySerializedAs("audio")] [SerializeField] private AudioSource sfx;
    [SerializeField] private string activatorTag;
    
    public void LeverStateChanged(LeverState value) 
    {
        State = value;
        OnLeverStateChanged.NotifyAll(id, value);
        //manager.CheckLeversCombination();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!lever || lever.activeSelf) return;
        if (!other.CompareTag(activatorTag)) return;
        
        other.gameObject.SetActive(false);
        lever.SetActive(true);
        sfx.Play();
    }

    public void Dispose()
    {
        OnLeverStateChanged?.Dispose();
    }

    

    private void OnDestroy()
    {
        Dispose();
    }
}
