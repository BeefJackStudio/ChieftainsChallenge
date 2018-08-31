using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviourSingleton<LevelSelectMenu> {

    public TextMeshPro titleText;
    public Button3D arrowLeft;
    public Button3D arrowRight;

    private LevelSelectButton[] levelSelectButtons;
    private int m_CurrentPage = 0;
    private GameLevelCollection m_LevelCollection;

    private void Awake() {
        levelSelectButtons = GetComponentsInChildren<LevelSelectButton>();

        arrowLeft.onButtonClick.AddListener(PreviousPage);
        arrowRight.onButtonClick.AddListener(NextPage);
    }

    public void CreateButtons(GameLevelCollection collection) {
        m_LevelCollection = collection;
        titleText.text = collection.displayName;
        SetPage(0);
    }

    private void NextPage() {
        SetPage(m_CurrentPage + 1);
    }

    private void PreviousPage() {
        SetPage(m_CurrentPage - 1);
    }

    private void SetPage(int page) {
        m_CurrentPage = page;

        int startIndex = page * levelSelectButtons.Length;
        int endIndex = startIndex + levelSelectButtons.Length;

        for (int i = 0; i < levelSelectButtons.Length; i++) {
            LevelSelectButton currentButton = levelSelectButtons[i];
            int levelIndex = startIndex + i;

            if (levelIndex >= m_LevelCollection.levels.Count) {
                currentButton.gameObject.SetActive(false);
            } else {
                bool isLocked = true;
                GameLevelSet currentLevel = m_LevelCollection.levels[levelIndex];
                currentLevel.index = levelIndex;

                if (levelIndex == 0 || SaveDataManager.Instance.GetLevelScore(m_LevelCollection.levels[levelIndex - 1].scene) != -1) {
                    isLocked = false;
                }

                currentButton.gameObject.SetActive(true);
                currentButton.SetData(currentLevel, isLocked);
            }
        }

        arrowLeft.SetActive(startIndex != 0);
        arrowRight.SetActive(endIndex < m_LevelCollection.levels.Count);
    }

}
