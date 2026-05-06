using UnityEngine;
using VRGame.Audio;
using VRGame.SceneManagement;

namespace VRGame.UI.Menu
{
    /// <summary>
    /// Controlador principal del menú VR.
    ///
    /// SETUP EN LA ESCENA DE MENÚ:
    ///   1. Crear un GameObject vacío llamado "Managers" y añadirle:
    ///        - SoundManager
    ///        - SceneLoader
    ///   2. Crear otro GameObject llamado "MenuController" y añadirle este script.
    ///   3. En el campo "gameSceneName" escribir el nombre exacto de tu escena de juego.
    ///   4. En el ButtonPresenter del botón de inicio:
    ///        → Inspector → onPressed → (+) → arrastrar MenuController → OnStartButtonPressed()
    ///
    /// NOTA: El SoundManager y SceneLoader tienen DontDestroyOnLoad,
    ///       así que NO los pongas en la escena del juego.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────────────
        [Header("Scene Settings")]
        [Tooltip("Nombre EXACTO de la escena de juego tal como aparece en Build Settings.")]
        [SerializeField] private string gameSceneName = "SampleScene";

        // ─────────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Start()
        {
            // Arranca la música del menú cuando la escena carga
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMenuMusic();
            }
            else
            {
                Debug.LogWarning("[MenuController] SoundManager no encontrado. " +
                                 "Asegúrate de tener un GameObject con SoundManager en la escena.");
            }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        #region Public Methods (conectar desde el Inspector al ButtonPresenter.onPressed)

        /// <summary>
        /// Inicia la transición a la escena de juego.
        /// Conéctalo al evento onPressed del ButtonPresenter desde el Inspector.
        /// </summary>
        public void OnStartButtonPressed()
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("[MenuController] SceneLoader no encontrado. " +
                               "Añade el componente SceneLoader a un GameObject en la escena.");
                return;
            }

            Debug.Log($"[MenuController] Cargando escena: {gameSceneName}");
            SceneLoader.Instance.LoadScene(gameSceneName);
        }

        #endregion
    }
}
