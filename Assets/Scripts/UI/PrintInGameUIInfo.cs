using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrintInGameUIInfo : MonoBehaviour {

    public Text distance;
    public Text strokes;
    public Text par;
    public Text scoreText;

    //----------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;
        NotificationsManager.UpdateInGameUI += NotificationsManager_UpdateInGameUI;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
        NotificationsManager.UpdateInGameUI -= NotificationsManager_UpdateInGameUI;

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType currentGameType, int coursePar) {
        par.text = "Par: " + coursePar.ToString();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_UpdateInGameUI(int distanceToHole, int currentScore, int shotCount) {
        if (shotCount < 0)
            shotCount = 0;

        distance.text = "Distance: " + distanceToHole.ToString();
        scoreText.text = currentScore.ToString("D9");
        strokes.text = "Strokes: " + shotCount.ToString();
    }

}
