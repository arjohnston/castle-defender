using UnityEngine;

public class Hex {
    public int Q;
    public int R;
    public int S;
    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    static readonly float radius = 1f;

    public Hex(int Q, int R) {
        this.Q = Q;
        this.R = R;
        this.S = -Q - R;
    }

    public float HexHeight() {
        return radius * 2;
    }

    public float HexWidth() {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing() {
        return HexHeight() * 0.75f;
    }

    public float HexHorizontalSpacing() {
        return HexWidth();
    }

    public Vector3 Position() {
        bool isOddRow = (int)Mathf.Abs(this.R % 2) == 1;

        float horizontalModifier = this.Q;

        // rows above 0
        if (this.Q + this.S > 0) {
            if (isOddRow) horizontalModifier += this.R / 2 - 1;
            else horizontalModifier += this.R / 2;
        }
        // rows below 0
        else if (this.Q + this.S < 0) {
            if (isOddRow) horizontalModifier += this.R / 2;
            else horizontalModifier += this.R / 2;
        }

        if (isOddRow) horizontalModifier += 0.5f;

        return new Vector3(
            HexHorizontalSpacing() * horizontalModifier,
            0,
            HexVerticalSpacing() * -this.R
        );
    }
}