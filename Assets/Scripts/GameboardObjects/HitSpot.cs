using UnityEngine;

public class HitSpot {
    public Color color { get; set; }
    public int radius { get; set; }

    public HitSpot(Color color, int radius) {
        this.color = color;
        this.radius = radius;
    }
}
