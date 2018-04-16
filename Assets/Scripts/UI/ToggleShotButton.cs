using UnityEngine;
using System.Collections;

public class ToggleShotButton : MonoBehaviour {

    public Player player;
    public GameObject shotButton;
    public BallCamera ballCam;

    private float timeAtBall;

    void Update() {
        if (player.state == Player.PlayerState.AtBall) {
            timeAtBall += Time.deltaTime;

            if (timeAtBall > 0.4f) {
                shotButton.SetActive(true);
                ballCam.m_state = BallCamera.CameraStat.FreeCam;
            }
        } else {
            timeAtBall = 0f;
            shotButton.SetActive(false);
        }

        if (GameManager.Instance.gameState == GameManager.GameState.GameFinished) {
            timeAtBall = 0f;
            shotButton.SetActive(false);
        }
    }
}
