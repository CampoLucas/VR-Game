using System;
using Oculus.Interaction;
using UnityEngine;

namespace VRGame.General.Interactables
{
    public class GearSnapPointFilter : MonoBehaviour, IGameObjectFilter, IDisposable
    {
        [SerializeField] private GearSnapPoint snapPoint;

        private void Awake()
        {
            if (!snapPoint)
            {
                snapPoint = GetComponent<GearSnapPoint>();
            }
        }

        public bool Filter(GameObject gameObject)
        {
            return snapPoint && snapPoint.IsFree();
        }

        public void Dispose()
        {
            snapPoint = null;
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}