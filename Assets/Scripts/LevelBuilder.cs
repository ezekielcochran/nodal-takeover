using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private int levelNumber; // [SerializeField] forces Unity to allow initialization of a private variable within the Unity Editor
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject lineControllerPrefab;
    [SerializeField] private float attackSpeed = 0.5f;
    private GameObject[] nodes;
    private GameObject[,] connectionLines;

    // Start is called before the first frame update
    // It creates objects for nodes and connections based on the level data
    // It also initailizes the nodes and connections arrays
    // It could stand a little method extraction
    void Start()
    {
        Debug.Log("Level Builder Start");
        // Get the LevelElements object to parent the nodes to
        GameObject levelElements = GameObject.Find("Level Elements"); // maybe handle error if object missing
        // Get the level data
        LevelData levelData = GetLevelData(levelNumber);
        Debug.Log(levelData);

        // Create and draw nodes
        nodes = new GameObject[levelData.nodeCount];
        for (int i = 0; i < levelData.nodeCount; i++)
        {
            GameObject newNode = Instantiate(nodePrefab, levelElements.transform.position, levelElements.transform.rotation, levelElements.transform);
            newNode.transform.position = Camera.main.ViewportToWorldPoint(levelData.nodePositions[i]); // This leaves the z coordinate at -10 (where the camera is);
            newNode.transform.position = new Vector3(newNode.transform.position.x, newNode.transform.position.y, 0); // set z to 0
            nodes[i] = newNode;
        }

        // Create and draw connections
        connectionLines = new GameObject[levelData.nodeCount, levelData.nodeCount];
        // List<GameObject> Connections = new List<GameObject>();
        for (int i = 0; i < levelData.nodeCount; i++)
        {
            for (int j = i + 1; j < levelData.nodeCount; j++)
            {
                if (levelData.nodeConnections[i, j] == 1)
                {
                    GameObject newConnection = Instantiate(lineControllerPrefab, levelElements.transform.position, levelElements.transform.rotation, levelElements.transform);
                    // newConnection.DrawSegment(Nodes[i].transform, Nodes[j].transform);
                    // lineController.DrawSegment(Nodes[i].transform, Nodes[j].transform);
                    newConnection.GetComponent<LineController>().DrawBackgroundSegment(nodes[i].transform, nodes[j].transform);
                    connectionLines[i, j] = newConnection;
                    // Debug.Log("Drawing a connection " + i + " to " + j);
                }
            }
        }
        GameController gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        gameController.SetNodesLines(nodes, connectionLines);

        // Assign the start nodes for player and computer
        gameController.UpdateOwnership(nodes[0], 1, 1);
        gameController.UpdateOwnership(nodes[levelData.nodeCount - 1], 2, 2);
    }

    // Return the LineController object that connects the two input nodes
    public LineController GetConnectionController(GameObject node1, GameObject node2)
    {
        int node1Index = NodeToIndex(node1);
        int node2Index = NodeToIndex(node2);
        // This is necessary becuase the connectionLines matrix is upper triangular: accessing elements in the lower triangle will just return null
        if (node1Index < node2Index)
        {
            return connectionLines[node1Index, node2Index].GetComponent<LineController>();
        }
        else
        {
            return connectionLines[node2Index, node1Index].GetComponent<LineController>();
        }
    }

    // Return an array of GameObjects that are neighbors of the input node
    public GameObject[] FindNeighbors(GameObject node)
    {
        int nodeIndex = NodeToIndex(node);
        List<GameObject> neighbors = new List<GameObject>();
        for (int i = 0; i < nodes.Length; i++)
        {
            if ((connectionLines[nodeIndex, i] != null) || (connectionLines[i, nodeIndex] != null))
            {
                neighbors.Add(nodes[i]);
            }
        }
        return neighbors.ToArray();
    }

    // Return the index of a node based on its GameObject
    public int NodeToIndex(GameObject node)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] == node)
            {
                return i;
            }
        }
        return -1;
    }

    // This function reads the level data from a file into a struct, leaving values as in the file. For example, positions are left in viewport coordinates (from 0 to 1)
    // However, nodes are 0-based in the array, so they must be decremented by 1 when used as indices
    // Also, stream reader may not work for android builds, so this may need to be changed
    LevelData GetLevelData(int level)
    {
        // Application.dataPath points to the Assets folder in unity play mode, and Contents within the .app bundle in a build for mac
        // string filePath = Application.dataPath + "/Resources/LevelData/level" + level + ".txt"; // hard path: Don't move stuff!

        // This loads the text file from the Resources folder, which is included in the build
        // It then converts it to a stream for the StreamReader to read.
        // It may be easier to just iterate through the lines of the text file and process them as needed... check this later
        var textFile = Resources.Load<TextAsset>("LevelData/level" + level);
        byte[] byteArray = Encoding.UTF8.GetBytes(textFile.text);
        MemoryStream stream = new MemoryStream(byteArray);

        // if (!File.Exists(filePath)) {
        //     Debug.LogError("File not found: " + filePath + "... working directory: " + Directory.GetCurrentDirectory());
        //     LevelData failure = new()
        //     {
        //         level = -1,
        //         levelDescription = "Level " + level + " data file not found: " + filePath
        //     };
        //     return failure;
        // }

        LevelData result = new()
        {
            level = level
        };

        using (StreamReader reader = new StreamReader(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Process each line of the file here
                string[] splitLine = line.Split(null);
                if (splitLine[0] == "#" || line == "") {
                    // Skip comments and empty lines
                } else if (splitLine[0] == "nc") {
                    // Add node count and initialize positions array and connections matrix... I think this must happen before nodes and connections
                    result.nodeCount = int.Parse(splitLine[1]);
                    result.nodePositions = new Vector3[result.nodeCount];
                    // c# arrays (and I assume matrices) are initialized to 0
                    result.nodeConnections = new int[result.nodeCount, result.nodeCount];
                } else if (splitLine[0] == "ld") {
                    // Add level description
                    result.levelDescription = line.Remove(0, 3);
                } else if (splitLine[0] == "n") {
                    // Add position for new node
                    // The -1 is because the node numbers are 1-based in the file
                    result.nodePositions[int.Parse(splitLine[1]) - 1] = new Vector3(float.Parse(splitLine[2]), float.Parse(splitLine[3]), 0);
                } else if (splitLine[0] == "c") {
                    // Add connection between nodes
                    result.nodeConnections[int.Parse(splitLine[1]) - 1, int.Parse(splitLine[2]) - 1] = 1;
                } else {
                    Debug.LogWarning("Unrecognized line in level data: " + line);
                }
            }
        }
        return result;
    }

    private struct LevelData
    {
        public int level;
        public string levelDescription;
        public int nodeCount;
        public Vector3[] nodePositions;
        public int[,] nodeConnections; // uniform array
        public override readonly string ToString()
        {
            return "Level: " + level + " Description: " + levelDescription;
        }
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}