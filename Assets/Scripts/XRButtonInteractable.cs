using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRButtonInteractable : XRSimpleInteractable
{
    [SerializeField]
    private Image buttonImage;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color highlightedColor;
    [SerializeField]
    private Color pressedColor;
    [SerializeField]
    private Color selectedColor;
    private bool isPressed;

    private void Start()
    {
        ResetButtonColor();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        isPressed = false;
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        
        if (!isPressed)
        {
            ResetButtonColor();
        }
    }

    // Gets called if we hold the trigger over the button
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        isPressed = true;
        buttonImage.color = pressedColor;
    }

    // Gets called if we release the trigger to really select the button
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        buttonImage.color = selectedColor;
    }

    public void ResetButtonColor()
    {
        buttonImage.color = normalColor;
    }
}
