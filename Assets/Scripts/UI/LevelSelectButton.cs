using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Button3D))]
public class LevelSelectButton : MonoBehaviour {

    public GameObject lockImage;
    public TextMeshPro levelText;
    public SpriteRenderer[] stars;
    public SpriteRenderer backdrop;
    public Sprite starEmptySprite;
    public Sprite starAcquiredSprite;

    private Button3D m_Button3D;
    private GameLevelSet m_LevelSet;
    private bool m_IsLocked = false;

    private void Awake() {
        m_Button3D = GetComponent<Button3D>();
    }

    public void SetData(GameLevelSet levelData, bool isLocked) {
        m_LevelSet = levelData;
        m_IsLocked = isLocked;

        lockImage.SetActive(isLocked);

        levelText.text = "" + (levelData.index + 1);

        int currentScore = Mathf.Clamp(SaveDataManager.Instance.GetLevelScore(levelData.scene), 0, 3);
        for(int i = 0; i < stars.Length; i++) {
            stars[i].sprite = i < currentScore ? starAcquiredSprite : starEmptySprite;
        }

        backdrop.color = isLocked ? Color.gray : Color.white;

        if (!isLocked) {
            m_Button3D.onButtonClick.AddListener(() => { LevelManager.LoadLevel(levelData.scene); });
        }
    }
}
