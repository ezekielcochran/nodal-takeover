using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I think it would be a good idea to add some protection to ensure that methods are called in the correct order

public class LineController : MonoBehaviour
{
    // These are the line renderers for the preexisting connection, left progress, and right progress lines
    // Note that "left" and "right" correspond to the order in which the nodes were connected in the level data file... this only corresponds to actual left and right if the lower index node is left of the higher
    private LineRenderer backgroundLine, leftAttackLine, rightAttackLine;
    // leftPoint and rightPoint do NOT change: they are the start and end points of the background line in increasing node index order
    // Some methods use origin and destination: thsese depend on which node is attacking
    private Transform leftPoint, rightPoint;
    private int leftAttackShape, rightAttackShape;
    private float leftAttackProgress, rightAttackProgress;
    private bool active = false;
    private GameController gameController;

    // Start is called before the first frame update
    void Start() {
        try {
            gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        } catch {
            gameController = null; // This is the case when the LineController is used in the level menu
        }
    }

    public void DrawBackgroundSegment(Transform start, Transform end)
    {
        leftPoint = start;
        rightPoint = end;
        backgroundLine = GetComponent<LineRenderer>(); // Moving the initialization to Start() doesn't work... seems that new instances don't get their Start() methods called
        backgroundLine.positionCount = 2;
        backgroundLine.SetPosition(0, start.position);
        backgroundLine.SetPosition(1, end.position);
    }

    private void UpdateAttack(Transform origin, float progress) // Must be called after StartAttack is called with the same origin
    {
        LineRenderer progressLine;
        Transform destination;
        if (origin == leftPoint)
        {
            destination = rightPoint;
            progressLine = leftAttackLine;
        }
        else if (origin == rightPoint)
        {
            destination = leftPoint;
            progressLine = rightAttackLine;
        }
        else
        {
            Debug.LogError("Error: LineController.Embark() called with a transform that is not a start or end point");
            return;
        }

        // if (progressLine == null) {
        //     return;
        // }
        Vector3 midpoint = Vector3.Lerp(origin.position, destination.position, progress);
        progressLine.SetPosition(1, midpoint);
    }

    // Remember that this is also used in LevelMenuManager.cs!
    public void StartAttack(Transform origin, Color color, int targetIntendedShape, float initialProgress = 0.0f) // Must be called after DrawBackgroundSegment
    {
        // We cannot attach multiple Renderers to the same object, so we spawn a new empty child object for the progress line
        LineRenderer attack;
        Transform destination;
        float progress;
        if (origin == leftPoint)
        {
            destination = rightPoint;
            if (leftAttackLine == null) {
                leftAttackLine = InitializeAttack(color, "Left Attack");
                leftAttackProgress = initialProgress; // This is mainly so in the level menu, the attack immediately shows as completed
            }
            leftAttackShape = targetIntendedShape; // This allows changing shape on an already started attack... might want to revisit
            attack = leftAttackLine;
            progress = leftAttackProgress;
        }
        else if (origin == rightPoint)
        {
            destination = leftPoint;
            if (rightAttackLine == null) {
                rightAttackLine = InitializeAttack(color, "Right Attack");
                rightAttackProgress = initialProgress;
            }
            rightAttackShape = targetIntendedShape;
            attack = rightAttackLine;
            progress = rightAttackProgress;
        }
        else
        {
            Debug.LogError("Error: LineController.StartAttack() called with a transform that is not a start or end point");
            return;
        }

        // point between the two nodes, used as the control point for the progress line
        Vector3 midpoint = Vector3.Lerp(origin.position, destination.position, progress);
        attack.SetPosition(0, origin.position);
        attack.SetPosition(1, midpoint);

        // Start the coroutine to animate the attacks
        active = true;
        if (gameController != null) { // if it is null, we should be in the level menu
            StartCoroutine(UpdateAttacks());
        }
    }

    IEnumerator UpdateAttacks(float speed = 0.5f) {
        while (active) {
            if (leftAttackLine != null)
            {
                leftAttackProgress += speed * Time.deltaTime;
                UpdateAttack(leftPoint, leftAttackProgress);
            }
            if (rightAttackLine != null)
            {
                rightAttackProgress += speed * Time.deltaTime;
                UpdateAttack(rightPoint, rightAttackProgress);
            }

            if (leftAttackProgress >= 1.0f) {
                gameController.UpdateOwnership(rightPoint.gameObject, gameController.GetNodeOwnership(leftPoint.gameObject), leftAttackShape);
                Debug.Log("Updated target shape to " + leftAttackShape);
                active = false;
            }
            if (rightAttackProgress >= 1.0f) {
                gameController.UpdateOwnership(leftPoint.gameObject, gameController.GetNodeOwnership(rightPoint.gameObject), rightAttackShape);
                Debug.Log("Updated target shape to " + rightAttackShape);
                active = false;
            }
            yield return null;
        }
    }

    // This function should do everything necessary to start a new attack except setting the position
    private LineRenderer InitializeAttack(Color color, string name) {
        LineRenderer result = new GameObject().AddComponent<LineRenderer>();
        result.gameObject.transform.SetParent(transform, false);
        result.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        result.gameObject.name = name;
        result.sortingLayerName = "Lines";
        result.sortingOrder = 1;

        // materials
        result.material = new Material(Shader.Find("Sprites/Default"));
        result.positionCount = 2;
        result.startWidth = backgroundLine.startWidth * 2;
        result.endWidth = backgroundLine.endWidth * 2;

        // Set the progress color from the input parameter
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = color;
        colorKey[1].time = 1.0f;
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKey, alphaKey);
        result.colorGradient = gradient;
        return result;
    }

    // Update is called once per frame
    private void Update() {
        // progress.SetPosition(0, start.position);
    }
}

