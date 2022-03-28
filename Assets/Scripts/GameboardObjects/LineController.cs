using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;
using Unity.Netcode;

public class LineController : NetworkSingleton<LineController> {
    public GameObject LinePrefab;
    public GameObject line;
    private bool _isDrawn = false;
    private float _timer = 0.0f;
    private float _drawTime = 0.5f;

    void Update() {
        if (_isDrawn) {
            _timer += Time.deltaTime;
        }

        if (_timer >= _drawTime) {
            Clear();
        }
    }

    public void DrawAttackLine(GameObject from, GameObject to) {
        DrawAttackLineServerRpc(from.transform.position, to.transform.position);
    }

    public void Draw(Vector3 from, Vector3 to) {
        if (_isDrawn) return;

        _isDrawn = true;
        line = Instantiate(LinePrefab, from, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
    }

    private void Clear() {
        Destroy(line);
        _isDrawn = false;
        _timer = 0.0f;
    }

    [ServerRpc(RequireOwnership=false)]
    public void DrawAttackLineServerRpc(Vector3 from, Vector3 to) {
        DrawAttackLineClientRpc(from, to);
    }

    [ClientRpc]
    public void DrawAttackLineClientRpc(Vector3 from, Vector3 to) {
        Draw(from, to);
    }
}
