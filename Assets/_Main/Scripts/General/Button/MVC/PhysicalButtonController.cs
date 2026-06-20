using System;
using Oculus.Interaction;
using UnityEngine;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.PhysicalButton.MVC
{
    public class PhysicalButtonController : MonoBehaviour
    {
        public int Id => _model.Id;
        public PhysicalButtonView View => view;
        
        
        [Header("Settings")]
        [SerializeField] private int id;
        
        [Header("References")]
        [SerializeField] private PokeInteractable pokeInteractable;
        [SerializeField] private Transform surface;
        [SerializeField] private Transform buttonVisual;
        [SerializeField] private PhysicalButtonView view;

        private readonly PhysicalButtonModel _model = new();

        public ISubject<int> Subject => _model;
        public bool IsPressed => _model.IsPressed;

        private Vector3 _restPosition;

        private void Awake()
        {
            _model.Setup(id);
        }

        private void Start()
        {
            _restPosition = buttonVisual.localPosition;

            pokeInteractable.WhenSelectingInteractorAdded.Action += OnPress;
            pokeInteractable.WhenSelectingInteractorRemoved.Action += OnRelease;
        }
        
        private void OnPress(PokeInteractor interactor)
        {
            if (_model.IsPressed) return;

            _model.SetPressed(true);
            pokeInteractable.enabled    = false;
            buttonVisual.localPosition  = surface.localPosition;

            view.SetToPressed();
            _model.NotifyAll(_model.Id);
        }
        
        private void OnRelease(PokeInteractor interactor)
        {
            if (!_model.IsPressed) return;

            _model.SetPressed(false);
            view.SetToReleased();
        }
        
        [ContextMenu("Reset")]
        public void ResetButton()
        {
            _model.SetPressed(false);
            buttonVisual.localPosition = _restPosition;
            pokeInteractable.enabled = true;
            view.SetToReleased();
        }
        
        public void Setup(int newId)
        {
            id = newId;
            _model.Setup(newId);
        }
        
        private void OnDestroy()
        {
            pokeInteractable.WhenSelectingInteractorAdded.Action -= OnPress;
            pokeInteractable.WhenSelectingInteractorRemoved.Action -= OnRelease;

            _model.Dispose();
        }
    }
}