using UnityEngine;

namespace VRGame.Puzzles.Elements
{
    public class SetEmission : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private float inDuration = 0.2f;
        [SerializeField] private float outDuration = 0.4f;
        [SerializeField] private Color colorA;
        [SerializeField] private Color colorB;
        [SerializeField] private Color colorC;
 
        private MaterialPropertyBlock _block;
        private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");
 
        private Color _emissionFrom;
        private Color _emissionTo;
        private float _emissionElapsed;
        private float _emissionDuration;
        private bool _emissionFadingOut;
        private bool _emissionActive;
 
        private void Awake()
        {
            _block = new MaterialPropertyBlock();
        }
 
        private void Update()
        {
            if (!_emissionActive) return;
 
            _emissionElapsed += Time.deltaTime;
            var t = Mathf.Clamp01(_emissionElapsed / _emissionDuration);
            var smooth = t * t * (3f - 2f * t);
 
            targetRenderer.GetPropertyBlock(_block);
            _block.SetColor(EmissionID, Color.Lerp(_emissionFrom, _emissionTo, smooth));
            targetRenderer.SetPropertyBlock(_block);
 
            if (t < 1f) return;
 
            if (!_emissionFadingOut)
            {
                _emissionFrom = _emissionTo;
                _emissionTo = Color.black;
                _emissionElapsed = 0f;
                _emissionDuration = outDuration;
                _emissionFadingOut = true;
            }
            else
            {
                _emissionActive = false;
            }
        }
 
        public void SetEmissionColor(Color color)
        {
            targetRenderer.GetPropertyBlock(_block);
            _emissionFrom = _block.GetColor(EmissionID);
            _emissionTo = color;
            _emissionElapsed = 0f;
            _emissionDuration = inDuration;
            _emissionFadingOut = false;
            _emissionActive = true;
        }
        
        public void SetEmissionColorA()
        {
            SetEmissionColor(colorA);
        }
        
        public void SetEmissionColorB()
        {
            SetEmissionColor(colorB);
        }
        
        public void SetEmissionColorC()
        {
            SetEmissionColor(colorC);
        }
    }
}