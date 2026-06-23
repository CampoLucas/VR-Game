using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundary : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IBoundedObject>(out var bounded))
        {
            var parent = other.GetComponentInParent<IBoundedObject>(false);
            if (parent != null)
            {
                bounded = parent;
            }
            else
            {
                return;
            }
            
        }

        bounded.Reposition();
    }
}
