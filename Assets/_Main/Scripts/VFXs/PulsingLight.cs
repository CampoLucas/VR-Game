using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class PulsingLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light light;
    [SerializeField] private float period = 4f;
    [SerializeField] private float oscillations = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float holdRatio = 0.3f;
    [SerializeField] private float wobbleFreq = 5f;
    [SerializeField] private float wobbleAmp = 0.15f;
    [SerializeField] private float maxIntensity = 2f;

    [Header("Shader Settings")]
    [SerializeField] private string globalProperty;

    private int _lightIntensityId;

    private void Awake()
    {
        if (!light) light = GetComponent<Light>();
        
        if (string.IsNullOrEmpty(globalProperty)) return;
        _lightIntensityId = Shader.PropertyToID(globalProperty);
    }

    private void Update()
    {
        var v = IrregularPulse(Time.time);
        light.intensity = v * maxIntensity;
        
        if (string.IsNullOrEmpty(globalProperty)) return;
        Shader.SetGlobalFloat(_lightIntensityId, v);
    }
    
    private float IrregularPulse(float time)
    {
        var t = Mathf.Repeat(time / period, 1f);
        var phase = Mathf.Clamp01(t / (1f - holdRatio));
        var wave = 0.5f - 0.5f * Mathf.Cos(phase * Mathf.PI * 2f * (oscillations + 0.5f));
        var fade = Mathf.Sin(phase * Mathf.PI);
        var wobble = Mathf.Sin(phase * wobbleFreq * Mathf.PI * 2f) * wobbleAmp * fade;
        return Mathf.Clamp01(wave + wobble);
    }
}
