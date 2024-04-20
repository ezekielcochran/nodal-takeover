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

    //which player won
    private int winner = 0;

    //number of current attacks for each player
    private int[] attacks; 

    void Start(){
        levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        background = GameObject.Find("Background");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            GameObject testEnemyNode = nodes[nodes.Length - 1];
            GameObject[] neighbors = levelBuilder.FindNeighbors(testEnemyNode);
            LineController testLine = levelBuilder.GetConnectionController(testEnemyNode, neighbors[0]);
            testLine.StartAttack(testEnemyNode.transform, Color.blue, testEnemyNode.GetComponent<NodeController>().GetShape());
        }
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
            if (IsOwned(node, 1)){
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
                } else if (IsOwned(node, 1)){
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
                } else if (IsOwned(node, 1)){ // If the player clicks another node that they own, start the attack and then change the selected node
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
        LineController lineController = levelBuilder.GetConnectionController(selectedNode, targetNode);
        lineController.StartAttack(selectedNode.transform, selectedNode.GetComponent<NodeController>().GetColor(), targetNode.GetComponent<NodeController>().GetTransientShape());
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
            if (!IsOwned(neighbors[i], 1)){
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

    // This function needs to not only update the ownership of the node, but also clear all other outgoing attack lines, and any completed incoming ones
    public void ReportConquer(GameObject source, GameObject target, int shape){
        updateOwnership(target, GetNodeOwnership(source), shape);
        GameObject[] neighbors = levelBuilder.FindNeighbors(target);
        for (int i = 0; i < neighbors.Length; i++){
            if (neighbors[i] != source) {
                LineController lineController = levelBuilder.GetConnectionController(target, neighbors[i]);
                lineController.StopAttackCleanConnections(target.transform);
            }
        }
    }

    // node - the node to be updated, player - the player who now owns the node 
    public void updateOwnership(GameObject node, int player, int shape)
    {
        int nodeIndex = levelBuilder.NodeToIndex(node);
        nodeOwnership[nodeIndex] = player;
        node.GetComponent<NodeController>().changeColor(player);
        node.GetComponent<NodeController>().setShape(shape);
        // Debug.Log("In Game Controller, node " + nodeIndex + " is now owned by player " + player + " with shape " + shape);
        // Debug.Log("And node " + nodeIndex + " is reporting shape " + node.GetComponent<NodeController>().GetShape());

        // want to remove all 
    }

    private bool IsOwned(GameObject node, int playerNumber){
        if (node == background) {
            return false; 
        }
        int nodeIndex = levelBuilder.NodeToIndex(node);
        return nodeOwnership[nodeIndex] == playerNumber; 
    }


    IEnumerator StartComputerPlayer(int playerNumber, int maxAttacks) {
        GameObject node;
        List<GameObject> tmpNeighbors = new List<GameObject>();
        GameObject[] neighbors; 

        int nodeIndex = 0;
        while (winner != 0) {
            //if we can do another attack
            if (attacks[playerNumber] < maxAttacks) {
                //iterate over the nodes and find one that can attack
                for (nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++) {
                    //if it is owned by this cpu
                    if(nodeOwnership[nodeIndex] == playerNumber) {
                        node = nodes[nodeIndex];
                    }
                    else node = null;
                    if (node.GetComponent<NodeController>().isAttacking == false) {
                        neighbors = levelBuilder.FindNeighbors(node);
                        for (int i = 0; i < neighbors.Length; i++){
                            if (!IsOwned(neighbors[i], playerNumber)){
                                tmpNeighbors.Add(neighbors[i]);
                            }
                        }

                        //attack random neighbor
                        //increment attack count and find a new node that can attack
                        attacks[playerNumber] = attacks[playerNumber]+1;
                        break;
                    }
                    
                }
                
            }
        }
        yield return null;
    }
}
