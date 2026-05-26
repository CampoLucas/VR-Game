using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRGame.DesignPatterns.Observers;
using VRGame.Managers;

namespace VRGame.SceneManagement
{
    public class LevelManager : SingletonBehaviour<LevelManager>
    {
        private enum TransitionState { None, In, Out }
        
        public static readonly ISubject<float> InTransition  = new Subject<float>();
        public static readonly ISubject<float> OutTransition = new Subject<float>();
        public static readonly ISubject<string> OnLevelLoaded = new Subject<string>();

        [SerializeField] private float inDuration = 1f;
        [SerializeField] private float outDuration = 1f;
        
        private TransitionState _state;
        private float _elapsed;
        private float _inDuration;
        private float _outDuration;
        private string _nextScene;

        protected override void OnAwake()
        {
            SceneHandler.SceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (_state == TransitionState.None) return;
            Tick(Time.deltaTime);
        }

        private void Tick(float delta)
        {
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
        
        protected override bool DontDestroy() => true;

        protected override void OnDisposeInstance()
        {
            SceneHandler.SceneLoaded -= OnSceneLoaded;
            InTransition.Dispose();
            OutTransition.Dispose();
            OnLevelLoaded.Dispose();
        }
    }
}