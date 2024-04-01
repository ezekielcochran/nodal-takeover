using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Much of this script is thrown together, please do NOT use this in the final product
// Besides Micah's functionality, this script is meant to be a demonstration of how to use the LineController and LevelBuilder scripts, at least for the prototype

public class Node_Handler : MonoBehaviour
{
    private bool attacking = false;
    private float attackProgress = 0.0f;
    private float speed;


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
        else if (_render.color == Color.red){ _render.color = Color.blue;}
        else if (_render.color == Color.blue){ _render.color = Color.green;}
        else if (_render.color == Color.green){ _render.color = Color.yellow;}
        else if (_render.color == Color.yellow){ _render.color = Color.black;}
        else if (_render.color == Color.black){ _render.color = Color.magenta;}
        else if (_render.color == Color.magenta){ _render.color = Color.white;}
        else{ _render.color = Color.white;}
            
        AttackSomeoneRandom();
    }

    private void AttackSomeoneRandom(){
        // Infinite loops might still be a problem, because attacking is turned back off once the attack is finished
        if (attacking) {return;} // If the node is already attacking, don't allow it to attack again (this is to prevent infinite loops and duplicate attacing paths)
        attacking = true;
        LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        if (speed <= 0.0f) {speed = levelBuilder.GetAttackSpeed();} // If the speed is not set, get it from the LevelBuilder, where it exists as a paramater
        GameObject[] neighbors = levelBuilder.FindNeighbors(gameObject);
        if (neighbors.Length == 0) {return;} // If the node has no neighbors, there is no one to attack

        // Start an Attack to a random neighboring node
        int randomIndex = UnityEngine.Random.Range(0, neighbors.Length);
        LineController lineController = levelBuilder.GetConnectionController(gameObject, neighbors[randomIndex]);
        lineController.StartAttack(gameObject.transform, _render.color);
        StartCoroutine(AttackRandom(neighbors[randomIndex], speed));
    }

    IEnumerator AttackRandom(GameObject target, float speed = 0.5f){ // 1 means a connection is covered in 1 second
        while (attackProgress < 1.0f)
        {
            attackProgress += Time.deltaTime * speed;
            // Debug.Log(attackProgressToAll);
            updateRandomAttack(target, attackProgress);
            yield return null;
        }
        target.GetComponent<Node_Handler>().SimulateClick();
        attacking = false;
        attackProgress = 0.0f;
    }

    private void updateRandomAttack(GameObject target, float progress){
        LevelBuilder levelBuilder = GameObject.Find("Level Elements").GetComponent<LevelBuilder>();
        LineController lineController = levelBuilder.GetConnectionController(gameObject, target);
        lineController.UpdateAttack(gameObject.transform, progress);
    }
}