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
                Debug.LogError("[MenuController] LevelManager not found. " +
                               "Add the LevelManager component to a GameObject in the scene.");
                return;
            }

            if (manager.IsTransitioning())
            {
                return;
            }

            Debug.Log($"[MenuController] Loading scene: {gameSceneName}");
            manager.TransitionToScene(gameSceneName);
        }

        public void OnQuitButtonPressed()
        {
            Debug.Log("[MenuController] Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}
