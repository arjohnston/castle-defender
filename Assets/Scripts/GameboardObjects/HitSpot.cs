using UnityEngine;

public class HitSpot {
    public Color color { get; set; }
    public int radius { get; set; }
    public bool ignoreEdgeOfMap { get; set; }

    public HitSpot(Color color, int radius, bool ignoreEdgeOfMap = false) {
        this.color = color;
        this.radius = radius;
        this.ignoreEdgeOfMap = ignoreEdgeOfMap;
    }
}
