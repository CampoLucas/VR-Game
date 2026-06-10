using System;
using UnityEngine;
using VRGame.DesignPatterns.Observers;

namespace VRGame.General.Interactables
{
    public class ButtonView : MonoBehaviour
    {
        [Header("Interactable")]
        [SerializeField] private ButtonInteractable interactable;

        [Header("Renderer")]
        [SerializeField] private Renderer buttonRenderer;
        
        [Header("Movement")]
        [SerializeField] private Transform target;
        [SerializeField] private float travelDistance = 0.008f;
        
        [Header("Colors")]
        [SerializeField] private Color restColor = Color.white;
        [SerializeField] private Color pressedColor = Color.gray;
        [SerializeField] private Color disabledColor = Color.red;
        [SerializeField] private Color enabledEmissionColor = Color.black;

        [Header("Disabled")]
        [SerializeField, Range(0, 1)] private float disabledDepth = .5f;

        [Header("Speed")]
        [SerializeField] private float transitionSpeed = 12f;
        [SerializeField] private float emissionSpeed = 8f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
 
        private MaterialPropertyBlock _mpb;
        private DesignPatterns.Observers.IObserver<float> _valueObserver;
        private DesignPatterns.Observers.IObserver<bool> _enableObserver;
        private DesignPatterns.Observers.IObserver _highlightObserver;
        
        private Vector3 _restLocalPos;
        private float _currentVisualDepth;
        private bool _isDisabled;
        
        private Color _highlightColor;
        private float _highlightT;
        private bool _highlightIn;
        private bool _highlightActive;
 
        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            _restLocalPos = target.localPosition;
            ApplyBaseColor(restColor);
        }
 
        private void OnEnable()
        {
            if (_valueObserver == null)
            {
                _valueObserver = new ActionObserver<float>(OnValueChanged);
            }

            if (_enableObserver == null)
            {
                _enableObserver = new ActionObserver<bool>(OnEnableChanged);
            }

            if (_highlightObserver == null)
            {
                _highlightObserver = new ActionObserver(Highlight);
            }
            
            interactable.OnValueChanged.Attach(_valueObserver);
            interactable.OnEnabled.Attach(_enableObserver);
            interactable.OnHighlight.Attach(_highlightObserver);
        }
 
        private void OnDisable()
        {
            interactable.OnValueChanged.Detach(_valueObserver);
            interactable.OnEnabled.Detach(_enableObserver);
            interactable.OnHighlight.Attach(_highlightObserver);
        }

        private void Update()
        {
            if (_isDisabled)
            {
                _currentVisualDepth = Mathf.Lerp(_currentVisualDepth, disabledDepth, Time.deltaTime * transitionSpeed);
                target.localPosition = _restLocalPos + Vector3.down * (_currentVisualDepth * travelDistance);
            }

            if (_highlightActive)
            {
                _highlightT = Mathf.MoveTowards(_highlightT, _highlightIn ? 1f : 0f, Time.deltaTime * emissionSpeed);

                if (_highlightIn && _highlightT >= 1f)
                {
                    _highlightIn = false;
                }
                else if (!_highlightIn && _highlightT <= 0f)
                {
                    _highlightActive = false;
                }
 
                ApplyEmission(Color.Lerp(Color.black, _highlightColor, _highlightT));
            }
            
            
        }

        [ContextMenu("Do Highlight")]
        public void Highlight()
        {
            Highlight(enabledEmissionColor);
        }

        public void Highlight(Color color)
        {
            _highlightColor = color;
            _highlightT = 0f;
            _highlightIn = true;
            _highlightActive = true;
        }

        private void OnValueChanged(float depth)
        {
            if (_isDisabled) return;
            _currentVisualDepth = depth;
            target.localPosition = _restLocalPos + Vector3.down * (depth * travelDistance);
            ApplyBaseColor(Color.Lerp(restColor, pressedColor, depth));
        }
        
        private void OnEnableChanged(bool enabled)
        {
            _isDisabled = !enabled;
 
            if (!enabled)
            {
                ApplyBaseColor(disabledColor);
            }
            else
            {
                _currentVisualDepth = interactable.CurrentDepth;
                target.localPosition = _restLocalPos + Vector3.down * (_currentVisualDepth * travelDistance);
                ApplyBaseColor(Color.Lerp(restColor, pressedColor, _currentVisualDepth));
                Highlight(enabledEmissionColor);
            }
        }
        
        private void ApplyBaseColor(Color color)
        {
            buttonRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(BaseColorId, color);
            buttonRenderer.SetPropertyBlock(_mpb);
        }
 
        private void ApplyEmission(Color color)
        {
            buttonRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(EmissionColorId, color);
            buttonRenderer.SetPropertyBlock(_mpb);
        }
    }
}