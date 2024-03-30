using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Much of this script is thrown together, please do NOT use this in the final product
// Besides Micah's functionality, this script is meant to be a demonstration of how to use the LineController and LevelBuilder scripts, at least for the prototype

public class Node_Handler : MonoBehaviour
{
    private bool attacking = false;
    private float attackProgressToAll = 0.0f;


    private SpriteRenderer _render;    
    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            if (_render.color == Color.white){ _render.color = Color.red;}
            else{ _render.color = Color.white;}
            // _render.color = Color.red;
        }
    }

    void OnMouseDown() {
        if (_render.color == Color.white){ _render.color = Color.red;}
        else{ _render.color = Color.white;}
        try {   
            AttackSomeoneRandom();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Tried to do Level 4 magic in another level... error: " + e);
        }
    }
    public void SimulateClick() {
        if (_render.color == Color.white){ _render.color = Color.red;}
        else{ _render.color = Color.white;}
        AttackSomeoneRandom();
    }

    // Allow color to change when the Node is clicked
    private void AttackEveryone(){
        if (attacking) {return;} // If the node is already attacking, don't allow it to attack again (this is to prevent infinite loops and duplicate attacing paths)
            attacking = true;
            LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
            GameObject[] neighbors = levelBuilder.FindNeighbors(gameObject);

            // Start an Attack to all neighboring nodes
            for (int i = 0; i < neighbors.Length; i++)
            {
                LineController lineController = levelBuilder.GetConnectionController(gameObject, neighbors[i]);
                lineController.StartAttack(gameObject.transform, Color.red, 0.0f);
                // neighbors[i].GetComponent<Node_Handler>().simulateClick();
            }
            StartCoroutine(AttackAll());
    }

    private void AttackSomeoneRandom(){
        if (attacking) {return;} // If the node is already attacking, don't allow it to attack again (this is to prevent infinite loops and duplicate attacing paths)
        attacking = true;
        LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        GameObject[] neighbors = levelBuilder.FindNeighbors(gameObject);

        // Start an Attack to a random neighboring node
        int randomIndex = UnityEngine.Random.Range(0, neighbors.Length);
        LineController lineController = levelBuilder.GetConnectionController(gameObject, neighbors[randomIndex]);
        lineController.StartAttack(gameObject.transform, Color.red);
        StartCoroutine(AttackRandom(neighbors[randomIndex]));
    }

    IEnumerator AttackAll(float speed = 0.2f){ // 1 means a connection is covered in 1 second
        while (attackProgressToAll < 1.0f)
        {
            attackProgressToAll += Time.deltaTime * speed;
            // Debug.Log(attackProgressToAll);
            updateAttacks(attackProgressToAll);
            yield return null;
        }
    }

    IEnumerator AttackRandom(GameObject target, float speed = 0.2f){ // 1 means a connection is covered in 1 second
        while (attackProgressToAll < 1.0f)
        {
            attackProgressToAll += Time.deltaTime * speed;
            // Debug.Log(attackProgressToAll);
            updateRandomAttack(target, attackProgressToAll);
            yield return null;
        }
        target.GetComponent<Node_Handler>().SimulateClick();
    }

    private void updateRandomAttack(GameObject target, float progress){
        LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        LineController lineController = levelBuilder.GetConnectionController(gameObject, target);
        lineController.UpdateAttack(gameObject.transform, progress);
    }

    private void updateAttacks(float progress){
        LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        GameObject[] neighbors = levelBuilder.FindNeighbors(gameObject);
        for (int i = 0; i < neighbors.Length; i++)
        {
            LineController lineController = levelBuilder.GetConnectionController(gameObject, neighbors[i]);
            lineController.UpdateAttack(gameObject.transform, progress);
        }
    }
}
