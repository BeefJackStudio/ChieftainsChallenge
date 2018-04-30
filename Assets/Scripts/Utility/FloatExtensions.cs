using UnityEngine;

public static class FloatExtensions {

    public static float Remap(this float value, float min, float max, float targetMin, float targetMax) {
        value = Mathf.Clamp(value, min, max);
        float result = targetMin + (value - min) * (targetMax - targetMin) / (max - min);
        if (Mathf.Approximately(result, 0))
            return 0;

        return result;
    }

    public static bool Aprox(this float value, float other, float threshold) {
        return Mathf.Abs(value - other) < threshold;
    }

    public static float ClampNeg1(this float value) {
        if (value < -1f) {
            return -1f;
        }
        if (value > 1f) {
            return 1f;
        }

        return value;
    }

}