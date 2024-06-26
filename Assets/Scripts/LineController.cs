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
    private int leftShape, rightShape, leftIntendedShape, rightIntendedShape;
    private float leftAttackProgress, rightAttackProgress;
    private bool leftActive = false;
    private bool rightActive = false;
    private bool coroutineRunning = false;
    private GameController gameController;
    private const float DEFAULT_SPEED = 0.3f;

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
        // relying on short-circuit boolean evaluation here for the level menu
        if ((gameController != null) && (gameController.GetNodeOwnership(leftPoint.gameObject) == gameController.GetNodeOwnership(rightPoint.gameObject))) {
            Debug.LogError("Error: Friendly Fire! LineController.StartAttack() called between two nodes owned by the same player ");
            return;
        }

        // Handle if a previous attack was already in progress


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
            // The bottom line is for comparing attacks based on the intended shape, not the actual shape
            leftIntendedShape = targetIntendedShape; // This allows changing shape on an already started attack... might want to revisit
            if (gameController != null) {
                leftShape = origin.GetComponent<NodeController>().GetShape();
            } else {
                leftShape = 0; // another case when we check whether we are in the level menu
            }
            attack = leftAttackLine;
            progress = leftAttackProgress;
            leftActive = true;
        }
        else if (origin == rightPoint)
        {
            destination = leftPoint;
            if (rightAttackLine == null) {
                rightAttackLine = InitializeAttack(color, "Right Attack");
                rightAttackProgress = initialProgress;
            }
            // again, the following commented line is for comparing attacks based on the intended shape, not the actual shape
            rightIntendedShape = targetIntendedShape;
            if (gameController != null) {
                rightShape = origin.GetComponent<NodeController>().GetShape();
            } else {
                rightShape = 0; // another case when we check whether we are in the level menu
            }
            attack = rightAttackLine;
            progress = rightAttackProgress;
            rightActive = true;
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
        // Do NOT start the coroutine for the RIGHT attack if it is already going for the LEFT! (or vice versa, point is two copies running causes lines to double speed)
        if (gameController != null && coroutineRunning == false) { // if it is null, we should be in the level menu
            coroutineRunning = true;
            StartCoroutine(UpdateAttacks());
        }
    }

    public void UpdateIntendedShape(Transform origin, int newShape)
    {
        if (origin == rightPoint)
        {
            leftIntendedShape = newShape;
        }
        else if (origin == leftPoint)
        {
            rightIntendedShape = newShape;
        }
        else
        {
            Debug.LogError("Error: LineController.UpdateIntendedShape() called with a transform that is not a start or end point");
        }
    }

    public void StopAttack(Transform origin)
    {
        if (origin == leftPoint)
        {
            if (leftAttackLine == null) {
                return;
            }
            Destroy(leftAttackLine.gameObject);
            leftAttackLine = null;
            leftAttackProgress = 0.0f;
            leftShape = 0;
            leftActive = false;
        }
        else if (origin == rightPoint)
        {
            if (rightAttackLine == null) {
                return;
            }
            Destroy(rightAttackLine.gameObject);
            rightAttackLine = null;
            rightAttackProgress = 0.0f;
            rightShape = 0;
            rightActive = false;
        }
        else
        {
            Debug.LogError("Error: LineController.StopAttack() called with a transform that is not a start or end point");
        }
    }

    // As of 02/23/24 at 10:00, this function is untested
    public void PauseAttack(Transform origin)
    {
        if (origin == leftPoint)
        {
            leftActive = false;
        }
        else if (origin == rightPoint)
        {
            rightActive = false;
        }
        else
        {
            Debug.LogError("Error: LineController.PauseAttack() called with a transform that is not a start or end point");
        }
    }

    private void PruneFinishedAttacks()
    {
        if (leftAttackLine != null && leftAttackProgress >= 1.0f)
        {
            StopAttack(leftPoint);
        }
        if (rightAttackLine != null && rightAttackProgress >= 1.0f)
        {
            StopAttack(rightPoint);
        }
    }

    // game controller should call this function on every connection of a newly captured node, except the incoming "victorious" connection
    public void StopAttackCleanConnections(Transform origin)
    {
        StopAttack(origin);
        PruneFinishedAttacks();
    }

    // This function needs to only be called ONCE per frame, so that the progress of the attacks is consistent
    IEnumerator UpdateAttacks(float speed = DEFAULT_SPEED) {
        while (leftActive || rightActive) {
            float dx = speed * Time.deltaTime;
            // If either attack is active, it should have a line
            Debug.Assert(leftAttackLine != null || rightAttackLine != null, "Error: LineController.UpdateAttacks() called with no active attacks");

            // only the left is attacking
            if (rightAttackLine == null || rightActive == false) // The second check allows inactive attacks to be bulldozed by active ones
            {
                leftAttackProgress += dx;
                UpdateAttack(leftPoint, leftAttackProgress);
            }
            // only the right is attacking
            else if (leftAttackLine == null || leftActive == false)
            {
                rightAttackProgress += dx;
                UpdateAttack(rightPoint, rightAttackProgress);
            }
            // both are attacking
            else
            {
                Debug.Assert(leftAttackLine != null && rightAttackLine != null, "Error: LineController.UpdateAttacks() called with both attacks active but no line renderers");
                leftAttackProgress += dx;
                rightAttackProgress += dx;
                // The attacks have collided
                if (leftAttackProgress + rightAttackProgress >= 1)
                {
                    // Debug.Log("Collision! Left Shape: " + leftShape + ", Right Shape: " + rightShape + ", Left Progress: " + leftAttackProgress + ", Right Progress: " + rightAttackProgress );

                    // They have the same shape
                    if (BeatsShape(leftShape, rightShape) == 0)
                    {
                        // So they are in deadlock, and each eats half the extra
                        float overflow = leftAttackProgress + rightAttackProgress - 1;
                        leftAttackProgress -= overflow / 2;
                        rightAttackProgress -= overflow / 2;
                    }
                    // Left shape wins, so left slows down and right is pushed back
                    else if (BeatsShape(leftShape, rightShape) == 1)
                    {
                        leftAttackProgress -= dx / 2;
                        rightAttackProgress = 1 - leftAttackProgress;
                    }
                    // Right shape wins, so left attack is pushed back
                    else if (BeatsShape(leftShape, rightShape) == -1)
                    {
                        rightAttackProgress -= dx / 2;
                        leftAttackProgress = 1 - rightAttackProgress;
                    }
                    else
                    {
                        Debug.LogError("Error: LineController.UpdateAttacks() called with unknown return value from BeatsShape()");
                    }
                }
                UpdateAttack(leftPoint, leftAttackProgress);
                UpdateAttack(rightPoint, rightAttackProgress);
            }

            // Did someone win?
            if (leftAttackProgress >= 1.0f) {
                leftPoint.gameObject.GetComponent<NodeController>().AttackFinished();
                gameController.ReportConquer(leftPoint.gameObject, rightPoint.gameObject, leftIntendedShape);
                leftActive = false;
                rightActive = false;
                StopAttack(rightPoint);
            }
            if (rightAttackProgress >= 1.0f) {
                rightPoint.gameObject.GetComponent<NodeController>().AttackFinished();
                gameController.ReportConquer(rightPoint.gameObject, leftPoint.gameObject, rightIntendedShape);
                leftActive = false;
                rightActive = false;
                StopAttack(leftPoint);
            }
            yield return null;
        }
        coroutineRunning = false;
    }

    private int BeatsShape(int firstShape, int secondShape) {
        Debug.Assert(firstShape >= 1 && firstShape <= 3, "Error: LineController.BeatsShape() called with an unknown first shape: I only know shapes 1, 2, and 3");
        Debug.Assert(secondShape >= 1 && secondShape <= 3, "Error: LineController.BeatsShape() called with an unknown second shape: I only know shapes 1, 2, and 3");
        if (firstShape == secondShape) {
            return 0;
        }
        if (firstShape == 1 && secondShape == 3) {
            return 1;
        }
        if (firstShape == 2 && secondShape == 1) {
            return 1;
        }
        if (firstShape == 3 && secondShape == 2) {
            return 1;
        }
        return -1;
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

