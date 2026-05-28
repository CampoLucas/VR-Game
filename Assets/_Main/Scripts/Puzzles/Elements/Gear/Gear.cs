using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    [SerializeField] private bool isRotating = false;
    [SerializeField] private float gearDiameter = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private GameObject visualsGO;

    public bool IsRotating { get => isRotating; set => isRotating = value; }

    private void Update()
    {
        visualsGO.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    public void SetRotatingLogic(bool IsRotating,float RotationSpeed) 
    {
        isRotating = IsRotating;
        rotationSpeed = RotationSpeed;
    }
    public void SetRotationOffSet(float RotationOffSet) 
    {
        visualsGO.transform.Rotate(Vector3.up,RotationOffSet);
    }
    public float GetCurrentRotationY() 
    {
        return visualsGO.transform.localRotation.eulerAngles.y;
    }
}
