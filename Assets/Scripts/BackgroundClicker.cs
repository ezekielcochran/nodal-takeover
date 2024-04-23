using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundClicker : MonoBehaviour
{
    GameController gameController;


    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }
    
    // Start is called before the first frame update
    void OnMouseDown()
    {
        gameController.ReportClick(gameObject);
    }
}
