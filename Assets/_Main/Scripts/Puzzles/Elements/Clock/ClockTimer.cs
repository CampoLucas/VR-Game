using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ClockTimer : MonoBehaviour
{
    [SerializeField] private int limitedTimeInSeconds = 0;
    [SerializeField] private int timeInSeconds = 0;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private TMP_Text textTime;
    
    private float currentTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning) 
        {
            if (currentTimer < 1)
            {
                currentTimer += Time.deltaTime;
            }
            else 
            {
                if (limitedTimeInSeconds > 0) 
                { 
                    limitedTimeInSeconds--;
                    textTime.text = TimeSpan.FromSeconds(limitedTimeInSeconds).Minutes.ToString("00") + ":" + TimeSpan.FromSeconds(limitedTimeInSeconds).Seconds.ToString("00");
                }
                timeInSeconds++;
                currentTimer = 0;
            }
        }
    }
}
