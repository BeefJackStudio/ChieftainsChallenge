using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : UI3DElement {

    public UnityEvent onButtonClick;
    public Button3DEffects onButtonClickEffect = Button3DEffects.NONE;

    [Header("Swing effect")]
    public float swingEffectForce = 30;
    public float swingEffectReduction = 0.975f;
    public Transform swingEffectParentOverride;

    private Transform m_SwingEffectParent;
    private float m_SwingEffectVelocity = 0;
    private float m_SwingEffectStartRotation = 0;

    private void Awake() {
        if (swingEffectParentOverride == null) {
            m_SwingEffectParent = transform.parent;
        }else {
            m_SwingEffectParent = swingEffectParentOverride;
        }
    }

    private void Start() {
        m_SwingEffectStartRotation = m_SwingEffectParent.localRotation.x;
    }

    private void Update() {
        m_SwingEffectParent.Rotate(m_SwingEffectVelocity * Time.deltaTime, 0, 0);
        m_SwingEffectVelocity += (m_SwingEffectStartRotation - m_SwingEffectParent.localRotation.x) * 20;
        m_SwingEffectVelocity *= swingEffectReduction;
    }

    public override void OnInteract(RaycastHit hit) {
        base.OnInteract(hit);

        onButtonClick.Invoke();

        if (onButtonClickEffect == Button3DEffects.SWING) {
            m_SwingEffectVelocity -= swingEffectForce;
        }
    }
}

public enum Button3DEffects {
    NONE,
    SWING
}