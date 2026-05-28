using System;
using UnityEngine;
using VRGame.DesignPatterns.Observers;

namespace VRGame
{
    public class Timer : IDisposable
    {
        public ISubject<float, float> OnTick { get; } = new Subject<float, float>();
        public ISubject<float> OnFinished { get; } = new Subject<float>();
        
        public float Elapsed { get; private set; }
        public float Duration { get; private set; }
        public bool IsRunning { get; private set; }

        public float Remaining => Mathf.Max(0f, Duration - Elapsed);

        public void StartTimer(float duration)
        {
            Duration = duration;
            Elapsed = 0f;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void FinishTimer()
        {
            Stop();
            OnFinished.NotifyAll(Elapsed);
        }

        public void Resume()
        {
            IsRunning = true;
        }

        public void Tick(float delta)
        {
            if (!IsRunning) return;

            Elapsed += delta;
            OnTick.NotifyAll(Elapsed, Remaining);

            if (Elapsed >= Duration)
            {
                FinishTimer();
            }
        }

        public void Dispose()
        {
            OnTick?.Dispose();
            OnFinished?.Dispose();
        }
    }
}