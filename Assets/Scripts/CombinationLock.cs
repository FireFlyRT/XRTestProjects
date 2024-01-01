using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CombinationLock : MonoBehaviour
{
    private const string START_INFO_TEXT = "Enter 3 Digits to Unlock";
    private const string RESET_INFO_TEXT = "Enter 3 Digits to Lock";
    private const string UNLOCK_PANEL_TEXT = "Unlocked";
    private const string LOCK_PANEL_TEXT = "Locked";

    [SerializeField]
    private XRButtonInteractable[] comboButtons;
    [SerializeField]
    private TMP_Text infoText; 
    [SerializeField]
    private Image lockedPanel;
    [SerializeField]
    private Color unlockColor;
    [SerializeField]
    private Color lockColor;
    [SerializeField]
    private TMP_Text unlockText;
    [SerializeField]
    private TMP_Text inputText;
    [SerializeField]
    private bool isLocked;
    [SerializeField] 
    private bool isResettable;
    [SerializeField]
    private int[] comboValues = new int[3];
    [SerializeField]
    private int[] inputValues;

    private int maxButtonPresses;
    private int buttonPresses;
    private bool resetCombo;

    public UnityAction UnlockAction;
    private void OnUnlocked() => UnlockAction?.Invoke();
    public UnityAction LockAction;
    private void OnLocked() => LockAction?.Invoke();

    // Start is called before the first frame update
    void Start()
    {
        inputValues = new int[comboValues.Length];
        maxButtonPresses = comboValues.Length;
        ResetLock();

        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        if (buttonPresses >= maxButtonPresses)
        {
            // Do SOMTH
        }

        else
        {
            for (int i = 0; i < comboButtons.Length; i++)
            {
                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    inputText.text += i.ToString();
                    inputValues[buttonPresses] = i;
                }
                else
                {
                    comboButtons[i].ResetButtonColor();
                }
            }

            buttonPresses++;

            if (buttonPresses == maxButtonPresses)
            {
                if (CheckCombination())
                {
                    Unlock();
                }
                else
                {
                    ResetLock();
                }
                
            }
        }
    }

    private bool CheckCombination()
    {
        if (resetCombo)
        {
            resetCombo = false;
            Lock();
            return false;
        }

        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (!(comboValues[i] == inputValues[i]))
            {
                return false;
            }
        }

        return true;
    }

    private void Unlock()
    {
        isLocked = false;
        OnUnlocked();
        lockedPanel.color = unlockColor;
        unlockText.text = UNLOCK_PANEL_TEXT;
        for (int i = 0; i < inputValues.Length; i++)
        {
            inputValues[i] = 0;
        }
        inputText.text = "";
        if (isResettable)
        {
            ResetCombination();
        }
    }

    private void Lock()
    {
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        isLocked = true;
        OnLocked();
    }

    private void ResetCombination()
    {
        infoText.text = RESET_INFO_TEXT;
        buttonPresses = 0;
        resetCombo = true;
    }

    private void ResetLock()
    {
        infoText.text = START_INFO_TEXT;
        inputText.text = "";
        unlockText.text = LOCK_PANEL_TEXT;
        lockedPanel.color = lockColor;
        buttonPresses = 0;
        for(int i = 0; i < inputValues.Length; i++)
        {
            inputValues[i] = 0;
        }
    }
}
