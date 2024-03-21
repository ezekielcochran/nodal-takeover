using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Handler : MonoBehaviour
{



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

    // Allow color to change when the Node is clicked
    void OnMouseDown(){
        if (_render.color == Color.white){ _render.color = Color.red;}
        else{ _render.color = Color.white;}
    }
}
