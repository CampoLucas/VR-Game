using UnityEngine;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Elements.Button
{
    [RequireComponent(typeof(AudioSource))]
    public class ButtonAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ButtonPresenter presenter;
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip pressSound;
        

        private ActionObserver<int> _pressedObserver;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            
            _pressedObserver = new ActionObserver<int>(OnPressed);
            presenter.PressedSubject.Attach(_pressedObserver);
        }

        private void OnDestroy()
        {
           
            _pressedObserver?.Dispose();
        }

        private void OnPressed(int id)
        {
            if (pressSound != null && audioSource != null)
            {

                audioSource.PlayOneShot(pressSound);
            }
        }
    }
}