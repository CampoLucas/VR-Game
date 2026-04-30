using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [SerializeField] private GameObject lever;
    [SerializeField] private Collider leverActivator;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other == leverActivator) 
        {
            other.gameObject.SetActive(false);
            lever.SetActive(true);
        }
    }
}
