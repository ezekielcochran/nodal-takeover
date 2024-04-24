using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{

    [SerializeField] private Sprite circle;
    [SerializeField] private Sprite square;
    [SerializeField] private Sprite hexagon;
    [SerializeField] private Sprite triangle;

    LineRenderer currentlyAttacking = null;
    
    private Sprite transientShape;
    private Sprite actualShape;
    private bool isPulsing;
    private SpriteRenderer _render;
    GameController gameController;
    Color[] playerColors = new Color[6] {Color.white, Color.red, Color.blue, Color.green, Color.yellow, Color.magenta}; 

    // Start is called before the first frame update
    void Start()
    {
        // Global Variable which toggles based off whether a node is  being toggled or not
        isPulsing = false;
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();

        // setting these here causes a race condition with LevelBuilder's start, which initializes starting nodes for each player
        if (actualShape != null) {
            transientShape = actualShape;
            return;
        }
        transientShape = circle;
        actualShape = circle;
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        gameController.ReportClick(gameObject);
    }

    public int toggleShape(){
        int shapeNum = -1;
        if (transientShape == circle)
        {
            transientShape = square;
            shapeNum = 1;
        }
        else if (transientShape == square)
        {
            transientShape = hexagon;
            shapeNum = 2;
        }
        else if (transientShape == hexagon)
        {
            transientShape = triangle;
            shapeNum = 3;
        }
        else if (transientShape == triangle)
        {
            transientShape = square;
            shapeNum = 1;
        }
        _render = GetComponent<SpriteRenderer>();
        _render.sprite = transientShape;
        return shapeNum;
    }

    // May be unnecessary if GameController directly changes sprite type as it is currently in resetTargetShape()
    public void setShape(int shape){
        switch(shape){
            case 0:
                actualShape = circle;
                break;
            case 1:
                actualShape = square;
                break;
            case 2:
                actualShape = hexagon;
                break;
            case 3:
                actualShape = triangle;
                break;
        }
        _render = GetComponent<SpriteRenderer>();
        _render.sprite = actualShape;
    }

    public int GetShape(){
        if (actualShape == circle){
            return 0;
        }
        else if (actualShape == square){
            return 1;
        }
        else if (actualShape == hexagon){
            return 2;
        }
        else if (actualShape == triangle){
            return 3;
        }
        return -1;
    }

    public int GetTransientShape(){
        transientShape = GetComponent<SpriteRenderer>().sprite;
        if (transientShape == circle){
            return 0;
        }
        else if (transientShape == square){
            return 1;
        }
        else if (transientShape == hexagon){
            return 2;
        }
        else if (transientShape == triangle){
            return 3;
        }
        return -1;
    }

    public void AttackFinished(){
        currentlyAttacking = null;
    }

    public void CommenceAttack(LineController lineController, Transform origin, Color color, int targetIntendedShape){
        if (currentlyAttacking != null){
            currentlyAttacking.GetComponent<LineController>().PauseAttack(transform);
        }
        lineController.StartAttack(origin, color, targetIntendedShape);
        currentlyAttacking = lineController.GetComponent<LineRenderer>();
    }

    public Color GetColor(){
        _render = GetComponent<SpriteRenderer>();
        return _render.color;
    }
    public void changeColor(int color){
        _render = GetComponent<SpriteRenderer>();
        _render.color = playerColors[color];
    }

    public void ActivatePulse(){
        isPulsing = true;
        StartCoroutine(Pulse());
    }

    public void DeactivatePulse(){
        isPulsing = false;
    }

    IEnumerator Pulse(){
        int direction = 1;
        float scaleSpeed = 0.003f;
        float scale = 1.3f;
        float size = 1.0f;
        Vector3 originalSize = transform.localScale;
        
        while(isPulsing){
            if (size > scale){
                direction = -1;
            } else if (size < .8f){
                direction = 1;
            }
            
            size = (direction * scaleSpeed) + size;
            Vector3 newScale = originalSize * size;
            transform.localScale = newScale;

            yield return null;
        }
        transform.localScale = originalSize;

    }
}
