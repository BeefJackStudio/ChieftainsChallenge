using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Slider3D : UI3DElement {

    public Transform sliderKnob;
    public Action<float> onValueChange = delegate { };

    private BoxCollider m_SliderArea;
    private float m_SliderValue = 1;
    private float m_MinPosition;
    private float m_MaxPosition;
    private Vector3 m_StartLocation;
    private Vector3 m_LiftedLocation;

    private RaycastHit m_RaycastHit;
    private bool m_IsSliding = false;
    private float m_RotationTarget = 0;

    private void Awake() {
        m_SliderArea = GetComponent<BoxCollider>();
        m_MinPosition = m_SliderArea.size.x / -2;
        m_MaxPosition = m_SliderArea.size.x / 2;
        m_SliderArea.size = new Vector3(m_SliderArea.size.x + 0.3f, m_SliderArea.size.y, m_SliderArea.size.z);

        m_StartLocation = transform.localPosition;
        m_LiftedLocation = m_StartLocation + new Vector3(0, 0, -0.15f);
    }

    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            m_IsSliding = false;
            m_RotationTarget = 0;
        }

        Vector3 targetLocation = m_StartLocation;
        if (m_IsSliding) {
            Vector3 hitPosition;
            if (UI3D.Instance.RaycastMouseOnPlane(m_RaycastHit.normal, m_RaycastHit.point, out hitPosition)) {
                SetValueFromWorldPoint(hitPosition);
            }

            targetLocation = m_LiftedLocation;
        }

        Quaternion currentRotation = transform.localRotation;
        transform.localRotation = Quaternion.Lerp(currentRotation, new Quaternion(currentRotation.x, m_RotationTarget, currentRotation.z, currentRotation.w), 0.1f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocation, 0.1f);
    }

    public override void OnInteract(RaycastHit hit) {
        base.OnInteract(hit);

        SetValueFromWorldPoint(hit.point);

        m_IsSliding = true;
        m_RaycastHit = hit;
    }

    private void SetValueFromWorldPoint(Vector3 worldPoint) {
        Vector3 localHit = transform.InverseTransformPoint(worldPoint);
        Value = (localHit.x / m_SliderArea.size.x) + (m_MaxPosition / m_SliderArea.size.x);

        m_RotationTarget = (Value - 0.5f) * -0.15f;
    }

    public float Value {
        get { return m_SliderValue; }
        set {
            m_SliderValue = Mathf.Clamp01(value);
            sliderKnob.localPosition = new Vector3(Mathf.Lerp(m_MinPosition, m_MaxPosition, m_SliderValue), sliderKnob.localPosition.y, sliderKnob.localPosition.z);
            onValueChange(m_SliderValue);
        }
    }
}