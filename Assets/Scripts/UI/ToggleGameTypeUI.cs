using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleGameTypeUI : MonoBehaviour {

    public GameObject takeShotButton;

    public List<GameObject> cannonModeUI;

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType thisGameType, int coursePar) {
        switch (thisGameType) {
            case GameManager.GameType.Basic:
                takeShotButton.SetActive(true);
                DeactivateList(cannonModeUI);
                break;

            case GameManager.GameType.Cannon:
                if (takeShotButton != null)
                    takeShotButton.SetActive(false);
                break;

            case GameManager.GameType.HoleInOne:
                takeShotButton.SetActive(true);
                DeactivateList(cannonModeUI);
                break;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    void DeactivateList(List<GameObject> thisList) {
        foreach (GameObject item in thisList) {
            item.SetActive(false);
        }
    }
}
