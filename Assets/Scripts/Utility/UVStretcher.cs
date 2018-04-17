using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class UVStretcher : MonoBehaviour {

    public float scaleMultiplier = 0.5f;

    private MeshRenderer m_MeshRenderer;

    private void Awake() {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        m_MeshRenderer.material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y) * scaleMultiplier;
    }
}
