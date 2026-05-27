using UnityEngine;

namespace Game.Level
{
    public class LevelTimes : ScriptableObject
    {
        [SerializeField] private float sGradeTime;
        [SerializeField] private float aGradeTime;
        [SerializeField] private float bGradeTime;
        [SerializeField] private float cGradeTime;

        public string GetGrade(float timeLeft)
        {
            if (timeLeft >= sGradeTime) return "S";
            if (timeLeft >= aGradeTime) return "A";
            if (timeLeft >= bGradeTime) return "B";
            if (timeLeft >= cGradeTime) return "C";
            return "F";
        }
    }
}