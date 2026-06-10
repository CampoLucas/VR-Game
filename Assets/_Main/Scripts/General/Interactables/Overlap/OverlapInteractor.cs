using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

namespace VRGame.General.Interactables
{
    public abstract class OverlapInteractor<TInteractor, TInteractable> : Interactor<TInteractor, TInteractable>
        where TInteractor : Interactor<TInteractor, TInteractable>
        where TInteractable : Interactable<TInteractor, TInteractable>
    {
        [Header("Detection")]
        [SerializeField] private LayerMask layer;
        [SerializeField] private float detectionInterval = .1f;
        [SerializeField, Range(1, 10)] private int maxOverlaps = 4;
        
        [Header("Origin & Size")]
        [SerializeField] protected Transform origin;
        [SerializeField] private float radius = 0.08f;
        
        private Transform _transform;
        protected bool InContact;
        
        private TInteractable _overlappingCandidate;
        private UpdateDelay _delay;
        private Collider[] _hits;

        private Dictionary<Collider, TInteractable> _interactableDict = new();
        
        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
            _delay = new UpdateDelay(detectionInterval, UpdateDetection);
            _hits = new Collider[maxOverlaps];
        }

        protected sealed override TInteractable ComputeCandidate()
        {
            return _overlappingCandidate;
        }

        protected override void Update()
        {
            base.Update();
            
            _delay.Run();
        }

        protected override bool ComputeShouldSelect()
        {
            return InContact;
        }

        protected override bool ComputeShouldUnselect()
        {
            return !InContact;
        }
        
        protected override void InteractableSelected(TInteractable interactable)
        {
            Debug.Log($"[{nameof(OverlapInteractor<TInteractor, TInteractable>)}] I selected: {interactable.name}");
        }

        protected override void InteractableUnselected(TInteractable interactable)
        {
            Debug.Log($"[{nameof(OverlapInteractor<TInteractor, TInteractable>)}] I released: {interactable.name}");
        }

        private Transform GetOrigin() => GetOrigin(_transform);
        private Transform GetOrigin(Transform defaultT) => origin ? origin : defaultT;

        private void UpdateDetection(float delta)
        {
            var tr = GetOrigin();
            var overlaps = Physics.OverlapSphereNonAlloc(tr.position, radius, _hits, layer, QueryTriggerInteraction.Ignore);
            
            TInteractable closestInteractable = null;
            var closestDist = float.MaxValue;

            for (var i = 0; i < overlaps; i++)
            {
                var hit = _hits[i];
                if (!hit) continue;

                if (!_interactableDict.TryGetValue(hit, out var interactable))
                {
                    interactable = hit.GetComponent<TInteractable>();
                    _interactableDict[hit] = interactable;
                }
                
                if (interactable == null) continue;

                var dist = Vector3.Distance(tr.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestInteractable = interactable;
                }
            }

            InContact = closestInteractable != null;

            _overlappingCandidate = closestInteractable;
        }

        protected override void OnDestroy()
        {
            _delay?.Dispose();
            _interactableDict?.Clear();
            
            
            origin = null;
            _transform = null;
            _overlappingCandidate = null;
            _delay = null;
            _interactableDict = null;
            
            base.OnDestroy();
            
        }

        private void OnDrawGizmos()
        {
            var origin = GetOrigin(transform);
            var position = origin.position;
            var isInteracting = State == InteractorState.Select;
 
            Gizmos.color = isInteracting ? Color.cyan : Color.green;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.35f);
            Gizmos.DrawSphere(position, radius);
 
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.85f);
            Gizmos.DrawWireSphere(position, radius);
        }
    }
}