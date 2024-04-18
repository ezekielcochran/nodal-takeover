// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;

// public class NodeHandler : MonoBehaviour
// {
//     private bool nodeSelected;
//     LevelBuilder levelBuilder;
//     GameController gameController;
//     GameObject[] neighbors;
//     GameObject[] nonCapturedNeighbors;

//     // Start is called before the first frame update
//     void Start(){
//         nodeSelected = false;
//         _render = GetComponent<SpriteRenderer>();
//         levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
//         gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
//         neighbors = levelBuilder.FindNeighbors(gameObject);
//         nonCapturedNeighbors = new GameObject[neighbors.Length];
//     }

//     void OnMouseDown(){

//         // Ensure that this node is owned by the player
//         if (gameController.isOwned(gameObject)){
//             int numValidTargets = 0;
//             // Determine which neighbors are not on the same team
//             foreach (GameObject neighbor in neighbors){
//                 if (!gameController.isOwned(neighbor)){
//                     nonCapturedNeighbors[numValidTargets++] = neighbor;
//                 }
//             }
//             if (numValidTargets >= 0){

//             }
//         }
//     }

//     public void togglePulse(){
//         StartCoroutine(Pulse())
//     }

//     private void Pulse(){
//         isPulsing = !isPulsing;
//         int direction = 1;
//         float scale = 1.5f;
//         float size = 1.0f;
//         while(isPulsing){
//             if (size > scale){
//                 direction = -1;
//             } else if (size < .8f){
//                 direction = 1;
//             }
//             transform.scale
//         }
//     }

//     void updateConnections(GameObject source){

//     }
// }
