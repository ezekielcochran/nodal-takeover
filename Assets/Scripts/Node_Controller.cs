using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Node_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create Nodes as Children
        GameObject NodeContainer = GameObject.Find("Nodes");

        // If "Nodes" doesn't exist, handle the error or return null
        if (NodeContainer == null)
        {
            Debug.LogError("Nodes game object not found!");
        }

        GameObject prefab = (GameObject)Resources.Load("Node", typeof(GameObject));

        if (prefab == null)
        {
            Debug.LogError("Nodes game object not found!");
        }


        // Generate Coordinates for Nodes
        float[][] generatedCoordinates = GenerateCoordinates();

        // Create the nodes to operate on
        List<GameObject> NodeList = new List<GameObject>();

        // Add objects to the list:
        for (int i = 0; i < 16; i++){
            NodeList.Add(Instantiate(prefab, NodeContainer.transform.position, NodeContainer.transform.rotation, NodeContainer.transform));
            NodeList[i].GetComponent<Transform>().position = new Vector3(generatedCoordinates[i][0], generatedCoordinates[i][1],0);
        }
        // NodeList.Add(Instantiate(prefab, NodeContainer.transform.position, NodeContainer.transform.rotation, NodeContainer.transform));
        // NodeList.Add(GameObject.Find("Object2"));

        // Access elements:
        GameObject firstObject = NodeList[0];

        // Get count of elements:
        int count = NodeList.Count;



        // GameObject n1 = Instantiate(prefab, NodeContainer.transform.position, NodeContainer.transform.rotation, NodeContainer.transform);
        // GameObject n2 = Instantiate(prefab, NodeContainer.transform.position, NodeContainer.transform.rotation, NodeContainer.transform);

        // // Optionally, set other properties of the new object
        // n1.GetComponent<Transform>().position = new Vector3(0f, 1f, 0);
        Debug.Log("Nodes placed");
    }

    public float[][] GenerateCoordinates(int numNodes = 16, float boundsX = 5f, float boundsY = 5f)
    {
        
        float[][] coordinates = new float[numNodes][];
        float rowColumnRatio = boundsY / boundsX;

        int cols = (Mathf.FloorToInt(Mathf.Sqrt(numNodes)) * 2) -1;
        int col_split = Mathf.FloorToInt(Mathf.Sqrt(numNodes))-1;
        int rows;
        float coordX;
        float coordY;
        int index = 0;

        for (int col = col_split * -1; col <= col_split; col++)
        {
            coordX = (col / col_split) * boundsX;

            // Calculate the number of rows for this column
            rows = (col_split + 1) - (Mathf.Abs(col) * -1);

            for (int row = 1; row <= rows; row++){
                float ratio = 1 / (rows + 1);
                coordY = (ratio * row * 2 * boundsY) - boundsY;
                coordinates[index] = new float[] {coordX, coordY};
                Debug.Log(coordY);
                Debug.Log(coordX);

            }

            index++ ;
        }

        return coordinates;
    }

    private float[][] GenerateCoordinates2(int numNodes = 18, float boundsX = 0f, float boundsY = 0f)
    {
        float[][] coordinates = new float[numNodes][];
        float radius = Mathf.Sqrt(numNodes / Mathf.PI) * 2.5f; // Calculate radius based on number of nodes

        for (int i = 0; i < numNodes; i++)
        {
            // Calculate angles for even distribution
            float angle = 2 * Mathf.PI * i / numNodes;

            // Calculate tapered radius based on angle
            float taperedRadius = radius * (1 - Mathf.Abs(Mathf.Sin(angle)));

            // Calculate coordinates within bounds
            float x = boundsX / 2 + taperedRadius * Mathf.Cos(angle);
            float y = boundsY / 2 + taperedRadius * Mathf.Sin(angle);

            coordinates[i] = new float[] { x, y };
        }

        return coordinates;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
