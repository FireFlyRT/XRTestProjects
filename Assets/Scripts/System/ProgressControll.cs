using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ProgressControll : MonoBehaviour
{
    [Header("StartButton")]
    [SerializeField]
    private XRButtonInteractable startButton;
    [SerializeField]
    private GameObject keyIndecatorLight;

    [Header("Challenge Settings")]
    [SerializeField]
    private string startGameString;
    [SerializeField]
    private string endGameString;
    [SerializeField]
    private string[] challengeStrings;
    [SerializeField]
    private int _wallCubesToDestroy;

    private int _wallCubesDestroyed;

    [Header("Drawer")]
    [SerializeField]
    private DrawerInteractable drawer;

    private XRSocketInteractor drawerSocket;

    [Header("Combo Lock")]
    [SerializeField]
    private CombinationLock combinationLock;

    [Header("Wall")]
    [SerializeField]
    private WallInteraction wall;
    [SerializeField]
    private GameObject teleportationAreas;

    [Header("Library")]
    [SerializeField]
    private SimpleSliderControll librarySlider;

    private XRSocketInteractor wallSocket;

    [Header("Robot")]
    [SerializeField]
    private NavMeshRobot robot;

    [HideInInspector]
    public UnityEvent<string> OnStartGame;
    [HideInInspector]
    public UnityEvent<string> OnChallengeComplete;

    private bool _startGame;
    private bool _challengesCompleted;
    private int _challengeNumber;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(StartButtonPressed);
        }

        OnStartGame?.Invoke(startGameString);
        SetDrawerInteractable();
        if (combinationLock != null)
            combinationLock.UnlockAction += OnComboUnlocked;
        if (wall != null)
            SetWall();
        if (librarySlider != null)
            librarySlider.OnSliderActive.AddListener(LibrarySliderActive);
        if (robot != null)
            robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
    }

    private void ChallangeComplete()
    {
        _challengeNumber++;
        if (_challengeNumber < challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeStrings[_challengeNumber]);
        }
        else if (_challengeNumber >= challengeStrings.Length)
        {
            // All Challenges Complete
            OnChallengeComplete?.Invoke(endGameString);
        }
    }

    private void StartButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!_startGame)
        {
            _startGame = true;

            if (keyIndecatorLight != null)
            {
                keyIndecatorLight.SetActive(true);
            }
            if (_challengeNumber < challengeStrings.Length && _challengeNumber == 0)
            {
                OnStartGame?.Invoke(challengeStrings[_challengeNumber]);
            }
        }
    }

    private void SetDrawerInteractable()
    {
        if (drawer != null)
        {
            drawer.OnDrawerDetatch.AddListener(OnDrawerDetach);
            drawerSocket = drawer.GetKeySocket;
            if (drawerSocket != null)
            {
                drawer.selectEntered.AddListener(OnDrawerSocketed);
            }
        }
    }

    private void SetWall()
    {
        wall.OnDestroy.AddListener(OnDestroyWall);
        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
            wallSocket.selectEntered.AddListener(OnWallSocketed);
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        if (_challengeNumber == 0)
        {
            ChallangeComplete();
        }
    }

    private void OnDrawerDetach()
    {
        if (_challengeNumber == 1)
        {
            ChallangeComplete();
        }
    }    

    private void OnComboUnlocked()
    {
        if (_challengeNumber == 2)
        {
            ChallangeComplete();
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        if (_challengeNumber == 3)
        {
            ChallangeComplete();
        }
    }

    private void OnDestroyWall()
    {
        if (_challengeNumber == 4)
        {
            ChallangeComplete();
        }
        if (teleportationAreas != null)
            teleportationAreas.SetActive(true);
    }

    private void LibrarySliderActive()
    {
        if (_challengeNumber == 5)
        {
            ChallangeComplete();
        }
    }

    private void OnDestroyWallCube()
    {
        _wallCubesDestroyed++;
        if (_wallCubesDestroyed >= _wallCubesToDestroy && !_challengesCompleted)
        {
            _challengesCompleted = true;
            if (_challengeNumber == 6)
            {
                ChallangeComplete();
            }
        }
    }
}
