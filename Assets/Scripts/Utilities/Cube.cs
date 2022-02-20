using UnityEngine;

public static class Cube {
    public static int GetDistanceToHex(Hex from, Hex to) {
        if (from == null || to == null) return -1;
        Vector3 vec = CubeSubtract(from, to);
        return (int)Mathf.Max(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }

    public static Vector3 CubeSubtract(Hex a, Hex b) {
        return new Vector3(a.Q - b.Q, a.R - b.R, a.S - b.S);
    }

    public static Vector3 CubeAdd(Hex a, Vector3 b) {
        return new Vector3(a.Q + b.x, a.R + b.y, a.S + b.z);
    }
}