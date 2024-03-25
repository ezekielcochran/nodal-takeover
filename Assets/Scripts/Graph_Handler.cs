using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph_Handler : MonoBehaviour
{
    // Create a 2D list representing our Non-Directed Graph
    int[][] graph = new int[][]
    {
        new int[] {1,2},
        new int[] {0,2,3,4},
        new int[] {0,1,4},
        new int[] {1,4,5},
        new int[] {2,3,5},
        new int[] {3,4},
        new int[] {4,5}
    };

    // public Graph_Handler()
    // {
    //     Transform[] Node_transforms = GetComponentsInChildren<Transform>();
    //     LineRenderer Node_linerenders = GetComponentInChildren<LineRenderer>();
    // }


    private Color line_color = Color.gray;
    public Material mat;
    Transform[] Node_transforms;

    // Start is called before the first frame update
    void Start()
    {
        // Create lists of all the node Positions (transforms) and LineRenders
        Transform[] tmp = GetComponentsInChildren<Transform>();
        Node_transforms = new Transform[tmp.Length-1];

        // Remove the first element since GetComponentsInChildren includes the parent object's transform which we don't want
        for (int i = 0; i < tmp.Length - 1; i++){
            Node_transforms[i] = tmp[i+1];
        }

        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        foreach (Transform child in Node_transforms)
        {
            LineRenderer LR = child.GetComponent<LineRenderer>();

            if (LR != null){
                lineRenderers.Add(LR);
            }
        }
    
        for (int i = 0; i < graph.Length; i++){
            drawLines(i, graph[i], lineRenderers[i]);
        }
    }

    void drawLines(int node, int[] connections, LineRenderer line){

        // Vector to contain all the positions needed for the LineRender
        Vector3[] positions = new Vector3[connections.Length * 2];
        List<Material> mat_list = new List<Material> {mat};

        Vector3 self_pos = Node_transforms[node].position;

        // LineRenderer line = node.GetComponent<LineRenderer>();
        // GameObject lineObj = new GameObject("DragLine", typeof(LineRenderer));
        // LineRenderer line = lineObj.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.startColor = Color.gray;
        line.endColor = Color.gray;
        line.SetMaterials(mat_list);
        line.positionCount = connections.Length * 2;
        line.sortingLayerName = "Lines";

        for (int i = 0; i < connections.Length - 1; i++){
            positions[i * 2] = self_pos;
            positions[i * 2 + 1] = Node_transforms[connections[i]].position;
        }
        positions[connections.Length *2 - 2] = self_pos;
        positions[connections.Length *2 - 1] = Node_transforms[connections[connections.Length-1]].position;

        line.SetPositions(positions);
        // line.SetPosition(0, Vector3.zero);
        // line.material = mat; 
        // line.SetPosition(1, Vector3.up);
    }
}
