using System;
using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using VRGame.General.Interactables;

public class Gear : MonoBehaviour
{
    
    public Action OnGrabbed { get; set; }
    public bool IsRotating { get => isRotating; set => isRotating = value; }
    public SnapInteractor Interactor => interactor;

    [SerializeField] private bool isRotating = false;
    [SerializeField] private float gearDiameter = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private GameObject visualsGO;

    [Header("References")]
    [SerializeField] private Transform parent;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private GameObject interactables;
    [SerializeField] private SnapInteractor interactor;
    [SerializeField] private Rigidbody rb;

    private Transform _transform;


    private void Awake()
    {
        _transform = transform;
        if (!grabbable) grabbable = GetComponent<Grabbable>();
        if (!rb) rb = GetComponent<Rigidbody>();
        
        _transform.SetParent(parent);
    }

    private void Update()
    {
        visualsGO.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    public void SetRotatingLogic(bool IsRotating,float RotationSpeed) 
    {
        isRotating = IsRotating;
        rotationSpeed = RotationSpeed;
    }

    public void SnapOn(SnapParams gearSnapPoint)
    {
        // Disable the grababble
        if (gearSnapPoint.DisableGrabbable)
        {
            grabbable.enabled = false;
            interactables.SetActive(false);
        }

        // Set kinematic
        rb.isKinematic = true;
        
        _transform.position = gearSnapPoint.Position;
        _transform.rotation = gearSnapPoint.Rotation;

        _transform.SetParent(gearSnapPoint.Parent);
    }

    public void SnapOff()
    {
        grabbable.enabled = true;
        interactables.SetActive(true);
        
        
        _transform.SetParent(parent);
    }
}
