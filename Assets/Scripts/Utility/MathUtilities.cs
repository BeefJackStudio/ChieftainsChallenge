using UnityEngine;

public static class MathUtilities {

    public static float GetNormalizedTime(float startTime, float duration, float currentTime) {
        return Mathf.Clamp01((currentTime - startTime) / duration);
    }

}