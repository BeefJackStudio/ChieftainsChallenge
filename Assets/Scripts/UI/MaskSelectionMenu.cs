using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskSelectionMenu : MonoBehaviour {
    public RectTransform container;

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

    private void Awake() {
        m_BackgroundFade = GetComponent<Image>();

        m_ContainerHideY = container.anchoredPosition.y;
        m_ContainerShowY = 5;

        m_ContainerTargetY = m_ContainerHideY;

        for(int i = 0; i < 4; i++) {
            CharacterMask mask = masksToGenerate[i];
            Button button = maskSelectButtons[i];

            button.GetComponentsInChildren<Image>()[1].sprite = mask.GetComponentInChildren<SpriteRenderer>().sprite;

            button.onClick.AddListener(() => {
                SelectMask(mask);
            });
        }

        maskSelectButtons[0].onClick.Invoke();
    }

    private void Update() {
        Vector2 pos = container.anchoredPosition;
        pos = Vector2.Lerp(pos, new Vector2(pos.x, m_ContainerTargetY), 0.1f);
        container.anchoredPosition = pos;

        m_BackgroundColor.a = Mathf.Lerp(m_BackgroundColor.a, m_BackgroundAlphaTarget, 0.1f);
        m_BackgroundFade.color = m_BackgroundColor;

        m_BackgroundFade.raycastTarget = m_BackgroundColor.a >= 0.1f;
    }

    private void SelectMask(CharacterMask mask) {
        maskInformationImage.sprite = mask.GetComponentInChildren<SpriteRenderer>().sprite;
        maskInformationTitle.text = mask.displayName;
        maskInformationDescription.text = mask.description;
        m_SelectedMask = mask;
    }

    public void ApplyMask() {
        LevelInstance.Instance.ApplyCharacterMask(m_SelectedMask);
        Hide();
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
}
