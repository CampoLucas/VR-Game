using System;
using UnityEngine;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Elements.Button
{
    public class ButtonView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private ButtonPresenter presenter;
        
        [Header("State Colors")]
        [FormerlySerializedAs("enabledColor")]
        [SerializeField, ColorUsage(false, false)] private Color normalColor  = Color.red;
        [SerializeField, ColorUsage(false, false)] private Color hoverColor    = Color.yellow;
        [SerializeField, ColorUsage(false, false)] private Color selectColor = Color.cyan;
        [SerializeField, ColorUsage(false, false)] private Color disabledColor = Color.gray;
 
        [Header("Color Transition")]
        [SerializeField] private float colorDuration = 0.2f;
        
        [Header("Highlight")]
        [SerializeField, ColorUsage(false, true)] private Color highlightColor = Color.white;
        [SerializeField] private float highlightInDuration  = 0.2f;
        [SerializeField] private float highlightOutDuration = 0.4f;
        
        private MaterialPropertyBlock _block;
        private static readonly int ColorID = Shader.PropertyToID("_Color");
        private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");
        //private static readonly string EmissionKeyword = "_EMISSION";
        
        // Color lerp
        private Color _colorFrom;
        private Color _colorTo;
        private float _colorElapsed;
        private bool _colorLerping;
        
        // Emission lerp
        private Color _emissionFrom;
        private Color _emissionTo;
        private float _emissionElapsed;
        private float _emissionDuration;
        private bool _emissionLerping;
        private bool _highlightFadingOut;
        
        private ActionObserver<ButtonState> _stateObserver;
        private ActionObserver _highlightObserver;

        private void Awake()
        {
            _block = new MaterialPropertyBlock();
            
            targetRenderer.GetPropertyBlock(_block);
            _block.SetColor(ColorID, normalColor);
            targetRenderer.SetPropertyBlock(_block);
 
            _stateObserver = new ActionObserver<ButtonState>(OnStateChanged);
            _highlightObserver = new ActionObserver(OnHighlight);
            
            presenter.StateSubject.Attach(_stateObserver);
            presenter.HighlightSubject.Attach(_highlightObserver);
        }
        
        private void Update()
        {
            var delta = Time.deltaTime;
            
            TickColor(delta);
            TickEmission(delta);
        }
        
        private void OnDestroy()
        {
            _stateObserver?.Dispose();
            _highlightObserver?.Dispose();
        }
        
        private Color StateToColor(ButtonState state) => state switch
        {
            ButtonState.Normal  => normalColor,
            ButtonState.Hover => hoverColor,
            ButtonState.Select => selectColor,
            ButtonState.Disabled => disabledColor,
            _ => normalColor
        };
        
        private void OnStateChanged(ButtonState state)
        {
            targetRenderer.GetPropertyBlock(_block);
            _colorFrom = _block.GetColor(ColorID);
            _colorTo = StateToColor(state);
            _colorElapsed = 0f;
            _colorLerping = true;
        }
        
        private void OnHighlight()
        {
            targetRenderer.GetPropertyBlock(_block);
            _emissionFrom = _block.GetColor(EmissionID);
            _emissionTo = highlightColor;
            _emissionElapsed = 0f;
            _emissionDuration = highlightInDuration;
            _highlightFadingOut = false;
            _emissionLerping = true;
            
            //targetRenderer.material.EnableKeyword(EmissionKeyword);
        }
        
        private void TickColor(float delta)
        {
            if (!_colorLerping) return;
 
            _colorElapsed += delta;
            var t = Mathf.Clamp01(_colorElapsed / colorDuration);
            var smooth = t * t * (3f - 2f * t);
 
            targetRenderer.GetPropertyBlock(_block);
            _block.SetColor(ColorID, Color.Lerp(_colorFrom, _colorTo, smooth));
            targetRenderer.SetPropertyBlock(_block);
 
            if (t >= 1f) _colorLerping = false;
        }
        
        private void TickEmission(float delta)
        {
            if (!_emissionLerping) return;
 
            _emissionElapsed += delta;
            var t = Mathf.Clamp01(_emissionElapsed / _emissionDuration);
            var smooth = t * t * (3f - 2f * t);
 
            targetRenderer.GetPropertyBlock(_block);
            _block.SetColor(EmissionID, Color.Lerp(_emissionFrom, _emissionTo, smooth));
            targetRenderer.SetPropertyBlock(_block);
 
            if (t < 1f) return;
 
            if (!_highlightFadingOut)
            {
                _emissionFrom = highlightColor;
                _emissionTo = Color.black;
                _emissionElapsed = 0f;
                _emissionDuration = highlightOutDuration;
                _highlightFadingOut = true;
            }
            else
            {
                _emissionLerping = false;
                //targetRenderer.material.DisableKeyword("_EMISSION");
            }
        }

        
    }
}