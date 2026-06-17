using UnityEngine;

namespace VRGame.Player
{
    /// <summary>
    /// Fades the VR view to black when the player's head enters solid geometry
    /// (camera clipping) and fades back out when it leaves. Uses OVRScreenFade so
    /// the fade renders fullscreen in stereo (the ScreenSpaceOverlay UI fade does not).
    /// </summary>
    public class CameraClipFade : MonoBehaviour
    {
        #region Serializable Variables

        [Header("Detection")]
        [Tooltip("Head transform to probe. Defaults to this transform.")]
        [SerializeField] private Transform head;
        [Tooltip("Layers considered solid geometry.")]
        [SerializeField] private LayerMask geometryMask = ~0;
        [Tooltip("Probe sphere radius around the head, in meters.")]
        [SerializeField] private float probeRadius = 0.12f;

        [Header("Fade")]
        [SerializeField] private OVRScreenFade screenFade;
        [Tooltip("Seconds to reach full black / full clear.")]
        [SerializeField] private float fadeDuration = 0.15f;

        #endregion

        #region Private Variables

        private float _fade; // 0 = clear, 1 = black

        #endregion

        #region Unity Lifecycle

        private void Reset()
        {
            head = transform;
            screenFade = GetComponent<OVRScreenFade>();
        }

        private void Awake()
        {
            if (head == null) head = transform;
        }

        private void LateUpdate()
        {
            if (screenFade == null) return;

            var inside = Physics.CheckSphere(head.position, probeRadius, geometryMask, QueryTriggerInteraction.Ignore);
            var target = inside ? 1f : 0f;
            var step = fadeDuration > 0f ? Time.deltaTime / fadeDuration : 1f;
            _fade = Mathf.MoveTowards(_fade, target, step);

            screenFade.SetExplicitFade(_fade);
        }

        #endregion
    }
}
