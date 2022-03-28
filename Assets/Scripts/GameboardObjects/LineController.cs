using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer line;
    private Transform[] points;


    // Start is called before the first frame update
    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                line.SetPosition(i, points[i].position);
            }
        }
    }
    public void SetUpLine(Transform[] points)
    {
        line.enabled = true;
        line.positionCount = points.Length;
        this.points = points;
    }
    public void ClearAttackLines()
    {
        line.enabled = false;
        points = null;

    }

}
