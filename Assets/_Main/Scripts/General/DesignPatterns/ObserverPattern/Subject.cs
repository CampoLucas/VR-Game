using System;
using System.Collections.Generic;
using System.Linq;

namespace VRGame.DesignPatterns.Observers
{
    public class BaseSubject<TObserver> : IDisposable
        where TObserver : IDisposable
    {
        protected HashSet<TObserver> Subscribers = new();
        protected readonly bool DetachOnNotify;
        protected readonly bool DisposeOnDetach;
        private HashSet<TObserver> _toDispose = new();
        
        protected BaseSubject(bool detachOnNotify = false, bool disposeOnDetach = false)
        {
            DetachOnNotify = detachOnNotify;
            DisposeOnDetach = disposeOnDetach;
        }

        public bool Attach(TObserver observer, bool disposeOnDetach = false)
        {
            if (observer != null && Subscribers != null && Subscribers.Add(observer))
            {
                if (disposeOnDetach)
                {
                    _toDispose.Add(observer);
                }
                return true;
            }

            return false;
        }
            

        public bool Detach(TObserver observer)
        {
            if (Subscribers != null && Subscribers.Remove(observer))
            {
                if (DisposeOnDetach || _toDispose.Contains(observer)) observer.Dispose();
                return true;
            }

            return false;
        }

        public void DetachAll()
        {
            if (DisposeOnDetach || _toDispose is { Count: > 0 })
            {
                var toDispose = DisposeOnDetach ? Subscribers : _toDispose;
                
                foreach (var subscriber in toDispose)
                {
                    subscriber.Dispose();
                }
            }
            
            _toDispose?.Clear();
            Subscribers?.Clear();
        }

        public virtual void Dispose()
        {
            DetachAll();
            Subscribers = null;
            _toDispose = null;
        }
    }

    public class Subject : BaseSubject<IObserver>, ISubject
    {
        public Subject(bool detachOnNotify = false, bool disposeOnDetach = false) : base(detachOnNotify, disposeOnDetach)
        {
            
        }
        
        public void NotifyAll()
        {
            var subscribers = Subscribers.ToList();
        
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber == null)
                {
                    continue;
                }
                subscriber.OnNotify();
        
                if (DetachOnNotify) Detach(subscriber);
            }
        }
    }

    public class Subject<T> : BaseSubject<IObserver<T>>, ISubject<T>
    {
        public Subject(bool detachOnNotify = false, bool disposeOnDetach = false) : base(detachOnNotify, disposeOnDetach)
        {
            
        }
        
        public void NotifyAll(T arg)
        {
            var subscribers = Subscribers.ToList();
        
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber == null)
                {
                    continue;
                }
                subscriber.OnNotify(arg);
        
                if (DetachOnNotify) Detach(subscriber);
            }
        }
    }

    public class Subject<T1, T2> : BaseSubject<IObserver<T1, T2>>, ISubject<T1, T2>
    {
        public Subject(bool detachOnNotify = false, bool disposeOnDetach = false) : base(detachOnNotify, disposeOnDetach)
        {
            
        }
        
        public void NotifyAll(T1 arg1, T2 arg2)
        {
            var subscribers = Subscribers.ToList();
        
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber == null)
                {
                    continue;
                }
                subscriber.OnNotify(arg1, arg2);
        
                if (DetachOnNotify) Detach(subscriber);
            }
        }
    }

    public class Subject<T1, T2, T3> : BaseSubject<IObserver<T1, T2, T3>>, ISubject<T1, T2, T3>
    {
        public void NotifyAll(T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = Subscribers.ToList();
        
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber == null)
                {
                    continue;
                }
                subscriber.OnNotify(arg1, arg2, arg3);
        
                if (DetachOnNotify) Detach(subscriber);
            }
        }
    }
}