using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using VRGame.DesignPatterns.Observers;
using VRGame.Managers;
using VRGame.SceneManagement;

namespace VRGame.Audio
{
    public class SoundManager : SingletonBehaviour<SoundManager>
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField, Range(0f, 1f)] private float musicVolume;

        private DesignPatterns.Observers.IObserver<float> _inObserver;
        private DesignPatterns.Observers.IObserver<float> _outObserver;

        protected override void OnAwake()
        {
            _inObserver  = new MusicFadeIn(Source, Volume);
            _outObserver = new MusicFadeOut(Source, Volume);
 
            LevelManager.InTransition.Attach(_inObserver);
            LevelManager.OutTransition.Attach(_outObserver);
        }

        public void PlayClip(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void Stop() => audioSource.Stop();

        protected override bool DontDestroy() => true;

        private AudioSource Source() => audioSource;
        private float Volume() => musicVolume;

        protected override void OnDisposeInstance()
        {
            _inObserver?.Dispose();
            _outObserver?.Dispose();
        }

        private abstract class MusicFade : DesignPatterns.Observers.IObserver<float>
        {
            protected Func<AudioSource> Source;
            protected Func<float> Volume;

            protected MusicFade(Func<AudioSource> source, Func<float> volume)
            {
                Source = source;
                Volume = volume;
            }
            
            public abstract void OnNotify(float t);

            public void Dispose()
            {
                Source = null;
                Volume = null;
            }
        }

        private sealed class MusicFadeIn : MusicFade
        {
            public MusicFadeIn(Func<AudioSource> source, Func<float> volume) : base(source, volume)
            {
            }

            public override void OnNotify(float t)
            {
                Source().volume = Mathf.Lerp(Volume(), 0f, t);
            }
        }
        
        private sealed class MusicFadeOut : MusicFade
        {
            public MusicFadeOut(Func<AudioSource> source, Func<float> volume) : base(source, volume)
            {
            }

            public override void OnNotify(float t)
            {
                Source().volume = Mathf.Lerp(0f,Volume(), t);
            }
        }
    }
}
