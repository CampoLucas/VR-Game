using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace VRGame.Puzzles.PhysicalButton
{
    public class PhysicalButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int id;
        
        [Header("References")]
        [SerializeField] private PokeInteractable pokeInteractable;
        [SerializeField] private Transform surface;
        [SerializeField] private Transform buttonVisual;

        [Header("Events")]
        [SerializeField] private UnityEvent<int> onPressed;

        public bool IsPressed { get; private set; }
        
        private Vector3 _restPosition;
        
        private void Start()
        {
            _restPosition = buttonVisual.position;
            
            pokeInteractable.WhenSelectingInteractorAdded.Action += OnPress;
        }
        
        private void OnPress(PokeInteractor interactor)
        {
            if (IsPressed) return;
            
            IsPressed = true;
            pokeInteractable.enabled = false;
            buttonVisual.position = surface.position;
            
            onPressed.Invoke(id);
        }
        
        [ContextMenu("Reset")]
        public void ResetButton()
        {
            IsPressed = false;
            buttonVisual.position = _restPosition;
            pokeInteractable.enabled = true;
        }
        
        private void OnDestroy()
        {
            pokeInteractable.WhenSelectingInteractorAdded.Action -= OnPress;
        }
    }
}
