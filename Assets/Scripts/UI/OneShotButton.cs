using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotButton : MonoBehaviour {

    public GameLevelCollection level;

    public void StartOneShot() {
        if (SaveDataManager.Instance.data.currentLives == 0) {
            VideoAdMenu.Instance.gameObject.SetActive(true);
        } else {
            LevelManager.Instance.LoadLevel(level.levels[0]);
        }
    }
}
