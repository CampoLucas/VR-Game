using System;
using UnityEngine;

namespace VRGame.General.Bounds
{
    public class BoundedObjectRef : MonoBehaviour, IBoundedObject
    {
        public int BoundedInstanceID => _bounded?.BoundedInstanceID ?? 0;
        
        [SerializeField] private MonoBehaviour boundedObject;

        private IBoundedObject _bounded;

        private void Awake()
        {
            if (!boundedObject)
            {
                Debug.LogError($"[{nameof(BoundedObjectRef)}] ERROR: The bounded object reference is null.");
                return;
            }
            
            if (boundedObject is IBoundedObject castedObject)
            {
                _bounded = castedObject;
            }
            else
            {
                Debug.LogError($"[{nameof(BoundedObjectRef)}] ERROR: The bounded object reference in not an {nameof(IBoundedObject)}.");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!boundedObject)
            {
                Debug.LogError($"[{nameof(BoundedObjectRef)}] ERROR: The bounded object reference is null.");
            }
            else if (boundedObject is not IBoundedObject)
            {
                Debug.LogError($"[{nameof(BoundedObjectRef)}] ERROR: The bounded object reference in not an {nameof(IBoundedObject)}.");
            }
        }
#endif
        public void Reposition()
        {
            if (_bounded == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"[{nameof(BoundedObjectRef)}] ERROR: Trying to reposition a null boundedObject.");
#endif
                
                return;
            }
            
            _bounded.Reposition();
        }

        public bool IgnoreReposition() => _bounded?.IgnoreReposition() ?? true;
    }
}