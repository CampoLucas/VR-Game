using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverStateTrigger : MonoBehaviour
{
    [SerializeField] private LeverController controller;
    [SerializeField] private string targetTag;
    [SerializeField] private int code;
    [SerializeField] private int nullCode;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == targetTag) 
        {
            controller.LeverStateChanged(code);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == targetTag)
        {
            controller.LeverStateChanged(nullCode);
        }
    }
}
