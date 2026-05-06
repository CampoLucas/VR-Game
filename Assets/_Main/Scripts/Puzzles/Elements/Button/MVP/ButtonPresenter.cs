using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Elements.Button
{
    /// <summary>
    /// The presenter for the button.
    /// Translates the poke interactable events to the model state machine.
    /// </summary>
    public class ButtonPresenter : MonoBehaviour
    {
        #region Properties

        public int Id => _model.Id;
        public ButtonState State => _stateMachine.Current;
        public bool IsPressed => _stateMachine.Current == ButtonState.Select;
        public bool IsActive => _stateMachine.Current != ButtonState.Disabled;

        public ISubject<ButtonState> StateSubject { get; } = new Subject<ButtonState>();
        public ISubject<int> PressedSubject { get; } = new Subject<int>();
        public ISubject HighlightSubject { get; } = new Subject();

        #endregion

        #region Serilizable Variables

        [Header("Settings")]
        [SerializeField] private int id;
        [Tooltip("If enabled, the button is disabled after its first press.")]
        [SerializeField] private bool once;
        [SerializeField] private bool activeOnStart = true;
        
        [Header("References")]
        [SerializeField] private PokeInteractable pokeInteractable;
        [SerializeField] private Transform buttonVisual;
        [SerializeField] private Transform surface;

        [Header("Event")]
        [SerializeField] private UnityEvent onPressed;
        [SerializeField] private UnityEvent onReleased;

        #endregion

        #region Private Variables

        private readonly ButtonModel _model = new();
        private readonly ButtonStateMachine _stateMachine = new();
        
        private Vector3 _restLocalPos;
        private ActionObserver<ButtonState> _stateChangedObserver;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            Setup(id);
            _stateChangedObserver = new ActionObserver<ButtonState>(OnStateChanged);
            _stateMachine.OnStateChanged.Attach(_stateChangedObserver);
            _restLocalPos = buttonVisual.localPosition;
        }

        private void Start()
        {
            pokeInteractable.WhenStateChanged += OnPokeStateChanged;

            if (!activeOnStart)
            {
                SetDisabled(true);
            }
        }
        
        private void OnDestroy()
        {
            onPressed.RemoveAllListeners();
            onReleased.RemoveAllListeners();
            
            StateSubject.Dispose();
            PressedSubject.Dispose();
            HighlightSubject.Dispose();
            
            pokeInteractable.WhenStateChanged -= OnPokeStateChanged;
            _stateMachine.OnStateChanged.Detach(_stateChangedObserver);
            _model.Dispose();
        }

        #endregion
        

        #region Public Methods

        [ContextMenu("Press")]
        private void Press()
        {
            _stateMachine.TransitionTo(ButtonState.Select);
        }

        public void SetDisabled(bool disabled)
        {
            //_stateMachine.TransitionTo(disabled ? ButtonState.Disabled : ButtonState.Normal);

            if (disabled)
            {
                SetAsDisabled();
                _stateMachine.TransitionTo(ButtonState.Disabled);
            }
            else
            {
                ResetButton();
            }
        }

        [ContextMenu("Toggle Disabled")]
        public void ToggleDisabled()
        {
            SetDisabled(IsActive);
        }
        
        [ContextMenu("Reset Button")]
        public void ResetButton()
        {
            buttonVisual.localPosition = _restLocalPos;
            pokeInteractable.enabled = true;
            _stateMachine.TransitionTo(ButtonState.Normal);
        }

        [ContextMenu("Highlight Button")]
        public void Highlight()
        {
            HighlightSubject.NotifyAll();
        }

        public void Setup(int newId)
        {
            id = newId;
            _model.Setup(newId, once);
        }

        #endregion

        #region Private Methods

        private void OnPokeStateChanged(InteractableStateChangeArgs args)
        {
            switch (args.NewState)
            {
                case InteractableState.Normal:
                    _stateMachine.TransitionTo(ButtonState.Normal);
                    break;
 
                case InteractableState.Hover:
                    _stateMachine.TransitionTo(ButtonState.Hover);
                    break;
 
                case InteractableState.Select:
                    _stateMachine.TransitionTo(ButtonState.Select);
                    break;
            }

            if (args.PreviousState == InteractableState.Select)
            {
                onReleased.Invoke();
            }
        }
        
        private void OnStateChanged(ButtonState state)
        {
            if (state != ButtonState.Select)
            {
                StateSubject.NotifyAll(state);
                return;
            }
 
            buttonVisual.localPosition = surface.localPosition;
            PressedSubject.NotifyAll(_model.Id);
            onPressed.Invoke();

            if (_model.IsOnce)
            {
                
                SetAsDisabled();
            }
            else
            {
                StateSubject.NotifyAll(ButtonState.Select);
            }
            
        }

        private void SetAsDisabled()
        {
            Debug.Log("ButtonTest: Set as disabled");
            pokeInteractable.enabled = false;
            buttonVisual.position = surface.position;
        }
        
        

        #endregion
    }
}