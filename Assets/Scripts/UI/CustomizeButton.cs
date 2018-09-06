using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeButton : MonoBehaviour {

    public Transform previewParent;
    public SpriteRenderer lockIcon;

    private Button3D m_Button;
    private SpriteRenderer m_BackgroundImage;
    private GameObject m_PreviewObj;
    private int m_ButtonNumber;

    private bool m_IsSelected = false;
    private Vector3 m_StartPos;
    private Vector3 m_StartScale;
    private Vector3 m_TargetScale;
    private Vector3 m_StartPosition;
    private Vector3 m_FloatPosition;
    private float m_TargetScaleMultiplier = 1;
    private float m_StartFloatTime = 0;

    private void Awake() {
        m_Button = GetComponent<Button3D>();
        m_BackgroundImage = GetComponent<SpriteRenderer>();
        m_StartPosition = transform.localPosition;
        m_FloatPosition = m_StartPosition - new Vector3(0, 0, 1);
    }

    private void Start() {
        SetLocked(true);
    }

    private void Update() {

        if (lockIcon.gameObject.activeSelf) {
            lockIcon.transform.localScale = Vector3.one * (Mathf.Sin(Time.time * 1.5f - m_ButtonNumber * 1f).Remap(-1, 1, 0.5f, 0.75f));
        }

        if (m_IsSelected) {
            m_TargetScale = m_StartScale * Mathf.Sin(Time.time * 2.5f).Remap(-1, 1, 0.8f, 1.1f);
        }

        if (m_PreviewObj != null) {
            m_PreviewObj.transform.localScale = Vector3.Lerp(m_PreviewObj.transform.localScale, m_TargetScale, 0.05f) * m_TargetScaleMultiplier;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_IsSelected ? GetFloatPosition() : m_StartPosition, 0.05f);
    }

    public void Initialize(Action<CustomizeButton, int> onClick, int buttonNumber) {
        m_ButtonNumber = buttonNumber;
        m_Button.onButtonClick.AddListener(() => { onClick(this, buttonNumber); });
    }

    private Vector3 GetFloatPosition() {
        return m_FloatPosition + new Vector3(0, 0, Mathf.Sin((m_StartFloatTime - Time.time) * 2) * 0.5f);
    }

    public void SetLocked(bool locked) {
        m_BackgroundImage.color = locked ? Color.gray : Color.white;
        m_Button.enabled = !locked;
        lockIcon.gameObject.SetActive(locked);
    }

    public void SetSelected(bool selected) {
        m_IsSelected = selected;

        if (!m_IsSelected) {
            m_TargetScale = m_StartScale;
        }else {
            m_StartFloatTime = Time.time;
        }

        if (!lockIcon.gameObject.activeSelf)  m_BackgroundImage.color = selected ? new Color(0.7f, 1, 0.7f, 1) : Color.white;
    }

    public void SetPreview(GameObject prefab) {
        if(m_PreviewObj != null) {
            Destroy(m_PreviewObj);
        }

        if (prefab != null) {
            bool originalState = prefab.activeSelf;
            prefab.SetActive(false);
            m_PreviewObj = Instantiate(prefab);
            m_PreviewObj.transform.SetParent(previewParent, false);

            CleanPreview();

            m_PreviewObj.SetActive(true);
            prefab.SetActive(originalState);

            m_StartPos = m_PreviewObj.transform.localPosition;
            m_StartScale = m_PreviewObj.transform.localScale;
            m_TargetScale = m_StartScale;
        }
    }

    private void CleanPreview() {
        m_TargetScaleMultiplier = 1;

        CharacterMask mask = m_PreviewObj.GetComponent<CharacterMask>();
        if (mask != null) {
            m_PreviewObj.transform.localScale = Vector3.one;
            m_PreviewObj.transform.localPosition = new Vector3(0.41f, 0.225f, 0);
        }

        CustomizationSkinWrapper skin = m_PreviewObj.GetComponent<CustomizationSkinWrapper>();
        if (skin != null) {
            m_PreviewObj.transform.localScale = Vector3.one * 1.2f;
            m_PreviewObj.transform.localPosition = new Vector3(0, 0, 0);
        }

        TrajectoryRenderer trajectory = m_PreviewObj.GetComponent<TrajectoryRenderer>();
        if (trajectory != null) {
            DestroyImmediate(trajectory);
        }

        GameBall ball = m_PreviewObj.GetComponent<GameBall>();
        if (ball != null) {
            DestroyImmediate(ball);
            m_PreviewObj.transform.localPosition = Vector3.zero;
            m_PreviewObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }

        Rigidbody2D body = m_PreviewObj.GetComponent<Rigidbody2D>();
        if (body != null) {
            DestroyImmediate(body);
        }

        Collider collider = m_PreviewObj.GetComponent<Collider>();
        if (collider != null) {
            DestroyImmediate(collider);
        }

        ParticleSystem particle = m_PreviewObj.GetComponent<ParticleSystem>();
        if(particle != null) {
            m_PreviewObj.transform.localPosition = Vector3.zero;
            ParticleSystem.MainModule main = particle.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            m_TargetScaleMultiplier = 0.95f;
        }
    }
}
