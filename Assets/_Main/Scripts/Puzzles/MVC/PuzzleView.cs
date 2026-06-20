using System;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles
{
    public enum PuzzleVisualState
    {
        Disabled,
        Normal,
        Solved
    }
    
    public class PuzzleView : MonoBehaviour, IObserver<int, bool>
    {
        public PuzzleVisualState CurrentState { get; private set; }
 
        [SerializeField] private Puzzle puzzle;
 
        [Header("Events")]
        [SerializeField] private UnityEvent onDisabled;
        [SerializeField] private UnityEvent onNormal;
        [SerializeField] private UnityEvent onSolved;

        private void Awake()
        {
            if (!puzzle)
            {
                puzzle = GetComponent<Puzzle>();
            }
        }

        private void OnValidate()
        {
            if (!puzzle)
            {
                Debug.LogWarning($"[{nameof(PuzzleView)}] WARNING: The puzzle reference is null, it will use GetComponent in the awake");
            }
        }

        private void Start()
        {
            puzzle.Attach(this);
            ApplyState(GetState(), true);
        }
 
        public void OnNotify(int id, bool isResolved)
        {
            ApplyState(GetState());
        }
 
        public void Dispose()
        {
            puzzle = null;
        }
 
        private PuzzleVisualState GetState()
        {
            if (!puzzle.IsEnabled) return PuzzleVisualState.Disabled;
            return puzzle.SolvedState ? PuzzleVisualState.Solved : PuzzleVisualState.Normal;
        }
 
        private void ApplyState(PuzzleVisualState state, bool force = false)
        {
            if (!force && state == CurrentState) return;
            CurrentState = state;
 
            switch (state)
            {
                case PuzzleVisualState.Disabled:
                    onDisabled.Invoke();
                    break;
                case PuzzleVisualState.Normal:
                    onNormal.Invoke();
                    break;
                case PuzzleVisualState.Solved:
                    onSolved.Invoke();
                    break;
            }
        }
 
        private void OnDestroy()
        {
            puzzle.Detach(this);
        }
    }
}