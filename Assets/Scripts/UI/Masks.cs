using UnityEngine;
using System.Collections.Generic;
using System;

public class Masks : MonoBehaviour {

    public List<RectTransform> wheels;

    private void Update() {
        float wheelRotation = 10 * Time.deltaTime;
        foreach (RectTransform wheel in wheels) {
            wheel.Rotate(0, 0, wheelRotation);
        }
    }
}
