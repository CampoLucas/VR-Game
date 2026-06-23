using UnityEngine;
using CharacterController = Oculus.Interaction.Locomotion.CharacterController;

namespace VRGame.Level
{
    public class DoorTrigger : MonoBehaviour
    {
        //[SerializeField] private string requiredTag = "Player";
 
        private bool _triggered;
 
        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (!other.gameObject.TryGetComponent<CharacterController>(out _)) return;
 
            _triggered = true;
            LevelManager.Instance.CompleteLevel();
        }
    }
}