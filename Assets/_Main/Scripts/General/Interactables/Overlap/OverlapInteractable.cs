using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

namespace VRGame.General.Interactables
{
    public abstract class OverlapInteractable<TInteractor, TInteractable> : Interactable<TInteractor, TInteractable>
        where TInteractor : Interactor<TInteractor, TInteractable>
        where TInteractable : Interactable<TInteractor, TInteractable>
    {
        
    }
}