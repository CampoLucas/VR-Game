using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [SerializeField] private GameObject lever;
    [SerializeField] private string activatorTag;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!lever.active)
        {
            if (other.tag == activatorTag)
            {
                other.gameObject.SetActive(false);
                lever.SetActive(true);
            }
        }
    }
}
