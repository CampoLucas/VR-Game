using System;
using UnityEngine;

namespace VRGame.General
{
    public class UpdateDelay : IDisposable
    {
        private readonly float _interval;
        private Action<float> _action;
        private float _lastTime;

        public UpdateDelay(float interval, Action<float> action)
        {
            _interval = interval;
            _action = action;
            _lastTime = Time.time;
        }

        public void Run()
        {
            if (Time.time - _lastTime < _interval) return;

            var time = Time.time;
            var deltaTime = time - _lastTime;
            _lastTime = time;
            _action(deltaTime);
        }
        
        public void Dispose()
        {
            _action = null;
        }
    }
}