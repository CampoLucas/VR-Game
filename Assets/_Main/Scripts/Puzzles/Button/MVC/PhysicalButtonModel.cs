using System;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.PhysicalButton.MVC
{
    public class PhysicalButtonModel : ISubject<int>, IDisposable
    {
        public int Id { get; private set; }
        public bool IsPressed { get; private set; }

        private readonly Subject<int> _subject = new();

        public void Setup(int id)
        {
            Id = id;
        }

        public void SetPressed(bool pressed)
        {
            IsPressed = pressed;
        }

        public bool Attach(DesignPatterns.Observers.IObserver<int> observer, bool disposeOnDetach = false)
        {
            return _subject.Attach(observer, disposeOnDetach);
        }

        public bool Detach(DesignPatterns.Observers.IObserver<int> observer)
        {
            return _subject.Detach(observer);
        }

        public void DetachAll()
        {
            _subject.DetachAll();
        }

        public void NotifyAll(int arg)
        {
            _subject.NotifyAll(arg);
        }
        
        public void Dispose()
        {
            _subject?.Dispose();
        }
    }
}