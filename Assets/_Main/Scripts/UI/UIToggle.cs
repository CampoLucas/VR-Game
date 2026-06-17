using UnityEngine;

namespace VRGame.UI
{
    public class UIToggle : MonoBehaviour
    {
        [SerializeField] private GameObject targetWindow;

        public void Toggle()
        {
            if (targetWindow != null)
            {
                targetWindow.SetActive(!targetWindow.activeSelf);
            }
        }
    }
}