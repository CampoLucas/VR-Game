using System;
using Oculus.Interaction;
using UnityEngine;

namespace VRGame.General
{
    public class RayOcclusionFilter : MonoBehaviour, IGameObjectFilter
    {
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private float distance;

        private Vector3 _start;
        private Vector3 _dir;
        
        public bool Filter(GameObject gm)
        {
            //return whatToReturn;
            if (gm == null) return false;

            _start = rayOrigin.position;
            _dir = rayOrigin.forward;

            return !Physics.Raycast(_start, _dir.normalized, out _, distance, wallLayerMask);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_start, .1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_start, _dir * distance);
        }
    }
}