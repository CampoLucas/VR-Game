using UnityEngine;

namespace VRGame.Audio
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
 
        private void Start()
        {
            SoundManager.Instance.PlayClip(clip);
        }
    }
}