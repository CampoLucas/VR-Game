using System.Collections;
using UnityEngine;

namespace VRGame.Audio
{
    /// <summary>
    /// Singleton persistente entre escenas.
    /// Gestiona la música de fondo con fade in/out entre dos clips (menú y juego).
    ///
    /// SETUP:
    ///   1. Crear un GameObject vacío en la escena del Menú llamado "SoundManager".
    ///   2. Añadirle este script.
    ///   3. Asignar menuMusic y gameMusic en el Inspector.
    ///   4. No duplicarlo en la escena del juego; el Singleton persiste automáticamente.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────────
        public static SoundManager Instance { get; private set; }

        // ─── Inspector ────────────────────────────────────────────────────────────
        [Header("Music Clips")]
        [Tooltip("Música que suena en la escena del menú.")]
        [SerializeField] private AudioClip menuMusic;

        [Tooltip("Música que suena durante el juego.")]
        [SerializeField] private AudioClip gameMusic;

        [Header("Settings")]
        [Tooltip("Duración del fade entre músicas (en segundos).")]
        [SerializeField] private float fadeDuration = 1.5f;

        [Tooltip("Volumen máximo de la música (0 – 1).")]
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.8f;

        // ─── Private ──────────────────────────────────────────────────────────────
        private AudioSource _audioSource;
        private Coroutine   _fadeCoroutine;

        // ─────────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Awake()
        {
            // Patrón Singleton con DontDestroyOnLoad
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSource();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        #region Public API

        /// <summary>Reproduce la música del menú con fade.</summary>
        public void PlayMenuMusic() => CrossFadeTo(menuMusic);

        /// <summary>Reproduce la música del juego con fade.</summary>
        public void PlayGameMusic() => CrossFadeTo(gameMusic);

        /// <summary>Detiene la música con fade.</summary>
        public void StopMusic()
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeOut());
        }

        /// <summary>Cambia el volumen en tiempo real.</summary>
        public void SetVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            // Solo aplica si no hay fade en curso
            if (_fadeCoroutine == null)
                _audioSource.volume = musicVolume;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        #region Private Methods

        private void SetupAudioSource()
        {
            _audioSource         = gameObject.AddComponent<AudioSource>();
            _audioSource.loop    = true;
            _audioSource.volume  = musicVolume;
            _audioSource.spatialBlend = 0f; // Sonido 2D (música de fondo)
        }

        private void CrossFadeTo(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning($"[SoundManager] El AudioClip es null. Asígnalo en el Inspector.");
                return;
            }

            // Si ya suena el mismo clip, no hacer nada
            if (_audioSource.clip == clip && _audioSource.isPlaying) return;

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeToClip(clip));
        }

        private IEnumerator FadeToClip(AudioClip newClip)
        {
            // ── Fade Out ──────────────────────────────────────────────────────────
            if (_audioSource.isPlaying)
            {
                float startVol = _audioSource.volume;
                float elapsed  = 0f;

                while (elapsed < fadeDuration)
                {
                    elapsed           += Time.deltaTime;
                    _audioSource.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
                    yield return null;
                }
            }

            _audioSource.Stop();
            _audioSource.volume = 0f;
            _audioSource.clip   = newClip;
            _audioSource.Play();

            // ── Fade In ───────────────────────────────────────────────────────────
            float elapsed2 = 0f;
            while (elapsed2 < fadeDuration)
            {
                elapsed2            += Time.deltaTime;
                _audioSource.volume  = Mathf.Lerp(0f, musicVolume, elapsed2 / fadeDuration);
                yield return null;
            }

            _audioSource.volume = musicVolume;
            _fadeCoroutine      = null;
        }

        private IEnumerator FadeOut()
        {
            float startVol = _audioSource.volume;
            float elapsed  = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed           += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
                yield return null;
            }

            _audioSource.Stop();
            _audioSource.volume = musicVolume;
            _fadeCoroutine      = null;
        }

        #endregion
    }
}
