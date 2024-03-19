using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Handler : MonoBehaviour
{



    private SpriteRenderer _render;
    private Color _color;

    
    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<SpriteRenderer>();
        _render.material.SetColor("red",_color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
