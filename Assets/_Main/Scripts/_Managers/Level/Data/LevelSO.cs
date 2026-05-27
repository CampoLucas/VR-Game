using UnityEngine;

namespace VRGame.Level.Data
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "VRGame/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public float TimerDuration => timerDuration;
        
        [Header("Timer")]
        [SerializeField] private float timerDuration = 300f;

        [Header("Par Times")]
        [SerializeField] private float parS = 120f;
        [SerializeField] private float parA = 180f;
        [SerializeField] private float parB = 240f;
        [SerializeField] private float parC = 300f;
        
        public LevelScore EvaluateScore(float elapsed)
        {
            if (elapsed <= parS) return LevelScore.S;
            if (elapsed <= parA) return LevelScore.A;
            if (elapsed <= parB) return LevelScore.B;
            if (elapsed <= parC) return LevelScore.C;
            return LevelScore.F;
        }
        
    }
}