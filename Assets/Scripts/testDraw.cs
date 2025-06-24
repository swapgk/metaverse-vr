using UnityEngine;
using System.Collections;

public class testDraw : MonoBehaviour
{
    private LineRenderer Line;
    public float lineWidth = 0.1f;
    float x_pos = 0f;

    Vector3 positions0,positions1;

    void Start()
    {
        Line = new GameObject("Line").AddComponent<LineRenderer>();

        Line.material = new Material(Shader.Find("Sprites/Default"));

        // Vector3 positions0 = new Vector3(0f, 0f, 0.0f);
        // Vector3 positions1 = new Vector3(0f, 0f, 0.0f);

        positions0 = transform.position;
        positions1 = transform.position;

        Line.startColor = Color.red;
        Line.endColor = Color.red;

        // set width of the renderer
        Line.startWidth = lineWidth;
        Line.endWidth = lineWidth;
        Line.positionCount = 2;
        Line.SetPosition(0,positions0);
        Line.SetPosition(1,positions1);

    }

    void Update() {
        // x_pos = x_pos + 0.1f;
        Vector3 positionsNew = (transform.position);
        Debug.Log("positionsNew"+ positionsNew);

        Line.positionCount++;
        Line.SetPosition(Line.positionCount -1 ,positionsNew);
    }
    
}