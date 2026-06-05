using UnityEngine;

namespace VRGame.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform playerSpawnAnchor;

        private void Awake()
        {
            if (player == null || playerSpawnAnchor == null)
            {
                Debug.LogError($"[{nameof(PlayerSpawner)}] player or playerSpawnAnchor not assigned.", this);
                return;
            }

            player.position = playerSpawnAnchor.position;
            player.rotation = playerSpawnAnchor.rotation;
        }
    }
}
