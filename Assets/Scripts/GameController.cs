using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // 0 - Not owned by anyone, 1 - Owned by the player, 2+ - Owned by the CPU 
    private int[] nodeOwnership;
    // The following are made public for now in levelBuilder
    private GameObject[] nodes;
    private GameObject[,] connectionLines;

    LevelBuilder levelBuilder;
    private GameObject background;
    
    // nodeSelected is the node that the player has selected to move from
    private GameObject selectedNode = null;
    // targetSelected is the node that the player has selected to move to
    private GameObject targetNode = null;
    // neighbors is the list of nodes that are neighbors to the selected node
    private GameObject[] targetNeighbors;
    // targetOriginalShape is the actual shape of the target node 
    private Sprite targetOriginalShape;


    void Start(){
        levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        background = GameObject.Find("Background");
    }

    public void SetNodesLines(GameObject[] nodes, GameObject[,] connectionLines)
    {
        this.nodes = nodes;
        this.connectionLines = connectionLines;
        nodeOwnership = new int[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeOwnership[i] = 0;
        }
        // Debug.Log("Nodes and Lines set");
        // Debug.Log("Nodes: " + nodes.Length);
    }

    // for use by node controller
    // when a node is clicked, it's node controller will call this function
    public void reportClick(GameObject node)
    {

        if (selectedNode == null){
            if (IsOwned(node)){
                selectedNode = node;
                updateTargetNeighbors();
                pulseAll();
            }
        } else {
            if (targetNode == null){
                if (node == selectedNode){  // If the player clicks the selected node again, deselect it
                    pulseAll();
                    selectedNode = null;
                    targetNeighbors = null;
                } else if (node == background){ // If the player clicks another node that they own, change the selected node
                    pulseAll();
                    selectedNode = null;
                    targetNeighbors = null;
                } else if (IsOwned(node)){
                    pulseAll();
                    selectedNode = node;
                    updateTargetNeighbors();
                    pulseAll();
                } else {
                    targetNode = node;
                    targetOriginalShape = targetNode.GetComponent<SpriteRenderer>().sprite;
                    node.GetComponent<NodeController>().toggleShape();
                }
            } else { // If both selectedNode and targetNode are selected
                if (node == selectedNode){  // If the player clicks the selected node again, deselect everything
                    pulseAll();
                    resetTargetShape();
                    targetNode = null;
                    selectedNode = null;
                    targetNeighbors = null;
                    targetOriginalShape = null;
                } else if (node == background){ // If the player clicks another node that they own, change the selected node
                    initiateAttack();
                    pulseAll();
                    resetTargetShape();
                    targetNode = null;
                    selectedNode = null;
                    targetNeighbors = null;
                    targetOriginalShape = null;
                } else if (IsOwned(node)){ // If the player clicks another node that they own, start the attack and then change the selected node
                    initiateAttack();
                    pulseAll();
                    resetTargetShape();
                    targetNode = null;
                    selectedNode = node;
                    updateTargetNeighbors();
                    targetOriginalShape = null;
                } else if (node == targetNode){ // If the player clicks on the target node, toggle the shape
                    node.GetComponent<NodeController>().toggleShape();
                } else { // If the player clicks on another possible target node, change targets.
                    resetTargetShape();
                    targetNode = node;
                    targetOriginalShape = targetNode.GetComponent<SpriteRenderer>().sprite;
                    node.GetComponent<NodeController>().toggleShape();
                }
            }
        }   
    }

    // Helper functions for handling node clicks//

    private void initiateAttack(){
        Debug.Log("Initiating attack");
        // Implement attack logic here
        LineController lineController = levelBuilder.GetConnectionController(selectedNode, targetNode);
        lineController.StartAttack(selectedNode.transform, Color.red, targetNode.GetComponent<NodeController>().GetTransientShape());
        Debug.Log("Attack initiated, hoping for shape " + targetNode.GetComponent<NodeController>().GetTransientShape());
    }

    private void launchAttack(GameObject node){
        // updateTargetNeighbors(node);
        // pulseAll();
    }

    private void resetTargetShape(){
        targetNode.GetComponent<SpriteRenderer>().sprite = targetOriginalShape;
    }

    // Assumes that a node is selected and its neighbors have already been updated for the current selected Node
    private void pulseAll(){
        selectedNode.GetComponent<NodeController>().activatePulse();
        for (int i = 0; i < targetNeighbors.Length; i++){
            targetNeighbors[i].GetComponent<NodeController>().activatePulse();
        }
    }

    private void updateTargetNeighbors(){
        List<GameObject> tmpNeighbors = new List<GameObject>();
        GameObject[] neighbors = levelBuilder.FindNeighbors(selectedNode);
        for (int i = 0; i < neighbors.Length; i++){
            if (!IsOwned(neighbors[i])){
                tmpNeighbors.Add(neighbors[i]);
            }
        }
        targetNeighbors = tmpNeighbors.ToArray();
    }

    public int GetNodeOwnership(GameObject node)
    {
        int nodeIndex = levelBuilder.NodeToIndex(node);
        return nodeOwnership[nodeIndex];
    }

    // node - the node to be updated, player - the player who now owns the node 
    public void UpdateOwnership(GameObject node, int player, int shape)
    {
        int nodeIndex = levelBuilder.NodeToIndex(node);
        nodeOwnership[nodeIndex] = player;
        node.GetComponent<NodeController>().changeColor(player);
        node.GetComponent<NodeController>().setShape(shape);
    }

    private bool IsOwned(GameObject node){
        if (node == background) {
            return false; 
        }
        int nodeIndex = levelBuilder.NodeToIndex(node);
        return nodeOwnership[nodeIndex] == 1; 
    }
}
