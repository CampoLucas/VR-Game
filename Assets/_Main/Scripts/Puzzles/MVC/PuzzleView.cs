using System;
using UnityEngine;
using UnityEngine.Events;
using VRGame.DesignPatterns.Observers;
using VRGame.Puzzles.Wires;

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
 
        [SerializeField] protected Puzzle puzzle;
        [SerializeField] protected Renderer targetRenderer;

        [Header("Settings")]
        [SerializeField] private bool flipConnectors = false;

        [Header("Wire")]
        [SerializeField] private WireMatRef[] wires;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onDisabled;
        [SerializeField] private UnityEvent onNormal;
        [SerializeField] private UnityEvent onSolved;

        private static readonly int EnabledPropertyId = Shader.PropertyToID("_Enabled");
        private static readonly int SolvedPropertyId = Shader.PropertyToID("_Solved");
        private static readonly int InvertPropertyId = Shader.PropertyToID("_Invert");
        
        
        private static readonly int WireOnPropertyId = Shader.PropertyToID("_IsOn");

        private MaterialPropertyBlock _propertyBlock;
        private MaterialPropertyBlock _flipConnectorBlock;
        private MaterialPropertyBlock _wireIsOnBlock;
        
        protected virtual void Awake()
        {
            if (!puzzle)
            {
                puzzle = GetComponent<Puzzle>();
            }
            

            if (targetRenderer)
            {
                _propertyBlock = new MaterialPropertyBlock();
                _flipConnectorBlock = new MaterialPropertyBlock();
            }

            if (wires.Length > 0)
            {
                _wireIsOnBlock = new MaterialPropertyBlock();
            }
        }
        
        protected virtual void Start()
        {
            puzzle.Attach(this);
            puzzle.OnEnabledSubject.Attach(this);
            ApplyState(GetState(), true);

            if (targetRenderer)
            {
                targetRenderer.GetPropertyBlock(_flipConnectorBlock, 0);
                _flipConnectorBlock.SetFloat(InvertPropertyId, flipConnectors ? 1 : 0);
                targetRenderer.SetPropertyBlock(_flipConnectorBlock, 0);
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
                    OnPuzzleEnabled();
                    break;
                case PuzzleVisualState.Solved:
                    OnPuzzleSolved();
                    break;
            }
        }

        private void OnPuzzleDisabled()
        {
            Wire(false);
            onDisabled.Invoke();
        }
        
        private void OnPuzzleEnabled()
        {
            Wire(false);
            onNormal.Invoke();
        }
        
        private void OnPuzzleSolved()
        {
            Wire(true);
            onSolved.Invoke();
        }

        private void Wire(bool state)
        {
            if (wires.Length == 0) return;

            for (var i = 0; i < wires.Length; i++)
            {
                var w = wires[i];
                if (w == null || !w.Renderer)
                {
#if UNITY_EDITOR
                    Debug.LogError($"ERROR: null reference for the wire at index {i}", this);    
#endif
                    return;
                }
                
                w.Renderer.GetPropertyBlock(_wireIsOnBlock, w.MaterialIndex);
                _wireIsOnBlock.SetFloat(WireOnPropertyId, state ? 1 : 0);
                w.Renderer.SetPropertyBlock(_wireIsOnBlock, w.MaterialIndex);
            }
            
            
            
            //wireRenderer.materials[materialIndex] = state ? OnMaterial : OffMaterial;
        }
        
        private void UpdateMaterialProperties(PuzzleVisualState state)
        {
            if (!targetRenderer) return;
 
            targetRenderer.GetPropertyBlock(_propertyBlock, 0);
            _propertyBlock.SetFloat(EnabledPropertyId, state == PuzzleVisualState.Disabled ? 0f : 1f);
            _propertyBlock.SetFloat(SolvedPropertyId, state == PuzzleVisualState.Solved ? 1f : 0f);
            targetRenderer.SetPropertyBlock(_propertyBlock, 0);
        }
 
        protected virtual void OnDestroy()
        {
            puzzle.Detach(this);
            puzzle.OnEnabledSubject?.Detach(this);
        }
    }
}