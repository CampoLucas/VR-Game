using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Elements.Button
{
    /// <summary>
    /// Manages states transitions for the button.
    /// </summary>
    public class ButtonStateMachine : IDisposable
    {
        public ButtonState Current { get; private set; }
        public ISubject<ButtonState> OnStateChanged { get; } = new Subject<ButtonState>();
        
        public ButtonStateMachine(ButtonState initialState = ButtonState.Normal)
        {
            Current = initialState;
        }

        public void TransitionTo(ButtonState target)
        {
            if (target == Current) return;

            Current = target;
            OnStateChanged.NotifyAll(Current);
        }

        public void Dispose()
        {
            OnStateChanged?.Dispose();
        }
    }
}