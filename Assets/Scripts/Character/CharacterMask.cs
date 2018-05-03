using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMask : MonoBehaviour {

    public bool useJiggleBones = true;
    public Transform[] jigglebones;
    public float jiggleAmount = 1;
    public float damping = 0.5f;

    private Vector3 m_LastPosition;
    private Dictionary<Transform, Vector3> m_StartScales = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> m_StartRotations = new Dictionary<Transform, float>();
    private Vector3 m_Velocity = Vector3.zero;

    private void Awake() {
        m_LastPosition = transform.position;
        foreach (Transform t in jigglebones) {
            m_StartScales.Add(t, t.localScale);
            m_StartRotations.Add(t, t.transform.localEulerAngles.z);
        }
    }

    private void Update() {
        if (!useJiggleBones) return;

        Vector3 pos = transform.position;
        Vector3 delta = pos - m_LastPosition;

        if(delta.magnitude >= 3) {
            m_LastPosition = transform.position;
            delta = Vector3.zero;
        }

        m_Velocity = delta;
        m_Velocity *= damping;

        m_Velocity = Vector3.Lerp(m_Velocity, -m_Velocity + delta, damping);

        Vector3 targetScale = m_Velocity * jiggleAmount;
        float targetRotation = m_Velocity.x * 150 * jiggleAmount;

        foreach (Transform t in jigglebones) {
            //float rotation = Mathf.Lerp(t.rotation.eulerAngles.z, m_StartRotations[t] + targetRotation, 0.05f);
            t.localScale = Vector3.Lerp(t.localScale, m_StartScales[t] + targetScale, 0.1f);
            t.localRotation = Quaternion.Euler(0, 180, Mathf.Lerp(t.localEulerAngles.z, m_StartRotations[t] + targetRotation, 0.05f));           
        }
        m_LastPosition = pos;
    }
}
