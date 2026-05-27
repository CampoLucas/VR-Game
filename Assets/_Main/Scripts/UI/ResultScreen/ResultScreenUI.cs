using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRGame.Level;

namespace VRGame.UI
{
    public class ResultScreenUI : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeText;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            var result = LevelManager.Instance.LastResult;

            if (result == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[{nameof(ResultScreenUI)}] WARNING: No last result found.");
#endif
                return;
            }

            Populate(result);
            retryButton.onClick.AddListener(OnRetry);
            mainMenuButton.onClick.AddListener(OnMainMenu);
        }

        private void Populate(LevelResult result)
        {
            titleText.text = result.Score == LevelScore.F ? "Level Lost" : "Level Won";
            scoreText.text = $"Score: {FormatScore(result.Score)}";
            timeText.text  = $"Time: {FormatTime(result.Elapsed)}";
        }
        
        private static string FormatScore(LevelScore score) => score switch
        {
            LevelScore.S => "S",
            LevelScore.A => "A",
            LevelScore.B => "B",
            LevelScore.C => "C",
            LevelScore.F => "F",
            _ => "?"
        };
 
        private static string FormatTime(float seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{ts.Minutes:00}:{ts.Seconds:00}";
        }
        
        private void OnRetry()
        {
            LevelManager.Instance.TransitionToScene(LevelManager.Instance.LastResult.SceneName);
        }
 
        private void OnMainMenu()
        {
            LevelManager.Instance.TransitionToScene(LevelManager.MainMenuScene);
        }
        
        private void OnDestroy()
        {
            retryButton.onClick.RemoveListener(OnRetry);
            mainMenuButton.onClick.RemoveListener(OnMainMenu);
        }
    }
}
