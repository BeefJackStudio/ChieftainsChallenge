﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskSelectionMenu : MonoBehaviourSingleton<MaskSelectionMenu> {
    public RectTransform container;
    public RectTransform openButton;

    [Header("Mask selector")]
    public CharacterMask[] masksToGenerate;
    public Button[] maskSelectButtons;

    [Header("Mask details")]
    public Image maskInformationImage;
    public TextMeshProUGUI maskInformationTitle;
    public TextMeshProUGUI maskInformationDescription;

    private Image m_BackgroundFade;
    private float m_ContainerHideY;
    private float m_ContainerShowY;
    private float m_ContainerTargetY;
    private CharacterMask m_SelectedMask;
    private Color m_BackgroundColor = new Color(0, 0, 0, 0);
    private float m_BackgroundAlphaTarget = 0;
    private float m_OpenButtonHideY;
    private float m_OpenButtonShowY;
    private float m_OpenButtonTargetY;

    private CharacterMask[] m_SkinnedMasks;
    private int m_SelectedIndex = 0;

    private void Awake() {
        Instance = this;

        m_BackgroundFade = GetComponent<Image>();

        m_ContainerHideY = container.anchoredPosition.y;
        m_ContainerShowY = 5;
        m_ContainerTargetY = m_ContainerHideY;

        m_OpenButtonHideY = openButton.sizeDelta.y;
        m_OpenButtonShowY = openButton.anchoredPosition.y;
        m_OpenButtonTargetY = m_OpenButtonHideY;
        openButton.anchoredPosition = new Vector2(openButton.anchoredPosition.x, m_OpenButtonTargetY);

        m_SkinnedMasks = new CharacterMask[4] { CustomizationSelected.woodenMask.Obj, CustomizationSelected.hawkMask.Obj, CustomizationSelected.royalMask.Obj, CustomizationSelected.skullMask.Obj };

        for (int i = 0; i < 4; i++) {
            CharacterMask mask = masksToGenerate[i];
            CharacterMask skinnedMask = m_SkinnedMasks[i];

            mask.CopySettingsTo(skinnedMask);

            Button button = maskSelectButtons[i];

            button.GetComponentsInChildren<Image>()[1].sprite = skinnedMask.GetComponentInChildren<SpriteRenderer>().sprite;

            button.onClick.AddListener(() => {
                SelectMask(skinnedMask);
            });
        }

        SelectMask(m_SkinnedMasks[0]);
    }

    private void Start() {
        ApplyMask(SaveDataManager.Instance.data.maskTypeSelected);
    }

    private void Update() {
        Vector2 containerPos = container.anchoredPosition;
        containerPos = Vector2.Lerp(containerPos, new Vector2(containerPos.x, m_ContainerTargetY), 0.1f);
        container.anchoredPosition = containerPos;

        m_BackgroundColor.a = Mathf.Lerp(m_BackgroundColor.a, m_BackgroundAlphaTarget, 0.1f);
        m_BackgroundFade.color = m_BackgroundColor;

        m_BackgroundFade.raycastTarget = m_BackgroundColor.a >= 0.1f;

        Vector2 openButtonPos = openButton.anchoredPosition;
        openButtonPos = Vector2.Lerp(openButtonPos, new Vector2(openButtonPos.x, m_OpenButtonTargetY), 0.1f);
        openButton.anchoredPosition = openButtonPos;
    }

    private void SelectMask(CharacterMask mask) {
        maskInformationImage.sprite = mask.GetComponentInChildren<SpriteRenderer>().sprite;
        maskInformationTitle.text = mask.displayName;
        maskInformationDescription.text = mask.description;
        m_SelectedMask = mask;

        for(int i = 0; i < m_SkinnedMasks.Length; i++) {
            if(m_SkinnedMasks[i] == mask) {
                m_SelectedIndex = i;
                break;
            }
        }
    }

    public void ApplyMask() {
        LevelInstance.Instance.ApplyCharacterMask(m_SelectedMask);
        SaveDataManager.Instance.data.maskTypeSelected = m_SelectedIndex;
        Hide();
    }

    public void ApplyMask(int type) {
        SelectMask(m_SkinnedMasks[type]);
        ApplyMask();
    }

    [ContextMenu("Show")]
    public void Show() {
        m_BackgroundAlphaTarget = 0.3f;
        m_ContainerTargetY = m_ContainerShowY;
    }

    [ContextMenu("Hide")]
    public void Hide() {
        m_BackgroundAlphaTarget = 0;
        m_ContainerTargetY = m_ContainerHideY;
    }

    public void ShowOpenButton() {
        m_OpenButtonTargetY = m_OpenButtonShowY;
    }

    public void HideOpenButton() {
        m_OpenButtonTargetY = m_OpenButtonHideY;
    }
}
