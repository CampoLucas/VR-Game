using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRGame.DesignPatterns.Observers;
using VRGame.Level.Data;
using VRGame.Managers;
using VRGame.SceneManagement;

namespace VRGame.Level
{
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        private enum TransitionState { None, In, Out }
        
        public static readonly ISubject<float> InTransition = new Subject<float>();
        public static readonly ISubject<float> OutTransition = new Subject<float>();
        public static readonly ISubject<string> OnLevelLoaded = new Subject<string>();

        public Timer LevelTimer { get; private set; } = new Timer();
        public LevelResult LastResult { get; private set; } = new LevelResult();
        public static string MainMenuScene => "MainMenu";

        [Header("Level Settings")]
        [SerializeField] private LevelSO current;
        [SerializeField] private string resultsScene  = "ResultsScreen";
        
        [Header("Level Transition Settings")]
        [SerializeField] private float inDuration = 1f;
        [SerializeField] private float outDuration = 1f;
        
        private TransitionState _state;
        private float _elapsed;
        private float _inDuration;
        private float _outDuration;
        private string _nextScene;

        private ActionObserver<float> _onTimerFinished;

        protected override void OnAwake()
        {
            SceneHandler.SceneLoaded += OnSceneLoaded;
            _onTimerFinished = new ActionObserver<float>(OnTimerFinished);
            LevelTimer.OnFinished.Attach(_onTimerFinished);
            
            StartTimer();
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }

        private void Tick(float delta)
        {
            if (_state == TransitionState.None)
            {
                LevelTimer.Tick(delta);
                return;
            }

            _elapsed += delta;

            if (_state == TransitionState.In)
            {
                var t = Mathf.Clamp01(_elapsed / _inDuration);
                InTransition.NotifyAll(t);

                if (t >= 1f)
                {
                    SceneHandler.LoadScene(_nextScene);
                }
            }
            else if (_state == TransitionState.Out)
            {
                var t = Mathf.Clamp01(_elapsed / _outDuration);
                OutTransition.NotifyAll(t);
 
                if (t >= 1f)
                {
                    _state = TransitionState.None;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnLevelLoaded.NotifyAll(scene.name);
            _elapsed = 0f;
            _state = TransitionState.Out;

            StartTimer();
        }

        private void StartTimer()
        {
            if (current != null)
            {
                LevelTimer.StartTimer(current.TimerDuration);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"[{nameof(LevelManager)}] WARNING: LevelSO missing.", this);
#endif
            }
        }

        #region Timer Events

        private void OnTimerFinished(float elapsed)
        {
            LastResult = LevelResult.RecordResult(current, elapsed);
        }

        #endregion

        public void CompleteLevel()
        {
            if (!LevelTimer.IsRunning) return;
            LevelTimer.FinishTimer();
            TransitionToScene(resultsScene);
        }
        
        public void LoadScene(string sceneName)
        {
            SceneHandler.LoadScene(sceneName);
        }
 
        public void LoadScene(int sceneIndex)
        {
            SceneHandler.LoadScene(sceneIndex);
        }
 
        public void ReloadLevel()
        {
            SceneHandler.LoadScene(SceneHandler.GetActiveIndex());
        }

        #region Transition Methods

        public bool TransitionToScene(string sceneName)
        {
            return TransitionToScene(sceneName, inDuration, outDuration);
        }
        
        public bool TransitionToScene(int sceneIndex)
        {
            return TransitionToScene(sceneIndex, inDuration, outDuration);
        }
        
        public bool TransitionToScene(string sceneName, float inDuration, float outDuration)
        {
            if (IsTransitioning())
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[{nameof(LevelManager)}] Warning, trying to transition to another scene when a transition is active");
#endif
                return false;
            }
 
            _nextScene = sceneName;
            _inDuration = inDuration;
            _outDuration = outDuration;
            _elapsed = 0f;
            _state = TransitionState.In;
            return true;
        }
        
        
        public bool TransitionToScene(int sceneIndex, float inDuration, float outDuration)
        {
            return TransitionToScene(SceneHandler.GetSceneNameByIndex(sceneIndex), inDuration, outDuration);
        }

        public bool IsTransitioning() => _state != TransitionState.None;

        #endregion
        
        protected override bool DontDestroy() => true;

        protected override void OnDisposeInstance()
        {
            SceneHandler.SceneLoaded -= OnSceneLoaded;
            InTransition.Dispose();
            OutTransition.Dispose();
            OnLevelLoaded.Dispose();
            
            LevelTimer.OnFinished.Detach(_onTimerFinished);
            _onTimerFinished.Dispose();
        }

        protected override void OnDispose()
        {
            LevelTimer.Dispose();
        }
    }
}