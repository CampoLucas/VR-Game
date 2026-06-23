using System;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;

namespace VRGame.General.Interactables
{
    public class ButtonInteractable : OverlapInteractable<ButtonInteractor, ButtonInteractable>
    {
        public ISubject<int ,bool> OnPressed { get; private set; } = new Subject<int, bool>();
        public ISubject<float> OnValueChanged { get; private set; } = new Subject<float>();
        public ISubject<bool> OnEnabled { get; private set; } = new Subject<bool>();
        public ISubject OnHighlight { get; private set; } = new Subject();
        
        public float CurrentDepth { get; private set; }
        public bool IsEnabled { get; private set; } = true;
 
        [Header("Settings")]
        [SerializeField] private float returnSpeed = 12f;
        [SerializeField] private int maxPresses = -1;
        [SerializeField] private float updateInterval = .15f;

        [Header("Unity Events")]
        [SerializeField] private UnityEvent onPressed;
        [SerializeField] private UnityEvent onReleased;
        
        private bool _pressed;
        private int _pressCount;
        private UpdateDelay _updateDelay;
 
        protected override void Awake()
        {
            base.Awake();
            _updateDelay = new UpdateDelay(updateInterval, UpdateDepth);
        }
 
        private void Update()
        {
            _updateDelay.Run();
        }

        [ContextMenu("Toggle Enabled")]
        public void ToggleEnabled()
        {
            SetEnable(!IsEnabled);
        }

        public void SetEnable(bool enable)
        {
            if (IsEnabled == enable) return;
            IsEnabled = enable;

            if (enable)
            {
                _pressCount = 0;
            }
            else if (_pressed)
            {
                _pressed = false;
                OnPressed.NotifyAll(Identifier, false);
                onReleased.Invoke();
            }
            
            OnEnabled.NotifyAll(enable);
        }

        private void UpdateDepth(float delta)
        {
            var goal = _pressed ? 1f : 0f;
            var depth = Mathf.Lerp(CurrentDepth, goal, delta * returnSpeed);
            if (Mathf.Approximately(depth, CurrentDepth)) return;
            
            CurrentDepth = depth;
            OnValueChanged.NotifyAll(CurrentDepth);
        }
 
        protected override void InteractorAdded(ButtonInteractor interactor) { }
        protected override void InteractorRemoved(ButtonInteractor interactor) { }
 
        protected override void SelectingInteractorAdded(ButtonInteractor interactor) 
        {
            if (!IsEnabled) return;
            
            _pressed = true;
            _pressCount++;
            OnPressed.NotifyAll(Identifier, true);
            onPressed.Invoke();

            if (maxPresses >= 0 && _pressCount >= maxPresses)
            {
                SetEnable(false);
            }
        }
        
        protected override void SelectingInteractorRemoved(ButtonInteractor interactor)
        { 
            if (!_pressed) return;
            _pressed = false;
            OnPressed.NotifyAll(Identifier, false);
            onReleased.Invoke();
        }

        private void OnDestroy()
        {
            OnPressed?.Dispose();
            OnValueChanged?.Dispose();
            OnEnabled?.Dispose();
            OnHighlight?.Dispose();

            onPressed?.RemoveAllListeners();
            onReleased?.RemoveAllListeners();
            
            _updateDelay?.Dispose();

                
            OnPressed = null;
            OnValueChanged = null;
            OnEnabled = null;
            OnHighlight = null;
            
            onPressed = null;
            onReleased = null;

            _updateDelay = null;
        }

        public void Highlight()
        {
            OnHighlight.NotifyAll();
        }
    }
}