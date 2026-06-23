using Oculus.Interaction;
using UnityEngine;

namespace VRGame.General.Interactables
{
    /// <summary>
    /// Defines a contact with the button interactable.
    /// </summary>
    public class ButtonInteractor : OverlapInteractor<ButtonInteractor, ButtonInteractable>
    {
        [Header("Approach")]
        [Tooltip("Min dot between the approach direction and the button up axis. 1 = straight on top, 0 = from the side.")]
        [SerializeField, Range(0f, 1f)] private float topApproachDot = 0.5f;

        public Vector3 PressTipPosition => origin.position;
        
        protected override bool ComputeShouldSelect()
        {
            return InContact;
            return InContact && Candidate != null && IsOnTop(Candidate);
        }
 
        private bool IsOnTop(ButtonInteractable button)
        {
            var buttonTr = button.transform;
            
            var toTip = (PressTipPosition - buttonTr.position).normalized;
            return Vector3.Dot(toTip, buttonTr.up) >= topApproachDot;
        }

    }
}