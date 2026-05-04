using System;

namespace VRGame.DesignPatterns.Observers
{
    public class ActionObserverBase<T> : IDisposable where T : class
    {
        protected T Action;

        protected ActionObserverBase(T action)
        {
            this.Action = action;
        }

        public void Dispose()
        {
            Action = null;
        }
    }
    
    public class ActionObserver : ActionObserverBase<Action>, IObserver
    {
        public ActionObserver(Action action) : base(action)
        {
        }
        
        public void OnNotify()
        {
            if (Action != null) Action();
        }
    }
    
    public class ActionObserver<T> : ActionObserverBase<Action<T>>, IObserver<T>
    {
        public ActionObserver(Action<T> action) : base(action)
        {
        }
        
        public void OnNotify(T arg)
        {
            if (Action != null) Action(arg);
        }
    }
    
    public class ActionObserver<T1, T2> : ActionObserverBase<Action<T1, T2>>, IObserver<T1, T2>
    {
        public ActionObserver(Action<T1, T2> action) : base(action)
        {
        }
        
        public void OnNotify(T1 arg1, T2 arg2)
        {
            if (Action != null) Action(arg1, arg2);
        }
    }
    
    public class ActionObserver<T1, T2, T3> : ActionObserverBase<Action<T1, T2, T3>>, IObserver<T1, T2, T3>
    {
        public ActionObserver(Action<T1, T2, T3> action) : base(action)
        {
        }
        
        public void OnNotify(T1 arg1, T2 arg2, T3 arg3)
        {
            if (Action != null) Action(arg1, arg2, arg3);
        }
    }
}