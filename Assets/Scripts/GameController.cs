using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private int[] nodeOwnership;
    // The following are made public for now in levelBuilder
    private GameObject[] nodes;
    private GameObject[,] connectionLines;

    // Start is called before the first frame update
    // void Start()
    // {
    // }

    public void SetNodesLines(GameObject[] nodes, GameObject[,] connectionLines)
    {
        this.nodes = nodes;
        this.connectionLines = connectionLines;
        nodeOwnership = new int[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeOwnership[i] = 0;
        }
        Debug.Log("Nodes and Lines set");
        Debug.Log("Nodes: " + nodes.Length);
    }

    // for use by node controller
    // when a node is clicked, it's node controller will call this function
    public void reportClick(GameObject node)
    {
        // add code here to handle click
        // check if node is owned by player
    }

    // for use by line controller
    // when an attack makes it all the way, the line controller will call this function with the reached node as a paramater
    public void reportCapture(GameObject node)
    {
        // add code here to handle captur
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
