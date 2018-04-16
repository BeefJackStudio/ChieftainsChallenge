using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrintScores : MonoBehaviour {

    public GameManager gameManager;
    public Text smashHitCount;
    public Text smashHitScore;
    public Text airTimeCount;
    public Text airTimeScore;
    public Text dynamicObjectsCount;
    public Text dynamicObjectsScore;
    public Text parText;
    public Text parScore;
    public Text totalScore;
    public PauseMenu pauseMenu;
    public Text GameOverMessage;

    public SetStars setStarsUI;

    private float gomVelocity;

    public List<AudioClip> endgameAudio;

    void OnEnable() {
        NotificationsManager.OnEndGameStart += NotificationsManager_OnEndGameStart;

        gomVelocity = 0;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnEndGameStart -= NotificationsManager_OnEndGameStart;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnEndGameStart(GameManager.GameType gameType, bool gameFinished, Score score) {
        switch (gameType) {
            case GameManager.GameType.HoleInOne:
                string message;

                if (gameFinished)
                    message = "Winner!";
                else
                    message = "Fail!";

                PrintEndGameMessage(message);
                StartCoroutine(PlayEndGameMessage());

                break;

            case GameManager.GameType.Basic:
                PrintScoreBreakdown(score);
                break;

            case GameManager.GameType.Cannon:
                PrintScoreBreakdown(score);
                break;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    void PrintScoreBreakdown(Score score) {
        score.InHole = true;
        int total = score.CalcScore();
        totalScore.text = total.ToString();

        smashHitCount.text = score.SmashHitsCount.ToString();
        smashHitScore.text = score.SmashHitTotal.ToString();
        airTimeCount.text = score.BallInAirTime.ToString("F2");
        airTimeScore.text = score.BallInAirTotal.ToString("F0");
        dynamicObjectsCount.text = score.DynamicObjectsHit.ToString();
        dynamicObjectsScore.text = score.DynamicObjectTotal.ToString();
        parText.text = score.ShotText;
        parScore.text = score.ShotTotal.ToString();

        SaveDataManager.Instance.SetLevelScore("", score.GetStarScore());

        setStarsUI.SetStarCount(score.GetStarScore());

        pauseMenu.SwitchEndGame();
        pauseMenu.ActivatePause();

        StartCoroutine(playSound());
    }

    IEnumerator playSound() {
        if (!GetComponent<AudioSource>().isPlaying)
            SoundManager.playSFXOneShot(GetComponent<AudioSource>(), endgameAudio[Random.Range(0, endgameAudio.Count)]);

        yield return new WaitForSeconds(1);

        StartCoroutine(playSound());
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------

    void PrintEndGameMessage(string message) {
        GameOverMessage.text = message;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    IEnumerator PlayEndGameMessage() {
        if (GameOverMessage.rectTransform.anchoredPosition.y.Aprox(10, 1) && gomVelocity.Aprox(0, .5f)) {
            Application.LoadLevel("MainMenu");
        } else {
            gomVelocity += 10f * Time.deltaTime;
            gomVelocity = Mathf.Clamp(gomVelocity, -1225, 1225);
            Vector2 pos = GameOverMessage.rectTransform.anchoredPosition;
            pos.y -= gomVelocity;
            if (pos.y < 10) {
                gomVelocity = -(gomVelocity * 0.5f);
                pos.y = 10;
            }
            GameOverMessage.rectTransform.anchoredPosition = pos;

            yield return null;
        }
    }
}
