using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePulsator : MonoBehaviour {

    public float pulseAmount = 0.15f;
    public float pulseSpeed = 3;

    private Vector3 m_StartScale;

    private void Awake() {
        m_StartScale = transform.localScale;
    }

    void Update () {
        transform.localScale = m_StartScale * (1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount);
	}

}
