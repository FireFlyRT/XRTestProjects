using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPhisicsButtonInteractable : XRSimpleInteractable
{
    [SerializeField]
    private Collider baseCollider;

    public UnityEvent OnBaseEnter;
    public UnityEvent OnBaseExit;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (baseCollider != null)
        {
            if (isHovered && other == baseCollider)
            {
                OnBaseEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (baseCollider != null)
        {
            if (other == baseCollider)
            {
                OnBaseExit?.Invoke();
            }
        }
    }
}
