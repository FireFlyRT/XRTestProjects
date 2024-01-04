using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] 
    private Transform _drawerTransform;

    [SerializeField] 
    private XRSocketInteractor _keySocket;
    public XRSocketInteractor GetKeySocket => _keySocket;

    [SerializeField] 
    private GameObject _keyIndecLight;
    [SerializeField] 
    private bool _isLocked;

    [SerializeField] 
    private InteractionLayerMask _defaultLayer;
    [SerializeField] 
    private InteractionLayerMask _grabLayer;

    [SerializeField] 
    private Vector3 _limitDistances = new(.02f, .02f, 0);
    [SerializeField] 
    private float _maxLimitPositionZ = 0.85f;

    [SerializeField]
    private AudioClip drawerMoveClip;
    public AudioClip GetDrawerMoveClip => drawerMoveClip;
    [SerializeField]
    private AudioClip socketedClip;
    public AudioClip GetSocketedClip => socketedClip;

    private Transform _parentTransform;
    private Vector3 _limitPositions;
    private bool _isGrabed;


    // Start is called before the first frame update
    void Start()
    {
        _isLocked = true;
        if (_keySocket != null)
        {
            _keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            _keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        _parentTransform = transform.parent.transform;
        _limitPositions = _drawerTransform.localPosition; 
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!_isLocked)
        {
            transform.SetParent(_parentTransform);
            _isGrabed = true;
        }
        else
        {
            ChangeLayerMask(_defaultLayer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ChangeLayerMask(_grabLayer);
        _isGrabed = false;
        transform.localPosition = _drawerTransform.localPosition;
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        _isLocked = true;
    }

    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        _isLocked = false;
        _keyIndecLight.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGrabed && _drawerTransform != null)
        {
            _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x,
                _drawerTransform.localPosition.y, transform.localPosition.z);

            CheckLimits();
        }   
    }

    private void CheckLimits()
    {
        if (transform.localPosition.x >= _limitPositions.x + _limitDistances.x ||
            transform.localPosition.x <= _limitPositions.x - _limitDistances.x ||
            transform.localPosition.y >= _limitPositions.y + _limitDistances.y ||
            transform.localPosition.y <= _limitPositions.y - _limitDistances.y )
        {
            ChangeLayerMask(_defaultLayer);
        }
        else if (_drawerTransform.localPosition.z <= _limitPositions.z - _limitDistances.z)
        {
            _isGrabed = false;
            _drawerTransform.localPosition = _limitPositions;
            ChangeLayerMask(_defaultLayer);
        }
        else if (_drawerTransform.localPosition.z >= _maxLimitPositionZ + _limitDistances.z)
        {
            _isGrabed = false;
            _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, _maxLimitPositionZ);
            ChangeLayerMask(_defaultLayer);
        }
    }

    private void ChangeLayerMask(InteractionLayerMask mask)
    {
        interactionLayers = mask;
    }
}
