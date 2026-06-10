using UnityEngine;
using System.Collections;

namespace VRGame.Audio
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;

        private IEnumerator Start()
        {
            
            yield return new WaitUntil(() => SoundManager.Instance != null);

            
            yield return null;

            SoundManager.Instance.PlayClip(clip);
        }
    }
}