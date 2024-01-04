using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRWandControll : XRGrabInteractable
{
    [Header("Wand Controlls")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform spawnPoint;

    private bool isFiring;

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        if (projectilePrefab != null )
        {
            Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
    }
}
