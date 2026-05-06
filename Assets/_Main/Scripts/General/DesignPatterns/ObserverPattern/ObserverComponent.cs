using UnityEngine;

namespace VRGame.DesignPatterns.Observers
{
    public abstract class ObserverComponent<T> : MonoBehaviour, IObserver<T>
    {
        public void OnNotify(T arg)
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}