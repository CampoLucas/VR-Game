using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearPuzzle : MonoBehaviour
{
    [SerializeField] private List<GearSpot> snapTriggers = new List<GearSpot>();
    [SerializeField] private bool isCorrect = true;
    [SerializeField] private bool puzzleSolved = false;
    // Start is called before the first frame update
    public void CheckSolution() 
    {
        isCorrect = true;
        for(int i = 0; i < snapTriggers.Count; i++)
        {
            if (snapTriggers[i].HasGear) 
            {
                if (i == 0) snapTriggers[i].CurrentGear.SetRotatingLogic(true, Mathf.Pow(-1,i) * 10);
                else 
                {
                    if (snapTriggers[i - 1].HasGear) 
                    {
                        if (snapTriggers[i - 1].CurrentGear.IsRotating) snapTriggers[i].CurrentGear.SetRotatingLogic(true, Mathf.Pow(-1, i) * 10);
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
        Debug.Log(isCorrect);
    }
}
