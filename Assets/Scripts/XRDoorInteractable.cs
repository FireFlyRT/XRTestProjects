using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent OnOpen;

    private Vector3 startRotation;
    private float startAngleX;
    private float endAngleX;
    private bool isClosed;
    private bool isOpen;

    protected override void Start()
    {
        base.Start();
        startRotation = transform.localEulerAngles;
        startAngleX = GetAngle(startRotation.x);

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
        float localAngleX = GetAngle(transform.localEulerAngles.x);        

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
            OnOpen?.Invoke();
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

    private float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }

        return angle;
    }
}
