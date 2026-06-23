using System;
using UnityEngine;

namespace VRGame.Puzzles.PhysicalButton.MVC
{
    public class PhysicalButtonView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer  renderer;
        
        [Header("Colors")]
        [SerializeField, ColorUsage(false, true)] private Color baseColor = Color.red;
        [SerializeField, ColorUsage(false, true)] private Color pressedColor = Color.green;
        [SerializeField, ColorUsage(false, true)] private Color releasedColor = Color.cyan;
        
        private Color  _currentColor;
        private Color  _targetColor;
        private Color  _lerpFromColor;
        private float  _lerpDuration;
        private float  _lerpElapsed;
        private bool   _isLerping;
        
        private Material  _material;

        private void Awake()
        {
            _material = renderer.material;
            SetColor(baseColor);
        }

        private void Update()
        {
            Run(Time.deltaTime);
        }

        public void SetColor(Color color)
        {
            _isLerping = false;
            _currentColor = color;
            _material.color = color;
        }

        public void SetColor(Color color, float duration)
        {
            SetColor(color, _currentColor, duration);
        }

        public void SetColor(Color colorA, Color colorB, float duration)
        {
            _lerpFromColor = colorA;
            _targetColor = colorB;
            _lerpDuration = duration;
            _lerpElapsed = 0f;
            _isLerping = true;
            _material.color = colorA;
        }

        public void SetToBase() => SetColor(baseColor);
        public void SetToPressed() => SetColor(_currentColor, pressedColor, .25f);
        public void SetToReleased() => SetColor(releasedColor, baseColor, .25f);

        private void Run(float delta)
        {
            if (!_isLerping) return;

            _lerpElapsed += delta;
            var t = Mathf.Clamp01(_lerpElapsed / _lerpDuration);

            var smooth = t * t * (3f - 2f * t);
            _currentColor = Color.Lerp(_lerpFromColor, _targetColor, smooth);
            _material.color = _currentColor;

            if (t >= 1f) _isLerping = false;
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }
    }
}