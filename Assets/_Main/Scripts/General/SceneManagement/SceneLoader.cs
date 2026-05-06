using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRGame.Audio;

namespace VRGame.SceneManagement
{
    /// <summary>
    /// Singleton persistente que gestiona la carga asíncrona de escenas.
    /// Incluye un delay para que el SoundManager pueda hacer el fade out antes de cargar.
    ///
    /// SETUP:
    ///   1. Añadir este script al mismo GameObject del SoundManager (o a uno propio).
    ///   2. En la escena del Menú, el MenuController llama a LoadScene().
    ///   3. Asegúrate de añadir TODAS las escenas en File → Build Settings.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────────
        public static SceneLoader Instance { get; private set; }

        // ─── Inspector ────────────────────────────────────────────────────────────
        [Header("Settings")]
        [Tooltip("Tiempo de espera antes de cargar la siguiente escena (permite el fade out de audio).")]
        [SerializeField] private float preLoadDelay = 1.5f;

        // ─── Private ──────────────────────────────────────────────────────────────
        private bool _isLoading;

        // ─────────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        #region Public API

        /// <summary>Carga una escena por nombre (debe estar en Build Settings).</summary>
        public void LoadScene(string sceneName)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] Ya hay una escena cargándose. Ignorando solicitud.");
                return;
            }
            StartCoroutine(LoadRoutine(sceneName));
        }

        /// <summary>Carga una escena por índice (Build Settings).</summary>
        public void LoadScene(int sceneIndex)
        {
            if (_isLoading)
            {
                Debug.LogWarning("[SceneLoader] Ya hay una escena cargándose. Ignorando solicitud.");
                return;
            }
            StartCoroutine(LoadRoutine(sceneIndex));
        }

        /// <summary>Recarga la escena activa.</summary>
        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        #region Private Routines

        private IEnumerator LoadRoutine(string sceneName)
        {
            _isLoading = true;

            // Fade out de la música antes de cargar
            if (SoundManager.Instance != null)
                SoundManager.Instance.StopMusic();

            yield return new WaitForSeconds(preLoadDelay);

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;

            // Esperar a que cargue casi por completo (Unity para en 0.9)
            while (asyncOp.progress < 0.9f)
                yield return null;

            asyncOp.allowSceneActivation = true;
            _isLoading = false;
        }

        private IEnumerator LoadRoutine(int sceneIndex)
        {
            _isLoading = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.StopMusic();

            yield return new WaitForSeconds(preLoadDelay);

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
            asyncOp.allowSceneActivation = false;

            while (asyncOp.progress < 0.9f)
                yield return null;

            asyncOp.allowSceneActivation = true;
            _isLoading = false;
        }

        #endregion
    }
}
