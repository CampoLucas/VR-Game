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
        [SerializeField] private Renderer targetRenderer;

        [Header("Settings")]
        [SerializeField] private bool flipConnectors = false;
 
        [Header("Events")]
        [SerializeField] private UnityEvent onDisabled;
        [SerializeField] private UnityEvent onNormal;
        [SerializeField] private UnityEvent onSolved;

        private static readonly int EnabledPropertyId = Shader.PropertyToID("_Enabled");
        private static readonly int OnPropertyId = Shader.PropertyToID("_On");
        private static readonly int FlipConnectorId = Shader.PropertyToID("_Flip");

        private MaterialPropertyBlock _propertyBlock;
        private MaterialPropertyBlock _flipConnectorBlock;
        
        private void Awake()
        {
            if (!puzzle)
            {
                puzzle = GetComponent<Puzzle>();
            }
            
            _propertyBlock = new MaterialPropertyBlock();
            _flipConnectorBlock = new MaterialPropertyBlock();
        }
        
        private void Start()
        {
            puzzle.Attach(this);
            puzzle.OnEnabledSubject.Attach(this);
            ApplyState(GetState(), true);

            if (targetRenderer)
            {
                targetRenderer.GetPropertyBlock(_flipConnectorBlock);
                _flipConnectorBlock.SetFloat(FlipConnectorId, flipConnectors ? 1 : 0);
                targetRenderer.SetPropertyBlock(_flipConnectorBlock);
            }
        }
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!puzzle)
            {
                Debug.LogWarning($"[{nameof(PuzzleView)}] WARNING: The puzzle reference is null, it will use GetComponent in the awake");
            }
#endif
        }
 
        public void OnNotify(int id, bool state)
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
 
            UpdateMaterialProperties(state);
            
            switch (state)
            {
                case PuzzleVisualState.Disabled:
                    OnPuzzleDisabled();
                    break;
                case PuzzleVisualState.Normal:
                    onNormal.Invoke();
                    break;
                case PuzzleVisualState.Solved:
                    onSolved.Invoke();
                    break;
            }
        }

        private void OnPuzzleDisabled()
        {
            onDisabled.Invoke();
        }
        
        private void OnPuzzleEnabled()
        {
            onNormal.Invoke();
        }
        
        private void OnPuzzleSolved()
        {
            onSolved.Invoke();
        }
        
        private void UpdateMaterialProperties(PuzzleVisualState state)
        {
            if (!targetRenderer) return;
 
            targetRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(EnabledPropertyId, state == PuzzleVisualState.Disabled ? 0f : 1f);
            _propertyBlock.SetFloat(OnPropertyId, state == PuzzleVisualState.Solved ? 1f : 0f);
            targetRenderer.SetPropertyBlock(_propertyBlock);
        }
 
        private void OnDestroy()
        {
            puzzle.Detach(this);
            puzzle.OnEnabledSubject?.Detach(this);
        }
    }
}