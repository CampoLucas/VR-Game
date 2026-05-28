using VRGame.Level.Data;
using VRGame.SceneManagement;

namespace VRGame.Level
{
    public class LevelResult
    {
        public string SceneName { get; }
        public float Elapsed { get; }
        public LevelScore Score { get; }

        public LevelResult()
        {
            
        }
        
        public LevelResult(string sceneName, float elapsed, LevelScore score)
        {
            SceneName = sceneName;
            Elapsed = elapsed;
            Score = score;
        }
        
        public static LevelResult RecordResult(LevelSO level, float elapsed)
        {
            var score = level != null ? level.EvaluateScore(elapsed) : LevelScore.F;
 
            return new LevelResult(
                SceneHandler.GetActiveName(),
                elapsed,
                score
            );
        }
    }
}