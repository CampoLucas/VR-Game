using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Serialization;

namespace VRGame.General.Interactables
{
    public class GearSnapPoint : SnapInteractable
    {
        public float RotSpeed => rotator ? rotator.RotSpeed : rotationSpeed;
        public bool RotDir => rotator ? !rotator.RotDir : invertDirection;
        
        [Header("Snap Position & Rotation")]
        [SerializeField] private Vector3 offset;

        [Header("Settings")]
        [SerializeField] private bool dissableGrabbable = true;

        [Header("Current")]
        [SerializeField] private Gear currentGear;

        [Header("Rotation Anim")]
        [SerializeField] private Transform origin;
        [SerializeField] private GearSnapPoint rotator;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private bool invertDirection = false;

        [Header("Hover")]
        [SerializeField] private GameObject hoverVisual;

        private SnapInteractor _interactor;
        private Transform _transform;
        private GearSnapPointFilter _filter;

        protected override void Awake()
        {
            base.Awake();
            if (!origin) origin = transform;
            _transform = origin;
        }


        protected override void Start()
        {
            base.Start();
            if (currentGear)
            {
                SnapOn(currentGear, currentGear.Interactor);
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            WhenStateChanged += OnStateChangedHandler;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            WhenStateChanged -= OnStateChangedHandler;
        }
        
        private void Update()
        {
            if (IsFree())
            {
                return;
            }

            var hasRotator = rotator;
            if (hasRotator && rotator.IsFree())
            {
                return;
            }

            var speed = RotSpeed;
            
            speed *= RotDir ? -1 : 1;
            _transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }

        private void OnStateChangedHandler(InteractableStateChangeArgs obj)
        {
            if (hoverVisual)
            {
                hoverVisual.SetActive(obj.NewState == InteractableState.Hover);
            }
            
            if (!IsFree()) return;
            
//             if (obj.PreviousState == InteractableState.Hover && obj.NewState == InteractableState.Select)
//             {
// #if false
//                 foreach (var interactor in Interactors)
//                 {
//                     if (interactor.Data is Gear gear)
//                     {
//                         SnapOn(gear, interactor);
//                     }
//                 }
// #else
//                 var interactor = Interactors.First();
//                 if (interactor.Data is Gear gear)
//                 {
//                     SnapOn(gear, interactor);
//                 }
// #endif
//                 
//                 
//             }

        }

        protected override void SelectingInteractorAdded(SnapInteractor interactor)
        {
            
            if (interactor.Data is Gear gear)
            {
                SnapOn(gear, interactor);
            }
            base.SelectingInteractorAdded(interactor);
        }

        private void GearStateChanged(InteractorStateChangeArgs obj)
        {
            _interactor.WhenStateChanged -= GearStateChanged;
            
            SnapOff(currentGear);
        }

        private void SnapOn(Gear gear, SnapInteractor interactor)
        {
            currentGear = gear;
            _interactor = interactor;

            if (!dissableGrabbable)
            {
                _interactor.WhenStateChanged += GearStateChanged;
            }
            gear.SnapOn(new SnapParams(_transform, TransformOffset(_transform), Quaternion.LookRotation(_transform.forward, _transform.up), dissableGrabbable));
            //enabled = false;
        }

        private void SnapOff(Gear gear)
        {
            gear.SnapOff();
            currentGear = null;
            //enabled = true;
        }
        
        // protected override void SelectingInteractorAdded(SnapInteractor interactor)
        // {
        //     base.SelectingInteractorAdded(interactor);
        //     if (interactor.Data is not Gear gear || !IsFree())
        //     {
        //         return;
        //     }
        //     
        //     SnapOn(gear);
        // }

        // protected override void SelectingInteractorRemoved(SnapInteractor interactor)
        // {
        //     base.SelectingInteractorRemoved(interactor);
        //     if (!currentGear)
        //     {
        //         return;
        //     }
        //     
        //     SnapOff(currentGear);
        // }

        // protected override void SelectingInteractorAdded(SnapInteractor interactor)
        // {
        //     base.SelectingInteractorAdded(interactor);
        //     if (interactor.Data is not Gear gear)
        //     {
        //         return;
        //     }
        //
        //     currentGear = gear;
        //     gear.Snap(new SnapParams(_transform, TransformOffset(_transform), Quaternion.LookRotation(_transform.forward, _transform.up), dissableGrabbable));
        //     enabled = false;
        // }

        // protected override void InteractorAdded(SnapInteractor interactor)
        // {
        //     base.InteractorAdded(interactor);
        //     if (interactor.Data is not Gear gear)
        //     {
        //         return;
        //     }
        //
        //     currentGear = gear;
        //     gear.Snap(new SnapParams(_transform, TransformOffset(_transform), Quaternion.LookRotation(_transform.forward, _transform.up), dissableGrabbable));
        //     enabled = false;
        // }

        private Vector3 TransformOffset(Transform tr)
        {
            var x = tr.right;
            var y = tr.up;
            var z = tr.forward;

            return tr.position + ((x * offset.x) + (y * offset.y) + (z * offset.z));
        }

        public bool IsFree()
        {
            return !currentGear;
        }
        
        private void OnDrawGizmos()
        {
            var color = IsFree() ? Color.green : Color.red;
            color.a = .5f;
            Gizmos.color = color;

            var pos = TransformOffset(transform);
            Gizmos.DrawSphere(pos, .01f);
        }
    }

    public struct SnapParams
    {
        public Transform Parent { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public bool DisableGrabbable { get; private set; }
        
        public SnapParams(Transform parent, Vector3 position, Quaternion rotation, bool disableGrabbable)
        {
            Parent = parent;
            Position = position;
            Rotation = rotation;
            DisableGrabbable = disableGrabbable;
        }
    }

    
}