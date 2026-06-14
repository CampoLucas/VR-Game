using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSpot : MonoBehaviour
{
    [SerializeField] private GearPuzzle gearPuzzle;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private bool hasGear;
    [SerializeField] private LayerMask gearLeyerMask;
    [SerializeField] private Gear currentGear;

    public bool HasGear { get => hasGear; set => hasGear = value; }
    public Gear CurrentGear { get => currentGear; set => currentGear = value; }

    public void CheckGear() 
    {
        
        Collider[] temp = Physics.OverlapCapsule(point1.position,point2.position,Vector3.Distance(point1.position,point2.position),gearLeyerMask);
        foreach (Collider c in temp) 
        {
            if(c.tag == "Gear")
            {
                currentGear = c.gameObject.GetComponent<Gear>();
            }
        }
        if (currentGear != null)
        {
            hasGear = true;
            gearPuzzle.CheckSolution();
        }
    }
    public void RemoveGear() 
    {
        if (currentGear) currentGear.SetRotatingLogic(false, 0);
        currentGear = null;
        hasGear = false;
        gearPuzzle.CheckSolution();
    }
}
