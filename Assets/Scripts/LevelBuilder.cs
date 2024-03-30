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

    // Start is called before the first frame update
    void Start()
    {
        // Get the LevelElements object to parent the nodes to
        GameObject LevelElements = GameObject.Find("Level Elements"); // maybe handle error if object missing
        // Get the level data
        LevelData levelData = getLevelData(levelNumber);
        Debug.Log(levelData);

        // Create and draw nodes
        GameObject[] Nodes = new GameObject[levelData.nodeCount];
        for (int i = 0; i < levelData.nodeCount; i++)
        {
            GameObject newNode = Instantiate(nodePrefab, LevelElements.transform.position, LevelElements.transform.rotation, LevelElements.transform);
            newNode.transform.position = Camera.main.ViewportToWorldPoint(levelData.nodePositions[i]); // This leaves the z coordinate at -10 (where the camera is);
            newNode.transform.position = new Vector3(newNode.transform.position.x, newNode.transform.position.y, 0); // set z to 0
            Nodes[i] = newNode;
        }

        // Create and draw connections
        GameObject[,] Connections = new GameObject[levelData.nodeCount, levelData.nodeCount];
        // List<GameObject> Connections = new List<GameObject>();
        for (int i = 0; i < levelData.nodeCount; i++)
        {
            for (int j = i + 1; j < levelData.nodeCount; j++)
            {
                if (levelData.nodeConnections[i, j] == 1)
                {
                    GameObject newConnection = Instantiate(lineControllerPrefab, LevelElements.transform.position, LevelElements.transform.rotation, LevelElements.transform);
                    // newConnection.DrawSegment(Nodes[i].transform, Nodes[j].transform);
                    // lineController.DrawSegment(Nodes[i].transform, Nodes[j].transform);
                    newConnection.GetComponent<LineController>().DrawSegment(Nodes[i].transform, Nodes[j].transform);
                    Connections[i, j] = newConnection;
                    // Debug.Log("Drawing a connection " + i + " to " + j);
                }
            }
        }
    }

    // This function reads the level data from a file into a struct, leaving values as in the file. For example, positions are left in viewport coordinates (from 0 to 1)
    // However, nodes are 0-based in the array, so they must be decremented by 1 when used as indices
    LevelData getLevelData(int level)
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
                if (splitLine[0] == "nc") {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}