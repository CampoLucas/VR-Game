using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;
using VRGame.General.Interactables;
using VRGame.Puzzles.Elements.Button;

namespace VRGame.Puzzles
{
    public class MemoryPuzzle : Puzzle
    {
        public bool ResolvedState { get; }
        
        [Header("Settings")]
        [SerializeField] private int sequenceLength = 4;
        [SerializeField] private float delayBetween = 1f;

        [Header("Buttons")]
        [SerializeField] private List<ButtonInteractable> buttons;
        [SerializeField] private ButtonInteractable onButton;

        [Header("Events")]
        [SerializeField] private UnityEvent onSuccess;
        [SerializeField] private UnityEvent onFailure;

        private int[] _sequence;
        private int _currentIndex;
        private bool _acceptingInput;
        
        private bool _playing;
        private int _playIndex;
        private float _timer;

        private readonly List<IObserver<int, bool>> _observers = new();

        protected sealed override void Awake()
        {
            base.Awake();
            foreach (var button in buttons)
            {
                var observer = new ActionObserver<int, bool>(OnButtonPressed);
                button.OnPressed.Attach(observer);
                _observers.Add(observer);
            }
        }
        
        private void Update()
        {
            if (!_playing) return;
 
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
 
            if (_playIndex >= _sequence.Length)
            {
                _playing = false;
                _acceptingInput = true;
                return;
            }
 
            FindButton(_sequence[_playIndex])?.Highlight();
            _playIndex++;
            _timer = delayBetween;
        }

        protected override void OnPuzzleDisabled()
        {
            SetDisableButtons(true);
            onButton.SetEnable(false);
        }

        protected override void OnPuzzleEnabled()
        {
            if (SolvedState) return;
            SetDisableButtons(false);
            onButton.SetEnable(true);
        }

        protected sealed override void OnDestroy()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].OnPressed.Detach(_observers[i]);
            }

            foreach (var observer in _observers)
            {
                observer.Dispose();
            }
 
            _observers.Clear();
            base.OnDestroy();
        }
        
        public void RegenerateSequence()
        {
            _sequence = new int[sequenceLength];

            for (var i = 0; i < sequenceLength; i++)
            {
                _sequence[i] = buttons[UnityEngine.Random.Range(0, buttons.Count)].Identifier;
            }
        }
        
        public void StartSequence(bool enableButtons)
        {
            if (_sequence == null)
            {
                RegenerateSequence();
            }

            if (enableButtons)
            {
                SetDisableButtons(false);
            }
 
            _acceptingInput = false;
            _currentIndex = 0;
            _playIndex = 0;
            _timer = delayBetween;
            _playing = true;
        }

        public void SetDisableButtons(bool value)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].SetEnable(!value);
            }
        }
        
        public void Reset()
        {
            _playing = false;
            _acceptingInput = false;
            _currentIndex = 0;
            _playIndex = 0;
            _timer = 0f;
        }
        
        private void OnButtonPressed(int id, bool pressed)
        {
            if (!_acceptingInput || !pressed) return;
 
            if (id != _sequence[_currentIndex])
            {
                _acceptingInput = false;
                onFailure.Invoke();
                SolvedState = false;
                return;
            }
 
            _currentIndex++;
 
            if (_currentIndex >= _sequence.Length)
            {
                _acceptingInput = false;
                SolvedState = true;
                onSuccess.Invoke();
            }
        }
        
        private ButtonInteractable FindButton(int id)
        {
            foreach (var button in buttons)
            {
                if (button.Identifier == id) return button;
            }
 
            return null;
        }
    }
}