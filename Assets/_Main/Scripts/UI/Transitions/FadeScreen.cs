using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using VRGame.DesignPatterns.Observers;
using VRGame.Level;
using VRGame.SceneManagement;

namespace VRGame.UI
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        private DesignPatterns.Observers.IObserver<float> _inObserver;
        private DesignPatterns.Observers.IObserver<float> _outObserver;
 
        private void Start()
        {
            _inObserver = new FadeInObserver(SetAlpha);
            _outObserver = new ActionObserver<float>(t => SetAlpha(1f - t));
 
            LevelManager.InTransition.Attach(_inObserver);
            LevelManager.OutTransition.Attach(_outObserver);
 
            SetAlpha(0f);
        }
        
        private void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }

        private void OnDestroy()
        {
            LevelManager.InTransition.Detach(_inObserver);
            LevelManager.OutTransition.Detach(_outObserver);
            
            _inObserver?.Dispose();
            _outObserver?.Dispose();
        }

        private abstract class FadeObserver : DesignPatterns.Observers.IObserver<float>
        {
            protected Action<float> SetAlpha;

            protected FadeObserver(Action<float> setAlpha)
            {
                SetAlpha = setAlpha;
            }

            public abstract void OnNotify(float t);

            public void Dispose()
            {
                SetAlpha = null;
            }
        }

        private sealed class FadeInObserver : FadeObserver
        {
            public FadeInObserver(Action<float> setAlpha) : base(setAlpha)
            {
            }

            public override void OnNotify(float t)
            {
                SetAlpha(t);
            }
        }

        private sealed class FadeOutObserver : FadeObserver
        {
            public FadeOutObserver(Action<float> setAlpha) : base(setAlpha)
            {
            }

            public override void OnNotify(float t)
            {
                SetAlpha(1f - t);
            }
        }
        
    }
}
