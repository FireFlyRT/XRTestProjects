using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ExplosiveDevice : XRGrabInteractable
{
    public UnityEvent OnDetonated;
    private bool isActivated;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (args.interactorObject.transform.GetComponent<XRSocketInteractor>() != null)
            isActivated = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActivated && collision.gameObject.GetComponent<WandProjectile>() != null) 
        {
            OnDetonated?.Invoke();
        }
    }
}
