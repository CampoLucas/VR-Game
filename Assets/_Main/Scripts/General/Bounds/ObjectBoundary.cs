using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundary : MonoBehaviour
{
    private readonly HashSet<int> _cachedRepositioned = new();
    
    private void LateUpdate()
    {
        _cachedRepositioned.Clear();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IBoundedObject>(out var bounded) || 
            bounded.IgnoreReposition() ||
            !_cachedRepositioned.Add(bounded.BoundedInstanceID))
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log($"[LOG]: ({nameof(ObjectBoundary)}) Out of bounds repositioning object (BoundedInstanceID: {bounded.BoundedInstanceID})");
#endif
        bounded.Reposition();
    }
}
