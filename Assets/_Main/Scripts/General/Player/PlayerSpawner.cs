using System;
using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterController = Oculus.Interaction.Locomotion.CharacterController;

namespace VRGame.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private FirstPersonLocomotor locomotor;
        [SerializeField] private Transform targetAnchor;

        private void Awake()
        {
            if (!locomotor)
            {
                locomotor = FindObjectOfType<FirstPersonLocomotor>();
            }

            //Spawn();
        }

        private void OnValidate()
        {
            if (!locomotor)
            {
                Debug.LogWarning($"[{nameof(PlayerSpawner)}] WARNING: The FirstPersonLocomotor reference is null, it will use FindObjectOfType in the awake");
            }
        }

        [ContextMenu("Spawn")]
        private void Spawn()
        {
            Spawn(targetAnchor);
        }

        private void Spawn(Transform target)
        {
            var pose = new Pose(target.position, target.rotation);
            var locomotionEvent = new LocomotionEvent(0, pose, LocomotionEvent.TranslationType.Absolute, LocomotionEvent.RotationType.Absolute);
            locomotor.HandleLocomotionEvent(locomotionEvent);
        }
    }
}
