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
    private int winner = -1;

    //number of current attacks for each player
    private int[] attacks = new int[5]; 
    private const float GLOAT_TIME = 3.0f;

    void Start(){
        levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        background = GameObject.Find("Background");
        Random.seed = (int)Time.deltaTime;
    }

    // void Update() {
    //     if (Input.GetKeyDown(KeyCode.A)) {
    //         GameObject testEnemyNode = nodes[nodes.Length - 1];
    //         GameObject[] neighbors = levelBuilder.FindNeighbors(testEnemyNode);
    //         LineController testLine = levelBuilder.GetConnectionController(testEnemyNode, neighbors[0]);
    //         testLine.StartAttack(testEnemyNode.transform, Color.blue, testEnemyNode.GetComponent<NodeController>().GetShape());
    //     }
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
        // Debug.Log("Nodes and Lines set");
        // Debug.Log("Nodes: " + nodes.Length);
    }

    // for use by node controller
    // when a node is clicked, it's node controller will call this function
    public void ReportClick(GameObject node)
    {

        if (selectedNode == null){
            if (IsOwned(node, 1)){
                selectedNode = node;
                UpdateTargetNeighbors();
                PulseAll();
            }
        } else {
            if (targetNode == null){
                if (node == selectedNode){  // If the player clicks the selected node again, deselect it
                    PulseAll();
                    selectedNode = null;
                    targetNeighbors = null;
                } else if (node == background){ // If the player clicks another node that they own, change the selected node
                    PulseAll();
                    selectedNode = null;
                    targetNeighbors = null;
                } else if (IsOwned(node, 1)){
                    PulseAll();
                    selectedNode = node;
                    UpdateTargetNeighbors();
                    PulseAll();
                } else {
                    targetNode = node;
                    targetOriginalShape = targetNode.GetComponent<SpriteRenderer>().sprite;
                    node.GetComponent<NodeController>().toggleShape();
                }
            } else { // If both selectedNode and targetNode are selected
                if (node == selectedNode){  // If the player clicks the selected node again, deselect everything
                    PulseAll();
                    ResetTargetShape();
                    targetNode = null;
                    selectedNode = null;
                    targetNeighbors = null;
                    targetOriginalShape = null;
                } else if (node == background){ // If the player clicks another node that they own, change the selected node
                    InitiateAttack();
                    PulseAll();
                    ResetTargetShape();
                    targetNode = null;
                    selectedNode = null;
                    targetNeighbors = null;
                    targetOriginalShape = null;
                } else if (IsOwned(node, 1)){ // If the player clicks another node that they own, start the attack and then change the selected node
                    InitiateAttack();
                    PulseAll();
                    ResetTargetShape();
                    targetNode = null;
                    selectedNode = node;
                    UpdateTargetNeighbors();
                    targetOriginalShape = null;
                } else if (node == targetNode){ // If the player clicks on the target node, toggle the shape
                    node.GetComponent<NodeController>().toggleShape();
                } else { // If the player clicks on another possible target node, change targets.
                    ResetTargetShape();
                    targetNode = node;
                    targetOriginalShape = targetNode.GetComponent<SpriteRenderer>().sprite;
                    node.GetComponent<NodeController>().toggleShape();
                }
            }
        }   
    }

    // Helper functions for handling node clicks//

    private void InitiateAttack(){
        LineController lineController = levelBuilder.GetConnectionController(selectedNode, targetNode);
        lineController.StartAttack(selectedNode.transform, selectedNode.GetComponent<NodeController>().GetColor(), targetNode.GetComponent<NodeController>().GetTransientShape());
    }

    private void LaunchAttack(GameObject node){
        // updateTargetNeighbors(node);
        // pulseAll();
    }

    private void ResetTargetShape(){
        targetNode.GetComponent<SpriteRenderer>().sprite = targetOriginalShape;
    }

    // Assumes that a node is selected and its neighbors have already been updated for the current selected Node
    private void PulseAll(){
        selectedNode.GetComponent<NodeController>().activatePulse();
        for (int i = 0; i < targetNeighbors.Length; i++){
            targetNeighbors[i].GetComponent<NodeController>().activatePulse();
        }
    }

    private void UpdateTargetNeighbors(){
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
        UpdateOwnership(target, GetNodeOwnership(source), shape);
        //tell the ai/player that it has 1 more attack now that it succeeded
        attacks[GetNodeOwnership(source)]--;
        GameObject[] neighbors = levelBuilder.FindNeighbors(target);
        for (int i = 0; i < neighbors.Length; i++){
            if (neighbors[i] != source) {
                LineController lineController = levelBuilder.GetConnectionController(target, neighbors[i]);
                lineController.StopAttackCleanConnections(target.transform);
                
            }
        }
        //if the node that just got taken over was attacking decrement attacks from that player
        //if (target.wasAttacking) {attacks[GetNodeOwnership(target)]--;}
    }

    // node - the node to be updated, player - the player who now owns the node 
    public void UpdateOwnership(GameObject node, int player, int shape)
    {
        int nodeIndex = levelBuilder.NodeToIndex(node);
        nodeOwnership[nodeIndex] = player;
        node.GetComponent<NodeController>().changeColor(player);
        node.GetComponent<NodeController>().setShape(shape);
        // Debug.Log("In Game Controller, node " + nodeIndex + " is now owned by player " + player + " with shape " + shape);
        // Debug.Log("And node " + nodeIndex + " is reporting shape " + node.GetComponent<NodeController>().GetShape());
        CheckGameOver();
    }

    private void CheckGameOver() {
        winner = CheckWinnerNumber();
        if (winner > 0){
            Debug.Log("Player " + winner + " wins!");
            Time.timeScale = 0;
        }
        // If this is the furthest unlocked level, unlock the next one
        // Note that this does NOT check whether the next level actually exists
        int levelsUnlocked = PlayerPrefs.GetInt("levelsUnlocked");
        if (winner == 1 && levelsUnlocked == levelBuilder.GetLevelNumber()){
            Debug.Log("Updating levelsUnlocked to " + (levelsUnlocked + 1));
            PlayerPrefs.SetInt("levelsUnlocked", levelsUnlocked + 1);
        }
        else if (winner == 1)
        {
            Debug.Log("Player win but Levels Unlocked not affected.");
        }
    }

    // returns -1 if there is no winner, otherwise returns the winning player number
    private int CheckWinnerNumber(){
        int potential = nodeOwnership[0];
        for (int i = 1; i < nodeOwnership.Length; i++){
            if (nodeOwnership[i] != potential){
                return -1;
            }
        }
        return potential;
    }

    private bool IsOwned(GameObject node, int playerNumber){
        if (node == background) {
            return false; 
        }
        int nodeIndex = levelBuilder.NodeToIndex(node);
        return nodeOwnership[nodeIndex] == playerNumber; 
    }

    public void StartComputerPlayer(int playerNumber, int maxAttacks) {
        StartCoroutine(ComputerPlayer(playerNumber, maxAttacks));
    }

    IEnumerator ComputerPlayer(int playerNumber, int maxAttacks) {
        GameObject node = null;
        GameObject targetNode = null;
        List<GameObject> neighbors = new List<GameObject>(); 
        List<GameObject> attackNodes = new List<GameObject>();
        List<GameObject> tmpNeighbors = new List<GameObject>();
        LineController testLine;
        int ownedNodes = 1;
        //int nodeIndex = 0;
        int nodeConquer = 0;
        //var rnd = new Random();
        //Debug.Log("Cpu Instantiated");

        //as long as there is no winner
        while (winner <= 0) {
            node = null;
            yield return null;
            // Debug.Log("Current attacks and max attacks" + attacks[playerNumber] + " " + maxAttacks);
            //if we can do another attack
            if (attacks[playerNumber] < maxAttacks) {
                ownedNodes = 0;
                //iterate over the nodes and find one that can attack
                attackNodes.Clear();
                foreach (GameObject tmpNode in nodes) {
                    //if it is owned by this cpu
                    if(IsOwned(tmpNode, playerNumber)) {
                        attackNodes.Add(tmpNode);
                        ownedNodes++;
                    }
                    //Debug.Log("Number Nodes: " + ownedNodes + " in cpu");
                }
                node = attackNodes[Random.Range(0, attackNodes.Count)];
                neighbors.Clear();
                neighbors.AddRange(levelBuilder.FindNeighbors(node));
                if (node != null && attacks[playerNumber] < maxAttacks) {
                    tmpNeighbors.Clear();
                    for (int i = 0; i < neighbors.Count; i++){
                        if (!IsOwned(neighbors[i], playerNumber)){
                            tmpNeighbors.Add(neighbors[i]);
                        }
                    }
                    //Debug.Log("number of neighbors " + tmpNeighbors.Count);
                    if (tmpNeighbors.Count != 0) {
                        targetNode = tmpNeighbors[Random.Range(0, tmpNeighbors.Count)];
                        //attack random neighbor
                        testLine = levelBuilder.GetConnectionController(node, targetNode);
                        //if testLine
                        testLine.StartAttack(node.transform, node.GetComponent<NodeController>().GetColor(), Random.Range(1,4));
                        //increment attack count and reset the loop to find a new node that can attack
                        attacks[playerNumber] = attacks[playerNumber]+1;
                    }
                }
            }
            
        }
    }
}
