using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

[RequireComponent(typeof(Slider))]
public class SimpleSliderControll : MonoBehaviour
{
    [SerializeField]
    private float minValue = 0.0f;
    [SerializeField]
    private float maxValue = 20.0f;
    [SerializeField]
    private Light libraryLight;

    public UnityEvent OnSliderActive;

    private Slider slider;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = minValue;
        slider.onValueChanged.AddListener(OnValueChange);


        if (libraryLight != null)
        {
            libraryLight.intensity = minValue;
        }
    }

    private void OnValueChange(float arg0)
    {
        if (arg0 >= maxValue) 
        {
            OnSliderActive?.Invoke();
        }

        if (libraryLight != null)
        {
            libraryLight.intensity = arg0;
        }
    }
}
