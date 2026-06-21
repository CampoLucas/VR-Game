using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverStateTrigger : MonoBehaviour
{
    [SerializeField] private Lever controller;
    [SerializeField] private AudioSource audio;
    [SerializeField] private string targetTag;
    [SerializeField] private LeverState state;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == targetTag) 
        {
            audio.Play();
            controller.LeverStateChanged(state);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == targetTag)
        {
            controller.LeverStateChanged(LeverState.Default);
        }
    }
}
