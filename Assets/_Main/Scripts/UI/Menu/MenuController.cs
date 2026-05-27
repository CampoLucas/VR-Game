using UnityEngine;
using VRGame.Level;

namespace VRGame.UI.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string gameSceneName = "SampleScene";

        #region Public Methods

        public void OnStartButtonPressed()
        {
            var manager = LevelManager.Instance;
            
            if (!manager)
            {
                Debug.LogError("[MenuController] SceneLoader no encontrado. " +
                               "Añade el componente SceneLoader a un GameObject en la escena.");
                return;
            }

            if (manager.IsTransitioning())
            {
                return;
            }

            Debug.Log($"[MenuController] Cargando escena: {gameSceneName}");
            manager.TransitionToScene(gameSceneName);
            //SceneLoader.Instance.LoadScene(gameSceneName);
        }

        #endregion
    }
}
