using System;
using Oculus.Interaction;
using UnityEngine;

namespace VRGame.General.Bounds
{
    public class BoundedGrabbable : MonoBehaviour, IBoundedObject
    {
        public int BoundedInstanceID => GetInstanceID();

        [Header("References")]
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private GameObject interactables;
        [SerializeField] private Rigidbody rb;
        
        private Transform _transform;
        private Vector3 _startPos;
        private Quaternion _startRot;

        private void Awake()
        {
            _transform = transform;
            _startPos = _transform.position;
            _startRot = _transform.rotation;
        }
        
        public void Reposition()
        {
            if (grabbable.SelectingPointsCount > 0)
            {
                return;
            }
            
            // Disable interactables
            grabbable.enabled = false;
            interactables.SetActive(false);
            
            // Kill all momentum
            rb.velocity = Vector3.zero;
            
            // Set rigibody to kinematic
            rb.isKinematic = true;
            
            // Change position
            _transform.position = _startPos;
            _transform.rotation = _startRot;
            
            // Set rigidboy to not kinematic
            rb.isKinematic = false;
            
            // Enable interactables
            grabbable.enabled = true;
            interactables.SetActive(true);
        }

        public bool IgnoreReposition() => grabbable.SelectingPointsCount > 0;

        private void OnDestroy()
        {
            grabbable = null;
            interactables = null;
            rb = null;
        }
    }
}