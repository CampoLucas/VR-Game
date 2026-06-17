using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    [SerializeField] private LeverPuzzleManager manager;
    [SerializeField] private GameObject lever;
    [SerializeField] private AudioSource audio;
    [SerializeField] private string activatorTag;
    [SerializeField] private int state = 2;

    public int State { get => state; set => state = value;}

    public void LeverStateChanged(int value) 
    {
        state = value;
        manager.CheckLeversCombination();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!lever.active)
        {
            if (other.tag == activatorTag)
            {
                other.gameObject.SetActive(false);
                lever.SetActive(true);
                audio.Play();
            }
        }
    }
}
