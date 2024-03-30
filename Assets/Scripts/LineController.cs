using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;

    // Start is called before the first frame update
    void Start() {}

    public void DrawSegment(Transform start, Transform end)
    {
        lr = GetComponent<LineRenderer>(); // Moving the initialization to Start() doesn't work... seems that new instances dont' get their Start() methods called
        lr.positionCount = 2;
        lr.SetPosition(0, start.position);
        lr.SetPosition(1, end.position);
    }

    // Update is called once per frame
    private void Update() {}
}

