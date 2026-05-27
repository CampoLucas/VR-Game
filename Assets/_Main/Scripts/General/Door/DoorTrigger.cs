using UnityEngine;

namespace VRGame.Level
{
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField] private string requiredTag = "Player";
 
        private bool _triggered;
 
        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;
 
            _triggered = true;
            LevelManager.Instance.CompleteLevel();
        }
    }
}