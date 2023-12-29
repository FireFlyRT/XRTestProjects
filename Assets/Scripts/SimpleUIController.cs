using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIController : MonoBehaviour
{
    [SerializeField]
    private string[] messageStrings;
    [SerializeField]
    private TMP_Text[] messageTexts;

    [SerializeField]
    private XRButtonInteractable startButton;
    [SerializeField]
    private GameObject keyIndecatorLight;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(StartButtonPressed);
        }
    }

    private void StartButtonPressed(SelectEnterEventArgs arg0)
    {
        SetText(messageStrings[1]);
        if (keyIndecatorLight != null)
        {
            keyIndecatorLight.SetActive(true);
        }
    }

    public void SetText(string msg)
    {
        for (int i = 0; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = msg;
        }
    }
}
