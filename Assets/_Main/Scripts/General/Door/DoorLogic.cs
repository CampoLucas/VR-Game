using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class DoorLogic : MonoBehaviour
{
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;
    [SerializeField] private List<GameObject> puzzlesGO = new List<GameObject>();
    private List<IPuzzleInterface> puzzlesControllers = new List<IPuzzleInterface>();

    private void Start()
    {
        foreach (GameObject go in puzzlesGO) 
        {
            var temp = go.GetComponent<IPuzzleInterface>();
            if (temp != null) puzzlesControllers.Add(temp);
        }
    }
    public void CheckPuzzleList() 
    {
        bool temp = true;
        foreach(IPuzzleInterface puzzle in puzzlesControllers) 
        {
            if(!puzzle.GetIsResolved()) temp = false;
        }
        if(temp) OpenDoor();
    }
    private void OpenDoor()
    {
        Debug.Log("Open the Door");
        closedDoor.SetActive(false);
        openedDoor.SetActive(true);
    }
}
