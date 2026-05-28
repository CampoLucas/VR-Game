using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VRGame.SceneManagement
{
    /// <summary>
    /// A Unity's Scene Management wrapper.
    /// </summary>
    public static class SceneHandler
    {
        public static event UnityAction<Scene, LoadSceneMode> SceneLoaded
        {
            add => SceneManager.sceneLoaded += value;
            remove => SceneManager.sceneLoaded -= value;
        }
        
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public static string GetActiveName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static int GetActiveIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public static int GetSceneCount()
        {
            return SceneManager.sceneCountInBuildSettings;
        }
        
        public static string GetSceneNameByIndex(int sceneIndex)
        {
            return System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
        }
    }
}