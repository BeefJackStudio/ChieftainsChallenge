using UnityEngine;
using System.Collections;

public class ScalePulse : MonoBehaviour {
    public float minScale;
    public float maxScale;
    bool dir = true;
    float duraction = 10;
    float startTime;
    float stepValue;

    // Use this for initialization
    void Start() {
        startTime = Time.time;
        stepValue = (minScale + ((maxScale - minScale) / 2)) * 100;
    }

    // Update is called once per frame
    void Update() {
        Vector3 scale = transform.localScale;
        float t = (Time.time - startTime) / duraction;
        if (dir) {
            stepValue = Mathf.SmoothStep(stepValue, minScale * 100, t);
            if (stepValue.Aprox(minScale * 100, 1)) {
                dir = false;
                startTime = Time.time;
            }

        } else {
            stepValue = Mathf.SmoothStep(stepValue, maxScale * 100, t);
            if (stepValue.Aprox(maxScale * 100, 1)) {
                dir = true;
                startTime = Time.time;
            }
        }
        scale.x = stepValue * 0.01f;
        scale.y = scale.x;
        transform.localScale = scale;
    }
}
