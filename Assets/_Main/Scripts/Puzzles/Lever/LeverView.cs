using UnityEngine;

namespace VRGame.Puzzles.Lever
{
    [RequireComponent(typeof(LeverPuzzle))]
    public class LeverView : PuzzleView
    {
        private static readonly int LeverMappingPropertyId = Shader.PropertyToID("_LeverMapping");
        private MaterialPropertyBlock _leverMappingBlock;

        private LeverPuzzle _leverPuzzle;
        
        protected override void Awake()
        {
            base.Awake();

            if (puzzle is not LeverPuzzle l)
            {
                l = GetComponent<LeverPuzzle>();
            }

            _leverPuzzle = l;

            if (targetRenderer)
            {
                _leverMappingBlock = new MaterialPropertyBlock();
            }
        }

        protected override void Start()
        {
            base.Start();

            if (!targetRenderer) return;

            targetRenderer.GetPropertyBlock(_leverMappingBlock, 0);
            _leverMappingBlock.SetVector(LeverMappingPropertyId, _leverPuzzle.LeverMapping);
            targetRenderer.SetPropertyBlock(_leverMappingBlock, 0);
        }

        protected override void OnDestroy()
        {
            _leverPuzzle = null;
            base.OnDestroy();
        }
    }
}