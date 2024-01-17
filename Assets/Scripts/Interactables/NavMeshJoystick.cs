using System;
using UnityEngine;

public class NavMeshJoystick : SimpleHinchInteractable
{
    [SerializeField]
    private Transform _trackedObject;
    [SerializeField]
    private Transform _trackingObject;

    [SerializeField]
    private NavMeshRobot _robot;
    [SerializeField]
    private Transform _rotationParentObject;

    protected override void ResetHinch()
    {
        if (_robot != null)
        {
            _robot.StopAgent();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isSelected)
        {
            MoveRobot();
        }
    }

    private void MoveRobot()
    {
        if (_robot != null)
        {
            _trackingObject.position = new Vector3(
                _trackedObject.position.x,
                _trackingObject.position.y,
                _trackedObject.position.z);
            _rotationParentObject.rotation = Quaternion.identity;
            _robot.MoveAgent(_trackingObject.localPosition);
        }
    }
}
