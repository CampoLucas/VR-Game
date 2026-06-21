using System;
using Oculus.Interaction;
using UnityEngine;
using VRGame.General.Interactables;

namespace VRGame.Puzzles
{
    public class Gear : MonoBehaviour
    {

        public SnapInteractor Interactor => interactor;

        [Header("References")]
        [SerializeField] private Transform parent;

        [SerializeField] private Grabbable grabbable;
        [SerializeField] private GameObject interactables;
        [SerializeField] private SnapInteractor interactor;
        [SerializeField] private Rigidbody rb;

        private Transform _transform;


        private void Awake()
        {
            _transform = transform;
            if (!grabbable) grabbable = GetComponent<Grabbable>();
            if (!rb) rb = GetComponent<Rigidbody>();

            _transform.SetParent(parent);
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
    }
}
