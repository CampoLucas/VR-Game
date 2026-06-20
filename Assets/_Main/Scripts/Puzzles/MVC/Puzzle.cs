using System;
using UnityEngine;
using VRGame.DesignPatterns.Observers;
using VRGame.Puzzles.Interfaces;

namespace VRGame.Puzzles
{
    public class Puzzle : MonoBehaviour, IPuzzleController
    {
        public bool SolvedState
        {
            get => _puzzleSolved;
            protected set
            {
                if (_puzzleSolved == value) return;
                _puzzleSolved = value;
                if (_isEnabled) NotifyAll(id, SolvedState);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            private set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnEnabledSubject.NotifyAll(id, value);
                if (_isEnabled && _puzzleSolved) NotifyAll(id, SolvedState);
            }
        }

        public ISubject<int, bool> OnEnabledSubject { get; private set; } = new Subject<int, bool>();
        
        [Header("Puzzle Settings")]
        [SerializeField] private int id;

        [Header("Enable Subject")]
        [Tooltip("A puzzle that turns on and off this puzzle")]
        [SerializeField] private Puzzle other;

        private ISubject<int, bool> _onSolved = new Subject<int, bool>();
        private bool _puzzleSolved;
        private bool _isEnabled = true;

        protected virtual void Awake()
        {
            if (other)
            {
                // Subscribes itself to the subject, in this case the subject is the other puzzle
                other.Attach(this);
                Debug.Log($"Test: Set state to {other.SolvedState}");
                SetEnabled(other.SolvedState);
            }
        }

        public void SetEnabled(bool isEnabled)
        {
            Debug.Log($"Test: Set enabled {isEnabled}");
            IsEnabled = isEnabled;
        }
        
        public void OnNotify(int otherId, bool solved)
        {
            SetEnabled(solved);
        }

        public bool Attach(IObserver<int, bool> observer, bool disposeOnDetach = false)
        {
            return _onSolved?.Attach(observer, disposeOnDetach) ?? false;
        }

        public bool Detach(IObserver<int, bool> observer)
        {
            return _onSolved?.Detach(observer) ?? false;
        }

        public void DetachAll()
        {
            _onSolved?.DetachAll();
        }

        public void NotifyAll(int id, bool arg)
        {
            _onSolved?.NotifyAll(id, arg);
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _onSolved.Dispose();
            OnEnabledSubject.Dispose();
            _onSolved = null;
            OnEnabledSubject = null;
        }
    }
}