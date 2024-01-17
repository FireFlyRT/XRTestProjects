using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] 
    private Transform _drawerTransform;

    [SerializeField] 
    private XRSocketInteractor _keySocket;
    public XRSocketInteractor GetKeySocket => _keySocket;

    [SerializeField]
    private XRPhisicsButtonInteractable _phisicsButton;
    public XRPhisicsButtonInteractable GetPhysicsButton => _phisicsButton;

    [SerializeField] 
    private GameObject _keyIndecLight;
    [SerializeField] 
    private bool _isLocked;
    [SerializeField]
    private bool _isDetachable;
    [SerializeField]
    private bool _isDetached;

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

    public UnityEvent OnDrawerDetatch;

    private Rigidbody _rigidbody;
    private Transform _parentTransform;
    private Vector3 _limitPositions;
    private bool _isGrabed;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _isLocked = true;
        if (_keySocket != null)
        {
            _keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            _keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        _parentTransform = transform.parent.transform;
        _limitPositions = _drawerTransform.localPosition; 

        if (_phisicsButton != null)
        {
            _phisicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            _phisicsButton.OnBaseExit.AddListener(OnIsNotDetachable);
        }
    }

    private void OnIsDetachable()
    {
        _isDetachable = true;
    }

    private void OnIsNotDetachable()
    {
        _isDetachable = false;
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
        if (!_isDetached)
        {
            ChangeLayerMask(_grabLayer);
            _isGrabed = false;
            transform.localPosition = _drawerTransform.localPosition;
        }
        else
            _rigidbody.isKinematic = false;
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
        if (!_isDetached)
        {
            if (_isGrabed && _drawerTransform != null)
            {
                _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x,
                    _drawerTransform.localPosition.y, transform.localPosition.z);

                CheckLimits();
            }
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
            if (!_isDetachable)
            {
                _isGrabed = false;
                _drawerTransform.localPosition = new Vector3(_drawerTransform.localPosition.x, _drawerTransform.localPosition.y, _maxLimitPositionZ);
                ChangeLayerMask(_defaultLayer);
            }
            else
            {
                DetatchDrawer();
            }
        }
    }

    private void DetatchDrawer()
    {
        _isDetached = true;
        _drawerTransform.SetParent(this.transform);
        OnDrawerDetatch?.Invoke();
    }

    private void ChangeLayerMask(InteractionLayerMask mask)
    {
        interactionLayers = mask;
    }
}
