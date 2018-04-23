using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviourSingleton<LevelSelectMenu> {

    public TextMeshPro titleText;

    private LevelSelectButton[] levelSelectButtons;

    private void Awake() {
        levelSelectButtons = GetComponentsInChildren<LevelSelectButton>();
    }

    public void CreateButtons(GameLevelCollection collection) {
        titleText.text = collection.displayName;

        GameLevelSet previousLevel = null;
        for(int i = 0; i < levelSelectButtons.Length; i++) {
            LevelSelectButton currentButton = levelSelectButtons[i];
            if(i >= collection.levels.Count) {
                currentButton.gameObject.SetActive(false);
            }else {
                GameLevelSet currentLevel = collection.levels[i];

                currentButton.gameObject.SetActive(true);

                bool isLocked = true;
                if (previousLevel == null ||
                    SaveDataManager.Instance.GetLevelScore(previousLevel.scene) != -1) isLocked = false;

                currentButton.SetData(currentLevel, isLocked);
                previousLevel = currentLevel;
            }
        }
    }

}
