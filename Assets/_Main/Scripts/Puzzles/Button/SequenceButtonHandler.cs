using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;
using VRGame.Puzzles.PhysicalButton.MVC;
using Random = UnityEngine.Random;

namespace VRGame.Puzzles.PhysicalButton
{
    public class SequenceButtonHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool regenerateOnFail = true;
        
        [Header("Highlight Colors")]
        [ColorUsage(false, true)] [SerializeField] private Color highlightColor = Color.yellow;
        [ColorUsage(false, true)] [SerializeField] private Color successColor = Color.green;
        [ColorUsage(false, true)] [SerializeField] private Color failColor = Color.red;
        [SerializeField] private float failFlashDuration = 0.5f;
        
        [Header("References")]
        [SerializeField] private PhysicalButtonController[] buttons;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onPuzzleComplete;
        [SerializeField] private UnityEvent onPuzzleFail;
        
        private int[] _sequence;
        private int _currentStep;
        private bool _isActive;
        private float _restartTimer = -1f;
        
        private readonly Dictionary<int, PhysicalButtonController> _buttonMap = new();

        private void Awake()
        {

        }

        private void Start()
        {
#if UNITY_EDITOR
            var ids = buttons.Select(b => b.Id).ToList();
            if (ids.Count != ids.Distinct().Count())
            {
                Debug.LogError("[PhysicalButtonPuzzle] Duplicate button IDs detected!", this);
            }
#endif

            foreach (var btn in buttons)
            {
                _buttonMap[btn.Id] = btn;
                btn.Subject.Attach(new ButtonObserver(this));
            }
            
            GenerateSequence();
            //StartPuzzle();
        }
        
        private void Update()
        {
            if (_restartTimer > 0f)
            {
                _restartTimer -= Time.deltaTime;
                if (_restartTimer <= 0f)
                {
                    _restartTimer = -1f;
                    StartPuzzle();
                }
            }
        }
        
        public void GenerateSequence()
        {
            var ids = buttons.Select(b => b.Id).ToArray();
            _sequence = new int[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                _sequence[i] = ids[Random.Range(0, ids.Length)];
            }

            Debug.Log($"[SequenceButtonHandler] Sequence: {string.Join(", ", _sequence)}");
        }
        
        [ContextMenu("Start Puzzle")]
        public void StartPuzzle()
        {
            _currentStep = 0;
            _isActive    = true;

            ResetAllButtons();
            HighlightCurrent();
        }
        
        public void OnButtonPressed(int id)
        {
            if (!_isActive) return;

            if (id == _sequence[_currentStep])
            {
                HandleCorrectPress();
            }
            else
            {
                HandleWrongPress(id);
            }
        }
        
        private void HandleCorrectPress()
        {
            GetView(_sequence[_currentStep])?.SetColor(successColor, 0.3f);

            _currentStep++;

            if (_currentStep >= _sequence.Length)
            {
                _isActive = false;
                onPuzzleComplete.Invoke();
                return;
            }

            HighlightCurrent();
        }

        private void HandleWrongPress(int pressedId)
        {
            _isActive = false;

            GetView(pressedId)?.SetColor(failColor, failFlashDuration);
            onPuzzleFail.Invoke();

            if (regenerateOnFail)
            {
                GenerateSequence();
            }

            _restartTimer = failFlashDuration + 0.1f;
        }

        private void HighlightCurrent()
        {
            GetView(_sequence[_currentStep])?.SetColor(highlightColor, .25f);
        }

        private void ResetAllButtons()
        {
            foreach (var btn in buttons)
            {
                btn.ResetButton();
            }
        }

        private PhysicalButtonView GetView(int id)
        {
            if (_buttonMap.TryGetValue(id, out var btn)) return btn.View;
            Debug.LogError($"[SequenceButtonHandler] No button found with id {id}", this);
            return null;
        }

        private sealed class ButtonObserver : VRGame.DesignPatterns.Observers.IObserver<int>
        {
            private readonly SequenceButtonHandler _puzzle;
            public ButtonObserver(SequenceButtonHandler puzzle) => _puzzle = puzzle;
            public void OnNotify(int id) => _puzzle.OnButtonPressed(id);
            public void Dispose() { }
        }
    }
}