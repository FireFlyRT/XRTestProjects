using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHinchInteractable : XRSimpleInteractable
{
    [SerializeField]
    private bool isLocked;
    [SerializeField] 
    private InteractionLayerMask _defaultLayer;
    [SerializeField] 
    private InteractionLayerMask _grabLayer;
    [SerializeField]
    private Vector3 positionLimits;

    private Transform grabHand;
    private Collider hingeCollider;
    private Vector3 hingePositions;

    protected virtual void Start()
    {
        hingeCollider = GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        if (grabHand != null)
        {
            TrackHand();
        }
    }

    public void UnlockHinge()
    {
        isLocked = false;
    }

    public void LockHinge()
    {
        isLocked = true;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isLocked)
        {
            base.OnSelectEntered(args);
            grabHand = args.interactorObject.transform;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        grabHand = null;
        ChangeLayerMask(_grabLayer);
        ResetHinch();
    }

    private void TrackHand()
    {
        transform.LookAt(grabHand, transform.forward); 
        hingePositions = hingeCollider.bounds.center;
        if (grabHand.position.x >= hingePositions.x + positionLimits.x ||
            grabHand.position.x <= hingePositions.x - positionLimits.x)
        {
            ReleaseHinch();
        }
        else if (grabHand.position.y >= hingePositions.y + positionLimits.y ||
            grabHand.position.y <= hingePositions.y - positionLimits.y)
        {
            ReleaseHinch();
        }
        else if (grabHand.position.z >= hingePositions.z + positionLimits.z || 
            grabHand.position.z <= hingePositions.z - positionLimits.z) 
        {
            ReleaseHinch();
        }
    }

    public void ReleaseHinch()
    {
        ChangeLayerMask(_defaultLayer);
    }

    private void ChangeLayerMask(InteractionLayerMask mask)
    {
        interactionLayers = mask;
    }

    protected abstract void ResetHinch();
}
