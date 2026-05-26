using System;
using UnityEngine;

namespace VRGame.Managers
{
    /// <summary>
    /// Generic base class for Singleton MonoBehaviours
    /// </summary>
    public abstract class SingletonBehaviour<TBehaviour> : MonoBehaviour, IDisposable
        where TBehaviour : MonoBehaviour
    {
        public static TBehaviour Instance
        {
            get
            {
                lock (Lock)
                {
                    if (!Usable) return null;
                    return BehaviourInstance ? BehaviourInstance : null;
                }
            }
        }


        protected static bool Usable => !_disposedInstance && !_quiting;
        protected static TBehaviour BehaviourInstance { get; private set; }
        
        private static readonly object Lock = new();
        private static bool _disposedInstance;
        private static bool _quiting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            BehaviourInstance = null;
            _disposedInstance = false;
            _quiting = false;
        }
        
        private void Awake()
        {
            if (BehaviourInstance != null && BehaviourInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            BehaviourInstance = this as TBehaviour;
            _disposedInstance = false;
            _quiting = false;

            if (DontDestroy())
            {
                if (transform.parent != null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[{typeof(TBehaviour).Name}] Singleton detached from parent '{transform.parent.name}' to add to Don't destroy on load.");    
#endif
                    transform.parent = null;
                }
                
                DontDestroyOnLoad(gameObject);
            }

            OnAwake();
        }
        
        protected virtual void OnAwake() { }
        protected abstract bool DontDestroy();
        protected virtual void OnDisposeInstance() { }
        protected virtual void OnDispose() { }
        
        public void Dispose()
        {
            if (BehaviourInstance == this && !_disposedInstance)
            {
                _disposedInstance = true;
                OnDisposeInstance(); // dispose stuff that the instance only has.
                BehaviourInstance = null;
            }
            OnDispose(); // dispose serialized references and stuff the class might have.
        }
        
        private void OnDestroy()
        {
            Dispose();
        }

        private void OnApplicationQuit()
        {
            _quiting = true;
            Dispose();
        }
    }
}