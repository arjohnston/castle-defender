using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;
using Unity.Netcode;

public class LineController : NetworkSingleton<LineController> {
    public GameObject LinePrefab;
    public GameObject line;
    private int _resolution = 20;
    private bool _isDrawn = false;
    private float _timer = 0.0f;
    private float _drawTime = 0.5f;
    private Vector3 prevFrom = new Vector3(0, 0, 0);
    private Vector3 prevTo = new Vector3(0, 0, 0);

    void Update() {
        if (_isDrawn) {
            _timer += Time.deltaTime;
        }

        if (_timer >= _drawTime) {
            Clear();
        }
    }

    public void DeferLineDestruction(float seconds)
    {
        AddTimeLineClientRpc(seconds);
    }
    public void AddTime(float seconds)
    {
        _drawTime += seconds;
    }
    public void DrawAttackLine(GameObject from, GameObject to) {

     DrawAttackLineServerRpc(from.transform.position, to.transform.position);

    }

    public void Draw(Vector3 from, Vector3 to) {
        if (_isDrawn)
        {
            if(prevFrom == from && prevTo == to)
            {
                _drawTime += Time.deltaTime;
            }
            return;
        }
        _isDrawn = true;
        line = Instantiate(LinePrefab, from, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.positionCount = _resolution;

        lr.SetPositions(CalculateArcArray(from,to));
        prevFrom = from;
        prevTo = to;

    }

    Vector3[] CalculateArcArray(Vector3 from, Vector3 to) {
        Vector3[] arcArray = new Vector3[_resolution];

        for (int n = 0; n < arcArray.Length; n++) {
            float range = .1f * ((10f / arcArray.Length) * n); // this is to scale range for lerp method
            Vector3 referencePoint = Vector3.Lerp(from, to, range);

            float y = (n*referencePoint.y) * (arcArray.Length - n);
            y *= .05f; // scale down the height a bit
            arcArray[n] = referencePoint + new Vector3(0, y);
        }

        return arcArray;
    }


    private void Clear() {
        Destroy(line);
        _isDrawn = false;
        _timer = 0.0f;
        _drawTime = .5f;
        prevFrom = new Vector3(0, 0, 0);
        prevTo = new Vector3 (0,0,0);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DrawAttackLineServerRpc(Vector3 from, Vector3 to) {
        DrawAttackLineClientRpc(from, to);
    }

    [ClientRpc]
    public void DrawAttackLineClientRpc(Vector3 from, Vector3 to) {
        Draw(from, to);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddTimeLineServerRpc(float seconds)
    {
        AddTimeLineClientRpc(seconds);
    }

    [ClientRpc]
    public void AddTimeLineClientRpc(float seconds)
    {
        AddTime(seconds);
    }

}
