using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundary : MonoBehaviour
{
    private readonly HashSet<int> _cachedRepositioned = new();
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void LateUpdate()
    {
        _cachedRepositioned.Clear();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IBoundedObject>(out var bounded) || 
            bounded.IgnoreReposition() ||
            _collider.bounds.Contains(other.bounds.center) ||
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
