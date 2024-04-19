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

    // Start is called before the first frame update
    void Start() {}

    public void DrawBackgroundSegment(Transform start, Transform end)
    {
        this.leftPoint = start;
        this.rightPoint = end;
        backgroundLine = GetComponent<LineRenderer>(); // Moving the initialization to Start() doesn't work... seems that new instances don't get their Start() methods called
        backgroundLine.positionCount = 2;
        backgroundLine.SetPosition(0, start.position);
        backgroundLine.SetPosition(1, end.position);
    }

    // public void UpdateAttack(Transform origin, float progress) // Must be called after StartAttack is called with the same origin
    // {
    //     LineRenderer progressLine;
    //     Transform destination;
    //     if (origin == leftPoint)
    //     {
    //         destination = rightPoint;
    //         progressLine = leftAttackLine;
    //     }
    //     else if (origin == rightPoint)
    //     {
    //         destination = leftPoint;
    //         progressLine = rightAttackLine;
    //     }
    //     else
    //     {
    //         Debug.LogError("Error: LineController.Embark() called with a transform that is not a start or end point");
    //         return;
    //     }

    //     Vector3 midpoint = Vector3.Lerp(origin.position, destination.position, progress);
    //     progressLine.SetPosition(1, midpoint);
    // }

    public void StartAttack(Transform origin, Color color, int shape, float initialProgress = 0.0f) // Must be called after DrawBackgroundSegment
    {
        // We cannot attach multiple Renderers to the same object, so we spawn a new empty child object for the progress line
        LineRenderer progress = new GameObject().AddComponent<LineRenderer>();
        progress.gameObject.transform.SetParent(transform, false);
        progress.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);

        Transform destination;
        if (origin == leftPoint)
        {
            destination = rightPoint;
            progress.gameObject.name = "Left Attack";
            leftAttackLine = progress;
        }
        else if (origin == rightPoint)
        {
            destination = leftPoint;
            progress.gameObject.name = "Right Attack";
            rightAttackLine = progress;
        }
        else
        {
            Debug.LogError("Error: LineController.Embark() called with a transform that is not a start or end point");
            return;
        }

        // point between the two nodes, used as the control point for the progress line
        Vector3 midpoint = Vector3.Lerp(origin.position, destination.position, initialProgress);

        progress.material = new Material(Shader.Find("Sprites/Default"));
        progress.positionCount = 2;
        progress.SetPosition(0, origin.position);
        progress.SetPosition(1, midpoint);
        progress.startWidth = backgroundLine.startWidth * 2;
        progress.endWidth = backgroundLine.endWidth * 2;

        // Set the sorting layer for the progress line renderer: we want it in the Lines layer, but in front of the background line
        progress.sortingLayerName = "Lines";

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
        progress.colorGradient = gradient;
    }

    // Update is called once per frame
    private void Update() {
        // progress.SetPosition(0, start.position);
    }
}

