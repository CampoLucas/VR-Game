using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private float refreshTimer = 0.2f;
    private float currentTimer = 0f;

    private void Update()
    {
        if (currentTimer >= refreshTimer)
        {
            float fps = 1 / Time.deltaTime;
            _textMeshPro.text = "FPS: " + (int)fps;
            currentTimer = 0f;
        }
        else currentTimer += Time.deltaTime;
    }
}
