using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearPuzzle : MonoBehaviour
{
    [SerializeField] private List<GearSpot> snapTriggers = new List<GearSpot>();
    [SerializeField] private Gear startGear;
    [SerializeField] private Gear endGear;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool isCorrect = true;
    [SerializeField] private bool puzzleSolved = false;

    // Start is called before the first frame update
    private void Start()
    {
        startGear.SetRotatingLogic(true, -rotationSpeed);
    }
    public void CheckSolution() 
    {
        isCorrect = true;
        for(int i = 0; i < snapTriggers.Count; i++)
        {
            if (snapTriggers[i].HasGear) 
            {
                if (i == 0) 
                {
                    if (!snapTriggers[i].CurrentGear.IsRotating)
                    {
                        snapTriggers[i].CurrentGear.SetRotationOffSet(-startGear.GetCurrentRotationY()+90f);
                        snapTriggers[i].CurrentGear.SetRotatingLogic(true, Mathf.Pow(-1, i) * rotationSpeed);
                    }
                }
                else
                {
                    if (snapTriggers[i - 1].HasGear)
                    {
                        if (snapTriggers[i - 1].CurrentGear.IsRotating) 
                        {
                            if (!snapTriggers[i].CurrentGear.IsRotating)
                            {
                                snapTriggers[i].CurrentGear.SetRotationOffSet(-snapTriggers[i - 1].CurrentGear.GetCurrentRotationY()+90f);
                                snapTriggers[i].CurrentGear.SetRotatingLogic(true, Mathf.Pow(-1, i) * rotationSpeed);
                            }
                        }
                        else snapTriggers[i].CurrentGear.SetRotatingLogic(false, 0);
                    }
                    else snapTriggers[i].CurrentGear.SetRotatingLogic(false, 0);
                }
            }
            else 
            {
                isCorrect = false;
            }
        }
        if (isCorrect)
        {
            endGear.SetRotationOffSet(startGear.GetCurrentRotationY()+90f);
            endGear.SetRotatingLogic(true, -rotationSpeed);
        }
        else endGear.SetRotatingLogic(false, 0);
        Debug.Log(isCorrect);
    }
}
