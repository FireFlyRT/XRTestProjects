using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HighlightControll : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractable _interactableObject;
    [SerializeField]
    private Material _startMat;
    [SerializeField]
    private Material _emmissionMat;
    [SerializeField]
    private Renderer _highlightableObject;

    private void OnEnable()
    {
        if (_interactableObject != null)
        {
            _interactableObject.selectEntered.AddListener(HighlightObject);
            _interactableObject.selectExited.AddListener(ResetObject);
        }
    }

    private void OnDisable()
    {
        if (_interactableObject != null)
        {
            _interactableObject.selectEntered.RemoveListener(HighlightObject);
            _interactableObject.selectExited.RemoveListener(ResetObject);
        }
    }

    private void HighlightObject(SelectEnterEventArgs arg0)
    {
        if (_highlightableObject != null && _startMat != null)
            _highlightableObject.material = _emmissionMat;
    }

    private void ResetObject(SelectExitEventArgs arg0)
    {
        if (_highlightableObject != null && _startMat != null)
            _highlightableObject.material = _startMat;
    }
}
