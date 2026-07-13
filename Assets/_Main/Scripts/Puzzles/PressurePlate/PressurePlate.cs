using UnityEngine;
using UnityEngine.Events;

namespace VRGame.Puzzles.Elements.PressurePlate
{
    /// <summary>
    /// Detects when the correct box (Caja) or the player rests on the plate and solves the puzzle.
    /// The plate sinks while resting and unsolves when both leave the trigger.
    /// Only the rigidbodies referenced by <see cref="targetBox"/> and <see cref="targetPlayer"/> trigger the puzzle.
    /// </summary>
    public class PressurePlate : Puzzle
    {
        #region Serializable Variables

        [Header("Detection")]
        [Tooltip("The only box that solves this plate. Compared by attached Rigidbody.")]
        [SerializeField] private Rigidbody targetBox;
        [Tooltip("The player rigidbody that also solves this plate. Compared by attached Rigidbody.")]
        [SerializeField] private Rigidbody targetPlayer;
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
        private bool _playerInside;

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
            var boxResting = _boxInside && (!requireNotGrabbed || !IsBoxGrabbed());
            var resting = boxResting || _playerInside;
            var target = resting ? _sunkLocalPos : _restLocalPos;
            plateVisual.localPosition = Vector3.Lerp(plateVisual.localPosition, target, sinkSpeed * Time.deltaTime);

            if (resting && !SolvedState)
            {
                SolvedState = true;
                Debug.Log("[PressurePlate] Puzzle resuelto");
                onSolved.Invoke();
            }
            else if (!resting && SolvedState)
            {
                Debug.Log($"[PressurePlate] Puzzle desactivado — causa: {DeactivationReason()}");
                SolvedState = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTargetBox(other))
            {
                _boxInside = true;
                Debug.Log("[PressurePlate] Caja entró al trigger");
            }
            else if (IsTargetPlayer(other))
            {
                _playerInside = true;
                Debug.Log("[PressurePlate] Player entró al trigger");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTargetBox(other))
            {
                _boxInside = false;
                Debug.Log("[PressurePlate] Caja salió del trigger");
            }
            else if (IsTargetPlayer(other))
            {
                _playerInside = false;
                Debug.Log("[PressurePlate] Player salió del trigger");
            }
        }

        #endregion

        #region Private Methods

        private bool IsTargetBox(Collider other)
        {
            return targetBox != null && other.attachedRigidbody == targetBox;
        }

        private bool IsTargetPlayer(Collider other)
        {
            return targetPlayer != null && other.attachedRigidbody == targetPlayer;
        }

        // At deactivation, resting is always false, which requires _playerInside == false;
        // so the cause is either the box leaving the trigger or the box being grabbed.
        private string DeactivationReason()
        {
            if (_boxInside && IsBoxGrabbed()) return "Caja agarrada";
            return "Caja fuera del trigger";
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
            targetPlayer = null;
            plateVisual = null;
            onSolved = null;
            
            base.OnDestroy();
        }
    }
}
