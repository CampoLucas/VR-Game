using System;
using Oculus.Interaction;
using UnityEngine;
using VRGame.General.Interactables;

namespace VRGame.Puzzles
{
    public class Gear : MonoBehaviour, IBoundedObject
    {
        public int BoundedInstanceID => GetInstanceID();
        public SnapInteractor Interactor => interactor;

        [Header("References")]
        [SerializeField] private Transform parent;

        [SerializeField] private Grabbable grabbable;
        [SerializeField] private GameObject interactables;
        [SerializeField] private SnapInteractor interactor;
        [SerializeField] private Rigidbody rb;

        private Transform _transform;
        private Vector3 _startPos;
        private Quaternion _startRot;


        private void Awake()
        {
            _transform = transform;
            if (!grabbable) grabbable = GetComponent<Grabbable>();
            if (!rb) rb = GetComponent<Rigidbody>();

            _transform.SetParent(parent);

            _startPos = _transform.position;
            _startRot = _transform.rotation;
        }

        public void SnapOn(SnapParams gearSnapPoint)
        {
            // Disable the grababble
            if (gearSnapPoint.DisableGrabbable)
            {
                grabbable.enabled = false;
                interactables.SetActive(false);
            }

            // Set kinematic
            rb.isKinematic = true;

            _transform.position = gearSnapPoint.Position;
            _transform.rotation = gearSnapPoint.Rotation;

            _transform.SetParent(gearSnapPoint.Parent);
        }

        public void SnapOff()
        {
            grabbable.enabled = true;
            interactables.SetActive(true);


            _transform.SetParent(parent);
        }

        public void Reposition()
        {
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
    }
}
