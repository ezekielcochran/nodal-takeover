using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Node_Creator : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject prefab;
    void Start()
    {
        // Create Nodes as Children
        GameObject NodeContainer = GameObject.Find("Nodes");

        // If "Nodes" doesn't exist, handle the error or return null
        if (NodeContainer == null)
        {
            Debug.LogError("Nodes game object not found!");
        }

        // GameObject prefab = (GameObject)Resources.Load("Node", typeof(GameObject));

        // if (prefab == null)
        // {
        //     Debug.LogError("Nodes prefab not found!");
        // }

        // Generate Coordinates for Nodes
        float[][] generatedCoordinates = GenerateCoordinates();

        // Create the nodes to operate on
        List<GameObject> NodeList = new List<GameObject>();

        Debug.Log(transform.position.x);

        // Add objects to the list:
        for (int i = 0; i < generatedCoordinates.Length; i++){
            NodeList.Add(Instantiate(prefab, NodeContainer.transform.position, NodeContainer.transform.rotation, NodeContainer.transform));
            NodeList[i].GetComponent<Transform>().position = new Vector3(generatedCoordinates[i][0], generatedCoordinates[i][1],0);
        }

        // Get count of elements:
        int count = NodeList.Count;

        // Optionally, set other properties of the new object
        // n1.GetComponent<Transform>().position = new Vector3(0f, 1f, 0);
        Debug.Log("Nodes placed. Drawing  Lines.");

        // Add LinesRenders
        for (int i = 0; i < NodeList.Count; i++){
            GameObject currNode = NodeList[i];
            LineRenderer currLR = currNode.GetComponent<LineRenderer>();
            currLR.positionCount = NodeList.Count - i;
            for (int j = 0; j < NodeList.Count - i; j++){
                GameObject nextNode = NodeList[i];
                Vector3 pos = nextNode.transform.position;
                
            }
        }
    }

    public float[][] GenerateCoordinates(int numNodes = 25, float boundsX = 7f, float boundsY = 5f)
    {
        float[][] coordinates = new float[numNodes][];

        // float cols = (Mathf.FloorToInt(Mathf.Sqrt(numNodes)) * 2) -1;
        // float col_split = Mathf.FloorToInt(Mathf.Sqrt(numNodes))-1;
        float col_split = Mathf.Sqrt(numNodes) -1;

        float rows;
        float coordX;
        float coordY;
        int index = 0;

        for (float col = col_split * -1; col <= col_split; col++)
        {
            coordX = (col / col_split) * boundsX;

            // Calculate the number of rows for this column
            rows = ((col_split + 1) - (Mathf.Abs(col)));

            for (int row = 1; row <= rows; row++){
                float ratio = 1 / (rows + 1);
                coordY = (ratio * row * 2 * boundsY) - boundsY;
                coordinates[index] = new float[] {coordX, coordY};
                index++ ;
            }            
        }
        return coordinates;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
