﻿using System;
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

    private void Awake() {
        m_Button = GetComponent<Button3D>();
        m_BackgroundImage = GetComponent<SpriteRenderer>();
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

        m_PreviewObj.transform.localScale = Vector3.Lerp(m_PreviewObj.transform.localScale, m_TargetScale, 0.05f);
    }

    public void Initialize(Action<CustomizeButton, int> onClick, int buttonNumber) {
        m_ButtonNumber = buttonNumber;
        m_Button.onButtonClick.AddListener(() => { onClick(this, buttonNumber); });
    }

    public void SetLocked(bool locked) {
        m_BackgroundImage.color = locked ? Color.gray : Color.white;
        lockIcon.gameObject.SetActive(locked);
    }

    public void SetSelected(bool selected) {
        m_IsSelected = selected;

        if (!m_IsSelected) {
            m_TargetScale = m_StartScale;
        }

        m_BackgroundImage.color = selected ? new Color(0.5f, 1, 0.5f, 1) : Color.white;
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
        CharacterMask mask = m_PreviewObj.GetComponent<CharacterMask>();
        if (mask != null) {
            m_PreviewObj.transform.localScale = Vector3.one;
            m_PreviewObj.transform.localPosition = new Vector3(0.41f, 0.225f, 0);
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
        }
    }
}