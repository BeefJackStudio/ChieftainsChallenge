using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Button3D : UI3DElement {

    public UnityEvent onButtonClick;
    public Button3DEffects onButtonClickEffect = Button3DEffects.SWING;

    private Transform m_SwingEffectParent;
    private float m_SwingEffectVelocity = 0;
    private float m_SwingEffectStartRotation = 0;

    private void Awake() {
        m_SwingEffectParent = transform.parent;
    }

    private void Start() {
        m_SwingEffectStartRotation = m_SwingEffectParent.localRotation.x;
    }

    private void Update() {
        m_SwingEffectParent.Rotate(m_SwingEffectVelocity * Time.deltaTime, 0, 0);
        m_SwingEffectVelocity += (m_SwingEffectStartRotation - m_SwingEffectParent.localRotation.x) * 20;
        m_SwingEffectVelocity *= 0.975f;
    }

    public override void OnInteract() {
        base.OnInteract();

        onButtonClick.Invoke();

        m_SwingEffectVelocity -= 30;
    }
}

public enum Button3DEffects {
    NONE,
    SWING
}