using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIController : MonoBehaviour
{
    [SerializeField]
    private ProgressControll progressControl;
    [SerializeField]
    private TMP_Text[] messageTexts;

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallangeComplete);
        }
    }

    private void ChallangeComplete(string arg0)
    {
        SetText(arg0);
    }

    private void StartGame(string arg0)
    {
        SetText(arg0);
    }

    public void SetText(string msg)
    {
        for (int i = 0; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = msg;
        }
    }
}
