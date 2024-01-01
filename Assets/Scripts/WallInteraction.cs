using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
class GeneratedColumn
{
    [SerializeField]
    private int columnIndex;
    [SerializeField]
    private GameObject[] wallObjects;
    [SerializeField]
    private bool isSocketed;

    private const string COLUMN_NAME = "Column";

    private Transform parentObject;
    private Transform columnObject;
    private bool isParented;

    public void InitializeColumn(Transform parent, int index, int rows, bool isSocketed)
    {
        parentObject = parent;
        columnIndex = index;
        wallObjects = new GameObject[rows];
        this.isSocketed = isSocketed;
    }

    public void SetCube(GameObject cube)
    {
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (!isParented)
            {
                isParented = true;
                cube.name = COLUMN_NAME + columnIndex;
                cube.transform.SetParent(parentObject);
                columnObject = cube.transform;
            }
            else
            {
                cube.transform.SetParent(columnObject);
            }

            if (wallObjects[i] == null)
            {
                wallObjects[i] = cube;
                break;
            }
        }
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallObjects.Length; i++)
        {
            Object.DestroyImmediate(wallObjects[i]);
        }
        wallObjects = null;
    }

    public void DestroyColumn(int power)
    {
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (wallObjects[i] != null)
            {
                Rigidbody rb = wallObjects[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
                wallObjects[i].transform.SetParent(parentObject);
                rb.AddRelativeForce(Random.onUnitSphere * power);
            }
        }
    }

    public void ActivateColumn()
    {
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (wallObjects[i] != null)
            {
                Rigidbody rb = wallObjects[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }

    public void ResetColumn()
    {
        for (int i = 0; i < wallObjects.Length; i++)
        {
            if (wallObjects[i] != null)
            {
                Rigidbody rb = wallObjects[i].GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }
        }
    }
}

[ExecuteAlways]
public class WallInteraction : MonoBehaviour
{
    [Header("Initialisation")]
    [SerializeField]
    private GameObject wallCubePrefab;
    [SerializeField] 
    private GameObject socketWallPrefab;
    [SerializeField]
    private int socketPosition = 1;
    [SerializeField]
    private int columns;
    [SerializeField]
    private int rows;

    [Header("Generating")]
    [SerializeField]
    private List<GeneratedColumn> generatedWalls;
    [SerializeField]
    private float cubeSpacing = 0.005f;

    [Header("Interaction")]
    [SerializeField]
    private ExplosiveDevice expDevice;
    [SerializeField]
    private bool buildWall;
    [SerializeField]
    private bool deleteWall;
    [SerializeField]
    private bool destroyWall; // blow up Wall
    [SerializeField]
    private int maxPower;

    [Header("Events")]
    [SerializeField]
    private UnityEvent OnDestroy; 

    private XRSocketInteractor wallSocket;
    private GameObject[] wallObjects;
    private Vector3 cubeSize;
    private Vector3 spawnPosition;

    private void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnSocketEnter);
            wallSocket.selectExited.AddListener(OnSocketExit);
        }

        if (expDevice != null)
        {
            expDevice.OnDetonated.AddListener(OnDestroyWall);
        }
    }

    private void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }

        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < generatedWalls.Count; i++)
            {
                generatedWalls[i].DeleteColumn();
            }
            generatedWalls.Clear();
        }
    }

    private void BuildWall()
    {
        cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        spawnPosition = transform.position;

        int socketedColumn = Random.Range(0, columns);
        for (int i = 0; i < columns; i++)
        {
            if (i  == socketedColumn)
                GenerateColumn(i, rows, true);
            else
                GenerateColumn(i, rows, false);
            spawnPosition.x += cubeSize.x + cubeSpacing;
            spawnPosition.y = transform.position.y;
        }
    }

    private void GenerateColumn(int index, int height, bool isSocketed)
    {
        GeneratedColumn genColumn = new GeneratedColumn();
        genColumn.InitializeColumn(transform, index, height, isSocketed);

        wallObjects = new GameObject[height];
        for (int i = 0; i < height; i++)
        {
            if (i == socketPosition && isSocketed && socketWallPrefab != null)
            {
                wallObjects[i] = Instantiate(socketWallPrefab, spawnPosition, transform.rotation);
                genColumn.SetCube(wallObjects[i]);

                wallSocket = wallObjects[i].GetComponentInChildren<XRSocketInteractor>();
                if (wallSocket != null)
                {
                    wallSocket.selectEntered.AddListener(OnSocketEnter);
                    wallSocket.selectExited.AddListener(OnSocketExit);
                }
            }

            else if (wallCubePrefab != null)
            {
                wallObjects[i] = Instantiate(wallCubePrefab, spawnPosition, transform.rotation);
                genColumn.SetCube(wallObjects[i]);
            }

            if (wallObjects[i] != null)
            {
                Rigidbody rb = wallObjects[i].GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }
            spawnPosition.y += cubeSize.y + cubeSpacing;
        }        
        generatedWalls.Add(genColumn);
    }

    private void AddSocketWall(GeneratedColumn socketedColumn)
    {
        if (wallObjects[socketPosition] != null)
        {
            Vector3 position = wallObjects[socketPosition].transform.position;
            DestroyImmediate(wallObjects[socketPosition]);
            wallObjects[socketPosition] = Instantiate(socketWallPrefab, position, transform.rotation);
            socketedColumn.SetCube(wallObjects[socketPosition]);

            wallSocket = wallObjects[socketPosition].GetComponentInChildren<XRSocketInteractor>();
            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(OnSocketEnter);
                wallSocket.selectExited.AddListener(OnSocketExit);
            }
        }
    }

    private void OnDestroyWall()
    {
        if (generatedWalls.Count > 0)
        {
            for (int i = 0; i < generatedWalls.Count; i++)
            {
                int power = Random.Range((maxPower / 2), maxPower);
                generatedWalls[i].DestroyColumn(power);
            }
        }
        OnDestroy?.Invoke();
    }

    private void OnSocketEnter(SelectEnterEventArgs arg0)
    {
        if (generatedWalls.Count > 0)
            for (int i = 0; i < generatedWalls.Count; i++)
                generatedWalls[i].ActivateColumn();
    }

    private void OnSocketExit(SelectExitEventArgs arg0)
    {
        if (generatedWalls.Count > 0)
        {
            for (int i = 0; i < generatedWalls.Count; i++)
            {
                generatedWalls[i].ResetColumn();
            }
        }
    }
}
