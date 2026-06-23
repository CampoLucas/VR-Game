using UnityEngine;
using UnityEngine.Events;

namespace VRGame.Puzzles.Elements.PressurePlate
{
    /// <summary>
    /// Detects when the correct box (Caja) rests on the plate and solves the puzzle.
    /// The plate sinks slightly while the box is resting and emits a Debug.Log once.
    /// Only the rigidbody referenced by <see cref="targetBox"/> triggers the puzzle.
    /// </summary>
    public class PressurePlate : Puzzle
    {
        #region Serializable Variables

        [Header("Detection")]
        [Tooltip("The only box that solves this plate. Compared by attached Rigidbody.")]
        [SerializeField] private Rigidbody targetBox;
        [Tooltip("If enabled, the puzzle only solves when the box is resting (not held).")]
        [SerializeField] private bool requireNotGrabbed = true;

        [Header("Visual")]
        [Tooltip("Transform that sinks down. Defaults to this transform if left empty.")]
        [SerializeField] private Transform plateVisual;
        [SerializeField] private float sinkDepth = 0.02f;
        [SerializeField] private float sinkSpeed = 4f;

        [Header("Event")]
        [SerializeField] private UnityEvent onSolved;

        #endregion

        #region Private Variables

        private Vector3 _restLocalPos;
        private Vector3 _sunkLocalPos;
        private bool _boxInside;

        #endregion

        #region Unity Lifecycle

        protected sealed override void Awake()
        {
            base.Awake();
            if (plateVisual == null) plateVisual = transform;
            _restLocalPos = plateVisual.localPosition;
            _sunkLocalPos = _restLocalPos - Vector3.up * sinkDepth;
        }

        private void Update()
        {
            var resting = _boxInside && (!requireNotGrabbed || !IsBoxGrabbed());
            var target = resting ? _sunkLocalPos : _restLocalPos;
            plateVisual.localPosition = Vector3.Lerp(plateVisual.localPosition, target, sinkSpeed * Time.deltaTime);

            if (resting && !SolvedState)
            {
                SolvedState = true;
                Debug.Log("[PressurePlate] Puzzle resuelto");
                onSolved.Invoke();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTargetBox(other)) _boxInside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTargetBox(other)) _boxInside = false;
        }

        #endregion

        #region Private Methods

        private bool IsTargetBox(Collider other)
        {
            return targetBox != null && other.attachedRigidbody == targetBox;
        }

        // While held, the Grabbable sets the rigidbody kinematic (kinematicWhileSelected),
        // so a kinematic target means it is currently grabbed rather than resting.
        private bool IsBoxGrabbed()
        {
            return targetBox != null && targetBox.isKinematic;
        }

        #endregion

        protected override void OnDestroy()
        {
            onSolved.RemoveAllListeners();
            
            targetBox = null;
            plateVisual = null;
            onSolved = null;
            
            base.OnDestroy();
        }
    }
}
