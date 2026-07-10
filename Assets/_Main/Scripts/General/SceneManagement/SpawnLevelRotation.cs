using System.Collections;
using UnityEngine;

namespace VRGame.SceneManagement
{
    /// <summary>
    /// Rotates the level root around the Y axis on spawn so the UI ends up
    /// centered in front of wherever the player is actually looking.
    /// Rotating the level instead of the rig avoids fighting HMD tracking,
    /// and works on Quest builds where OVRDisplay.RecenterPose() is ignored.
    /// </summary>
    public class SpawnLevelRotation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform head;      // CenterEyeAnchor
        [SerializeField] private Transform rooms;     // Level root to rotate
        [SerializeField] private Transform uiCenter;  // Point the player should face (Rooms/UI)

        [Header("Settings")]
        [Tooltip("Max seconds to wait for valid HMD tracking before aligning anyway.")]
        [SerializeField] private float trackingTimeout = 2f;
        [Tooltip("Re-align the level when the user recenters the view (long-press Oculus button).")]
        [SerializeField] private bool realignOnRecenter = true;

        private bool _subscribed;

        private IEnumerator Start()
        {
            yield return WaitForValidTracking();
            Align();

            if (realignOnRecenter && OVRManager.display != null)
            {
                OVRManager.display.RecenteredPose += OnRecentered;
                _subscribed = true;
            }
        }

        private void OnDestroy()
        {
            if (_subscribed && OVRManager.display != null)
            {
                OVRManager.display.RecenteredPose -= OnRecentered;
            }
        }

        private IEnumerator WaitForValidTracking()
        {
            float elapsed = 0f;
            while (elapsed < trackingTimeout)
            {
                // With floor-level tracking the eye anchor jumps to head height
                // once the HMD pose is valid; with eye-level tracking the
                // rotation stops being identity.
                bool positionValid = head.localPosition.sqrMagnitude > 0.0025f;
                bool rotationValid = head.localRotation != Quaternion.identity;
                if (positionValid || rotationValid)
                {
                    // One extra frame so the pose is stable before measuring it.
                    yield return null;
                    yield break;
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        private void OnRecentered()
        {
            StartCoroutine(AlignNextFrame());
        }

        private IEnumerator AlignNextFrame()
        {
            // Wait one frame so OVRCameraRig has applied the recentered pose.
            yield return null;
            Align();
        }

        private void Align()
        {
            Vector3 headForward = head.forward;
            headForward.y = 0f;
            if (headForward.sqrMagnitude < 0.0001f)
            {
                return; // Looking straight up/down, yaw is unreliable.
            }

            Vector3 toUI = uiCenter.position - rooms.position;
            toUI.y = 0f;
            if (toUI.sqrMagnitude < 0.0001f)
            {
                return;
            }

            float deltaYaw = Vector3.SignedAngle(toUI, headForward, Vector3.up);
            rooms.Rotate(0f, deltaYaw, 0f, Space.World);
        }
    }
}
