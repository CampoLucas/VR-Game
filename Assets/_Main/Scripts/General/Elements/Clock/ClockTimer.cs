using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using VRGame.DesignPatterns.Observers;
using VRGame.Level;

public class ClockTimer : MonoBehaviour
{
    [SerializeField] private bool countDown = true;
    [SerializeField] private TMP_Text timerText;
    
    private IObserver<float, float> _tickObserver;
    
    private void Start()
    {
        _tickObserver = new ActionObserver<float, float>(OnTick);
        if (!LevelManager.Instance)
        {
#if UNITY_EDITOR
            Debug.LogError($"[{nameof(ClockTimer)}] WARNING: Level Manager missing.", this);
#endif
            return;
        }
        LevelManager.Instance.LevelTimer.OnTick.Attach(_tickObserver);
    }

    private void OnTick(float elapsed, float remaining)
    {
        var display = countDown ? remaining : elapsed;
        timerText.text = Format(display);
    }

    
    public void Stop()
    {
        if (!LevelManager.Instance) return;
        LevelManager.Instance.LevelTimer.Stop();
    }

    public void Resume()
    {
        if (!LevelManager.Instance) return;
        LevelManager.Instance.LevelTimer.Resume();
    }
    
    private static string Format(float seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return $"{ts.Minutes:00}:{ts.Seconds:00}";
    }
    
    private void OnDestroy()
    {
        var manager = LevelManager.Instance;
        
        if (manager && manager.LevelTimer != null) manager.LevelTimer.OnTick.Detach(_tickObserver);
    }
    
}
