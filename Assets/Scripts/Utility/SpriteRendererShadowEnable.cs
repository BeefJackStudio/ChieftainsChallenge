using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererShadowEnable : MonoBehaviour {

    public Material litMaterial;
    public bool targetValue = true;

    [ContextMenu("Set status")]
    public void SetShadowStatus() {
        foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) {
            renderer.material = litMaterial;
            renderer.shadowCastingMode = targetValue ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = targetValue;
        }
    }
}
