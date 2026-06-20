using UnityEngine;

namespace VRGame.Player
{
    public class PlayerHeightAdjuster : MonoBehaviour
    {
        private const string HEIGHT_PREF_KEY = "PlayerHeightOffset";

        [Tooltip("The parent transform of the Camera Rig that will be moved up or down.")]
        [SerializeField] private Transform rigTransform;
        
        [Tooltip("Height offset to apply when playing seated (e.g., 0.5 to 0.8 meters).")]
        [SerializeField] private float seatedHeightOffset = 0.6f;

        private float currentOffset = 0f;
        private Vector3 basePosition;
        private bool basePositionCaptured = false;

        [Header("UI (Optional)")]
        [SerializeField] private UnityEngine.UI.Slider heightSlider;

        private void Start()
        {
            Initialize();
            LoadAndApplyHeight();

            if (heightSlider != null)
            {
                heightSlider.value = currentOffset;
                heightSlider.onValueChanged.AddListener(OnSliderHeightChanged);
            }
        }

        private void Initialize()
        {
            if (basePositionCaptured) return;

            if (rigTransform == null)
            {
                rigTransform = this.transform;
            }
            
            basePosition = rigTransform.position;
            basePositionCaptured = true;
        }

        private void LoadAndApplyHeight()
        {
            float savedOffset = PlayerPrefs.GetFloat(HEIGHT_PREF_KEY, 0f);
            ApplyHeightOffset(savedOffset);
        }

        public void ApplyHeightOffset(float offset)
        {
            if (!basePositionCaptured) Initialize();
            if (rigTransform == null) return;

            currentOffset = offset;
            rigTransform.position = new Vector3(basePosition.x, basePosition.y + currentOffset, basePosition.z);
            
            PlayerPrefs.SetFloat(HEIGHT_PREF_KEY, currentOffset);
            PlayerPrefs.Save();
            
            Debug.Log($"[PlayerHeightAdjuster] Height adjusted to: {currentOffset}m. Y Position: {rigTransform.position.y}");
        }

        /// <summary>
        /// Method to link directly with a Slider (OnValueChanged).
        /// </summary>
        public void OnSliderHeightChanged(float value)
        {
            ApplyHeightOffset(value);
        }

        public void SetSeatedMode(bool isSeated)
        {
            ApplyHeightOffset(isSeated ? seatedHeightOffset : 0f);
        }

        public void IncreaseHeight(float amount)
        {
            ApplyHeightOffset(currentOffset + amount);
        }

        public void DecreaseHeight(float amount)
        {
            IncreaseHeight(-amount);
        }

        public float GetSavedHeight()
        {
            return PlayerPrefs.GetFloat(HEIGHT_PREF_KEY, 0f);
        }

        // Parameterless methods for easy linking to UI buttons
        public void HeightUp()
        {
            IncreaseHeight(0.1f); // Increases by 10 cm
        }

        public void HeightDown()
        {
            IncreaseHeight(-0.1f); // Decreases by 10 cm
        }
    }
}
