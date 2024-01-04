using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Grabbables")]
    [SerializeField]
    private XRGrabInteractable[] grabInteractables;
    [SerializeField]
    private AudioSource grabSource;
    [SerializeField]
    private AudioClip grabClip;
    [SerializeField]
    private AudioClip keyClip;
    [SerializeField]
    private AudioSource activatedSource;
    [SerializeField]
    private AudioClip grabActivatedClip;
    [SerializeField]
    private AudioClip wandActivatedClip;

    [Header("Drawer")]
    [SerializeField]
    private DrawerInteractable drawer;

    private XRSocketInteractor drawerSocket;
    private AudioSource drawerSource;
    private AudioSource drawerSocketSource;
    private AudioClip drawerMoveClip;
    private AudioClip drawerSocketClip;

    [Header("Hinge")]
    [SerializeField]
    private SimpleHinchInteractable[] cabinatDoors = new SimpleHinchInteractable[2];

    private AudioSource[] cabinatDoorsSource;
    private AudioClip cabinatDoorsMoveClip;

    [Header("Combo Lock")]
    [SerializeField]
    private CombinationLock combinationLock;

    private AudioSource combinationLockSource;
    private AudioClip lockClip;
    private AudioClip unlockClip;
    private AudioClip comboButtonPressedClip;

    [Header("Wall")]
    [SerializeField]
    private WallInteraction wall;
    [SerializeField]
    private AudioSource wallSource;

    private XRSocketInteractor wallSocket;
    private AudioSource wallSocketSource;
    private AudioClip wallClip;
    private AudioClip wallSocketClip;

    [Header("Default")]
    [SerializeField]
    private AudioClip fallbackClip;
    private const string FALLBACK_CLIP_NAME = "fallBackClip";

    private void SetGrabbables()
    {
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void SetDrawerInteractable()
    {
        drawerSource = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetDrawerMoveClip;

        CheckClip(ref drawerMoveClip);
        drawerSource.clip = drawerMoveClip;
        drawerSource.loop = true;

        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);

        if (drawerSocket != null)
        {
            drawerSocketSource = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocketClip = drawer.GetSocketedClip;
            CheckClip(ref drawerSocketClip);
            drawerSocketSource.clip = drawerSocketClip;
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
        }
    }

    private void SetWall()
    {
        wallClip = wall.GetDestroyClip;
        CheckClip(ref wallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);
        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocketSource = wallSocket.transform.AddComponent<AudioSource>();
            wallSocketClip = wall.GetSocketClip;
            CheckClip(ref wallSocketClip);
            wallSocketSource.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void SetCabintDoors(int index)
    {
        cabinatDoorsSource[index] = cabinatDoors[index].transform.AddComponent<AudioSource>();
        cabinatDoorsMoveClip = cabinatDoors[index].GetHingeMoveClip;
        CheckClip(ref cabinatDoorsMoveClip);
        cabinatDoorsSource[index].clip = cabinatDoorsMoveClip;
        cabinatDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        cabinatDoors[index].selectExited.AddListener(OnDoorStop);
    }

    private void SetComboLock()
    {
        combinationLockSource = combinationLock.transform.AddComponent<AudioSource>();
        lockClip = combinationLock.GetLockClip;
        CheckClip(ref lockClip);
        unlockClip = combinationLock.GetUnlockClip;
        CheckClip(ref unlockClip);
        comboButtonPressedClip = combinationLock.GetComboButtonPressedClip;
        CheckClip(ref  comboButtonPressedClip);

        combinationLock.UnlockAction += OnComboUnlocked;
        combinationLock.LockAction += OnComboLocked;
        combinationLock.ComboButtonPressed += OnComboButtonPressed;
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        wallSocketSource.Play();
    }

    private void CheckClip(ref AudioClip clip)
    {
        if (clip == null)
            clip = fallbackClip;
    }

    private void OnDestroyWall()
    {
        if (wallSource != null)
            wallSource.Play();
    }

    private void OnSelectEnterGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("key"))
            grabSource.clip = keyClip;
        else
            grabSource.clip = grabClip;

        grabSource.Play();
    }

    private void OnSelectExitedGrabbable(SelectExitEventArgs arg0)
    {
        grabSource.clip = grabClip;
        grabSource.Play();
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        GameObject interactable = arg0.interactableObject.transform.gameObject;
        if (interactable.GetComponent<XRWandControll>() != null)
            activatedSource.clip = wandActivatedClip;
        else
            activatedSource.clip = grabActivatedClip;

        activatedSource.Play();
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        drawerSocketSource.Play();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSource.Play();
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSource.Stop();
    }

    private void OnDoorMove(SimpleHinchInteractable arg0)
    {
        for (int i = 0; i < cabinatDoors.Length; i++)
            if (arg0 == cabinatDoors[i])
                cabinatDoorsSource[i].Play();
    }

    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinatDoors.Length; i++)
            if (arg0.interactableObject == cabinatDoors[i])
                cabinatDoorsSource[i].Stop();
    }

    private void OnComboUnlocked()
    {
        combinationLockSource.clip = unlockClip;
        combinationLockSource.Play();
    }

    private void OnComboLocked()
    {
        combinationLockSource.clip = lockClip;
        combinationLockSource.Play();
    }

    private void OnComboButtonPressed()
    {
        combinationLockSource.clip = comboButtonPressedClip;
        combinationLockSource.Play();
    }

    private void OnEnable()
    {
        if (fallbackClip == null)
            fallbackClip = AudioClip.Create(FALLBACK_CLIP_NAME, 1, 1, 1000, true);
        
        SetGrabbables();

        if (drawer != null)
            SetDrawerInteractable();

        cabinatDoorsSource = new AudioSource[cabinatDoors.Length];
        for (int i = 0; i < cabinatDoors.Length; i++)
            if (cabinatDoors[i] != null)
                SetCabintDoors(i);

        if (combinationLock != null)
            SetComboLock();

        if (wall != null)
            SetWall();
    }

    private void OnDisable()
    {
        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }
    }
}
