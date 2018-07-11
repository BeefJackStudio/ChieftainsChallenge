using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallSelectionMenu : MonoBehaviourSingleton<BallSelectionMenu> {
    public RectTransform container;
    public RectTransform openButton;

    [Header("Mask selector")]
    public GameBall[] ballsToGenerate;
    public Button[] ballSelectButtons;

    [Header("Mask details")]
    public Image ballInformationImage;
    public TextMeshProUGUI ballInformationTitle;
    public TextMeshProUGUI ballInformationDescription;

    private Image m_BackgroundFade;
    private float m_ContainerHideY;
    private float m_ContainerShowY;
    private float m_ContainerTargetY;
    private GameBall m_SelectedBall;
    private Color m_BackgroundColor = new Color(0, 0, 0, 0);
    private float m_BackgroundAlphaTarget = 0;
    private float m_OpenButtonHideY;
    private float m_OpenButtonShowY;
    private float m_OpenButtonTargetY;

    private void Awake() {
        m_BackgroundFade = GetComponent<Image>();

        m_ContainerHideY = container.anchoredPosition.y;
        m_ContainerShowY = 5;
        m_ContainerTargetY = m_ContainerHideY;

        m_OpenButtonHideY = openButton.sizeDelta.y;
        m_OpenButtonShowY = openButton.anchoredPosition.y;
        m_OpenButtonTargetY = m_OpenButtonHideY;
        openButton.anchoredPosition = new Vector2(openButton.anchoredPosition.x, m_OpenButtonTargetY);

        GameBall[] skinnedBalls = new GameBall[4] { CustomizationSelected.stoneBall.Obj, CustomizationSelected.mudBall.Obj, CustomizationSelected.beachBall.Obj, CustomizationSelected.sunBall.Obj };

        for (int i = 0; i < 4; i++) {
            GameBall ball = ballsToGenerate[i];
            GameBall skinnedBall = skinnedBalls[i];

            ball.CopySettingsTo(skinnedBall);

            Button button = ballSelectButtons[i];

            button.GetComponentsInChildren<Image>()[1].sprite = skinnedBall.GetComponentInChildren<SpriteRenderer>().sprite;

            button.onClick.AddListener(() => {
                SelectBall(skinnedBall);
            });
        }

        ballSelectButtons[0].onClick.Invoke();
    }

    private void Start() {
        if (LevelInstance.Instance.useCannon) {
            openButton.anchoredPosition = new Vector2(0, openButton.anchoredPosition.y);
        }
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

    private void SelectBall(GameBall ball) {
        ballInformationImage.sprite = ball.GetComponentInChildren<SpriteRenderer>().sprite;
        ballInformationTitle.text = ball.displayName;
        ballInformationDescription.text = ball.description;
        m_SelectedBall = ball;
    }

    public void ApplyBall() {
        GameObject newBall = Instantiate(m_SelectedBall).gameObject;
        LevelInstance.Instance.SetBall(newBall.GetComponent<GameBall>());
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

    public void ShowOpenButton() {
        m_OpenButtonTargetY = m_OpenButtonShowY;
    }

    public void HideOpenButton() {
        m_OpenButtonTargetY = m_OpenButtonHideY;
    }
}
