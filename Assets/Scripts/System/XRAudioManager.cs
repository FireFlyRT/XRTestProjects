using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] 
    private ProgressControll progressControll;
    [SerializeField]
    private AudioSource progressSource;
    [SerializeField]
    private AudioClip startGameClip;
    [SerializeField]
    private AudioClip challangeCompleteClip;

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
    private XRPhisicsButtonInteractable drawerPhisicsButton;
    private AudioSource drawerSource;
    private AudioSource drawerSocketSource;
    private AudioClip drawerMoveClip;
    private AudioClip drawerSocketClip;
    private bool _isDetached;

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

    [Header("JoyStick_Robot")]
    [SerializeField]
    private SimpleHinchInteractable _joystick;
    [SerializeField]
    private NavMeshRobot robot;
    
    private AudioSource _joystickSource;
    private AudioClip _joystickClip;
    private AudioSource _destroyWallCubeSource;
    private AudioClip _destroyWallCubeClip;

    [Header("Default")]
    [SerializeField]
    private AudioClip fallbackClip;
    [SerializeField]
    private AudioSource backgroundMusic;
    [SerializeField]
    private AudioClip backgroundMusicClip;
    private const string FALLBACK_CLIP_NAME = "fallBackClip";

    private bool startAudio;

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
        drawer.OnDrawerDetatch.AddListener(OnDrawerDetatch);

        if (drawerSocket != null)
        {
            drawerSocketSource = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocketClip = drawer.GetSocketedClip;
            CheckClip(ref drawerSocketClip);
            drawerSocketSource.clip = drawerSocketClip;
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
        }

        drawerPhisicsButton = drawer.GetPhysicsButton;
        if (drawerPhisicsButton != null)
        {
            drawerPhisicsButton.OnBaseEnter.AddListener(OnPhysicsButtonEnter);
            drawerPhisicsButton.OnBaseExit.AddListener(OnPhysicsButtonExit);
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

    private void SetJoystick()
    {
        _joystickClip = _joystick.GetHingeMoveClip;
        _joystickSource = _joystick.transform.AddComponent<AudioSource>();
        _joystickSource.clip = _joystickClip;
        _joystickSource.loop = true;

        _joystick.OnHingeSelected.AddListener(JoystickMove);
        _joystick.selectExited.AddListener(JoystickExited);
    }

    private void SetRobot()
    {
        _destroyWallCubeSource = robot.transform.AddComponent<AudioSource>();
        _destroyWallCubeClip = robot.GetCollisionClip;
        _destroyWallCubeSource.clip = _destroyWallCubeClip;

        robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
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
        if (arg0.interactableObject.transform.CompareTag("Key"))
            PlayGrabSound(keyClip);
        else
            PlayGrabSound(grabClip);
    }

    private void OnSelectExitedGrabbable(SelectExitEventArgs arg0)
    {
        PlayGrabSound(grabClip);
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
        if (_isDetached)
            PlayGrabSound(grabClip);
        else
            drawerSource.Play();
    }

    private void OnDrawerDetatch()
    {
        _isDetached = true;
        drawerSource.Stop();
    }

    private void OnPhysicsButtonEnter()
    {
        PlayGrabSound(keyClip);
    }

    private void OnPhysicsButtonExit()
    {
        PlayGrabSound(keyClip);
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
            if ((SimpleHinchInteractable)arg0.interactableObject == cabinatDoors[i])
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

    private void OnDestroyWallCube()
    {
        _destroyWallCubeSource.Play();
    }

    private void JoystickMove(SimpleHinchInteractable arg0)
    {
        _joystickSource.Play();
    }

    private void JoystickExited(SelectExitEventArgs arg0)
    {
        _joystickSource.Stop();
    }

    private void PlayGrabSound(AudioClip clip)
    {
        grabSource.clip = clip;
        grabSource.Play();
    }

    private void StartGame(string arg0)
    {
        if (!startAudio)
        {
            startAudio = true;
            if (backgroundMusic != null && backgroundMusicClip != null)
            {
                backgroundMusic.clip = backgroundMusicClip;
                backgroundMusic.Play();
            }
        }
        else
        {
            if (progressSource != null && startGameClip != null)
            {
                progressSource.clip = startGameClip;
                progressSource.Play();
            }
        }
    }

    private void ChallangeComplete(string arg0)
    {
        if (progressSource != null && challangeCompleteClip != null)
        {
            progressSource.clip = challangeCompleteClip;
            progressSource.Play();
        }
    }

    private void OnEnable()
    {
        if (progressControll != null)
        {
            progressControll.OnStartGame.AddListener(StartGame);
            progressControll.OnChallengeComplete.AddListener(ChallangeComplete);
        }

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

        if (_joystick != null)
            SetJoystick();

        if (robot != null)
            SetRobot();
    }

    private void OnDisable()
    {
        if (progressControll != null)
        {
            progressControll.OnStartGame.RemoveListener(StartGame);
            progressControll.OnChallengeComplete.RemoveListener(ChallangeComplete);
        }

        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.RemoveListener(OnSelectEnterGrabbable);
            grabInteractables[i].selectExited.RemoveListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.RemoveListener(OnActivatedGrabbable);
        }

        if (drawer != null)
        {
            drawer.selectEntered.RemoveListener(OnDrawerMove);
            drawer.selectExited.RemoveListener(OnDrawerStop);
            drawer.OnDrawerDetatch.RemoveListener(OnDrawerDetatch);
        }

        for (int i = 0; i < cabinatDoors.Length; i++)
        {
            cabinatDoors[i].OnHingeSelected.RemoveListener(OnDoorMove);
            cabinatDoors[i].selectExited.RemoveListener(OnDoorStop);
        }

        if (combinationLock != null)
        {
            combinationLock.UnlockAction -= OnComboUnlocked;
            combinationLock.LockAction -= OnComboLocked;
            combinationLock.ComboButtonPressed -= OnComboButtonPressed;
        }

        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }

        if (wallSocket != null)
        {
            wallSocket.selectEntered.RemoveListener(OnWallSocketed);
        }
    }
}
