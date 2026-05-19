using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ClockTimer : MonoBehaviour
{
    [SerializeField] private bool decrese;
    [SerializeField] private int limitedTimeInSeconds = 0;
    [SerializeField] private int timeInSeconds = 0;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private TMP_Text textTime;
    
    private float currentTimer = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        isRunning = true;
        if (decrese) textTime.text = TimeSpan.FromSeconds(limitedTimeInSeconds).Minutes.ToString("00") + ":" + TimeSpan.FromSeconds(limitedTimeInSeconds).Seconds.ToString("00");
        else textTime.text = TimeSpan.FromSeconds(timeInSeconds).Minutes.ToString("00") + ":" + TimeSpan.FromSeconds(timeInSeconds).Seconds.ToString("00");
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
                if (decrese)
                {
                    if (limitedTimeInSeconds > 0)
                    {
                        limitedTimeInSeconds--;
                        textTime.text = TimeSpan.FromSeconds(limitedTimeInSeconds).Minutes.ToString("00") + ":" + TimeSpan.FromSeconds(limitedTimeInSeconds).Seconds.ToString("00");
                    }
                    timeInSeconds++;
                    currentTimer = 0;
                }
                else 
                {
                    timeInSeconds++;
                    currentTimer = 0;
                    textTime.text = TimeSpan.FromSeconds(timeInSeconds).Minutes.ToString("00") + ":" + TimeSpan.FromSeconds(timeInSeconds).Seconds.ToString("00");
                }
            }
        }
    }
}
