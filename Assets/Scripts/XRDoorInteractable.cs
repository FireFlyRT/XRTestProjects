using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDoorInteractable : SimpleHinchInteractable
{
    [SerializeField]
    private Transform doorObject;
    [SerializeField]
    private CombinationLock combinationLock;
    [SerializeField]
    private Vector3 rotationLimits;
    [SerializeField]
    private Collider closedCollider;
    [SerializeField]
    private Collider openCollider;
    [SerializeField]
    private Vector3 endRotation;

    private Vector3 startRotation;
    private float startAngleX;
    private float endAngleX;
    private bool isClosed;
    private bool isOpen;

    protected override void Start()
    {
        base.Start();
        startRotation = transform.localEulerAngles;
        startAngleX = startRotation.x;
        if (startAngleX >= 180)
            startAngleX -= 360;

        endRotation = openCollider.bounds.center;

        if (combinationLock != null)
        {
            combinationLock.UnlockAction += UnlockHinge;
            combinationLock.LockAction += LockHinge;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (doorObject != null)
        {
            doorObject.localEulerAngles = new Vector3(
                doorObject.localEulerAngles.x,
                transform.localEulerAngles.y,
                doorObject.localEulerAngles.z
                );
        }

        if(isSelected)
            CheckLimits();
    }

    private void CheckLimits()
    {
        isClosed = false;
        isOpen = false;
        float localAngleX = transform.localEulerAngles.x;

        if (localAngleX >= 180)
        {
            localAngleX -= 360;
        }

        if (localAngleX >= startAngleX + rotationLimits.x || 
            localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinch();
        }
    }

    protected override void ResetHinch()
    {
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;   
        }
        else if (isOpen)
        {
            transform.localEulerAngles = endRotation;
        }
        else
        {
            transform.localEulerAngles = new Vector3(
                startAngleX,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z
                );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == closedCollider)
        {
            isClosed = true;
            ReleaseHinch();
        }
        else if (other == openCollider)
        {
            isOpen = true;
            ReleaseHinch();
        }
    }
}
