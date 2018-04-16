using UnityEngine;
using System.Collections;

public class NotificationsManager : MonoBehaviour {

    public delegate void OnCourseStartHandler(GameManager.GameType currentGameType, int coursePar);
    public static event OnCourseStartHandler OnCourseStart;

    public void onCourseStart(GameManager.GameType currentGameType, int coursePar) {
        if (OnCourseStart != null)
            OnCourseStart(currentGameType, coursePar);
    }


    public delegate void OnGameStateChangeHandler(GameManager.GameState currentState);
    public static event OnGameStateChangeHandler OnGameStateChange;

    public void onGameStateChange(GameManager.GameState currentState) {
        if (OnGameStateChange != null)
            OnGameStateChange(currentState);
    }

    public delegate void OnGameStateStayHandler(GameManager.GameState currentState);
    public static event OnGameStateStayHandler OnGameStateStay;

    public void onGameStateStay(GameManager.GameState currentState) {
        if (OnGameStateStay != null)
            OnGameStateStay(currentState);
    }

    public delegate void OnInputStateChangeHandler(GameManager.InputState currentState, Vector2 backPos);
    public static event OnInputStateChangeHandler OnInputStateChange;

    public void onInputStateChange(GameManager.InputState currentState, Vector2 backPos) {
        if (OnInputStateChange != null)
            OnInputStateChange(currentState, backPos);
    }


    public delegate void OnSwingUIColourUpdateHandler(float shotTime);
    public static event OnSwingUIColourUpdateHandler OnSwingUIColourUpdate;

    public void onSwingUIColourUpdate(float shotTime) {
        if (OnSwingUIColourUpdate != null)
            OnSwingUIColourUpdate(shotTime);
    }

    public delegate void OnSwingBackAngleChangeHandler(float angle, float len, bool startLine);
    public static event OnSwingBackAngleChangeHandler OnSwingBackAngleChange;

    public void onSwingBackAngleChange(float angle, float len, bool startLine) {
        if (OnSwingBackAngleChange != null)
            OnSwingBackAngleChange(angle, len, startLine);
    }

    public delegate void OnPrintSwingAngleDataHandler(float angle, float power);
    public static event OnPrintSwingAngleDataHandler OnSwingAngleData;

    public void onPrintSwingAngleData(float angle, float power) {
        if (OnSwingAngleData != null)
            OnSwingAngleData(angle, power);
    }


    public delegate void ShotTakenHandler(float DistanceToHole);
    public static event ShotTakenHandler OnShotTaken;

    public void onShotTaken(float DistanceToHole) {
        if (OnShotTaken != null)
            OnShotTaken(DistanceToHole);
    }

    public delegate void BallStoppedHandler(float DistanceToHole);
    public static event BallStoppedHandler OnBallStopped;

    public void onBallStopped(float DistanceToHole) {
        if (OnBallStopped != null)
            OnBallStopped(DistanceToHole);
    }

    public delegate void StartPowerShotHandler();
    public static event StartPowerShotHandler StartPowerShot;

    public void startPowerShot() {
        if (StartPowerShot != null)
            StartPowerShot();
    }

    public delegate void UpdateInGameUIHandler(int distance, int currentScore, int shotCount);
    public static event UpdateInGameUIHandler UpdateInGameUI;

    public void updateInGameUI(int distance, int currentScore, int shotCount) {
        if (UpdateInGameUI != null)
            UpdateInGameUI(distance, currentScore, shotCount);
    }

    public delegate void OnResetPlayHandler();
    public static event OnResetPlayHandler OnResetPlay;

    public void onResetPlay() {
        if (OnResetPlay != null)
            OnResetPlay();
    }

    public delegate void OnEndGameStartHandler(GameManager.GameType gameType, bool gameFinished, Score score);
    public static event OnEndGameStartHandler OnEndGameStart;

    public void onEndGameStart(GameManager.GameType gameType, bool levelBeaten, Score score) {
        if (OnEndGameStart != null)
            OnEndGameStart(gameType, levelBeaten, score);
    }

    public delegate void UpdatePlayerTargetFrameHandler(float target);
    public static event UpdatePlayerTargetFrameHandler UpdatePlayerTargetFrame;

    public void updatePlayerTargetFrame(float target) {
        if (UpdatePlayerTargetFrame != null)
            UpdatePlayerTargetFrame(target);
    }

    public delegate void OnPlayerStateChangeHandler(Player.PlayerState playerState, bool bPlayerSelected);
    public static event OnPlayerStateChangeHandler OnPlayerStateChange;

    public void onPlayerStateChange(Player.PlayerState playerState, bool bPlayerSelected) {
        if (OnPlayerStateChange != null)
            OnPlayerStateChange(playerState, bPlayerSelected);
    }

    public delegate void OnBallMovementHandler(float airTime, Vector3 ballPosition);
    public static event OnBallMovementHandler OnBallMovement;

    public void onBallMovement(float airTime, Vector3 ballPosition) {
        if (OnBallMovement != null)
            OnBallMovement(airTime, ballPosition);
    }

    public delegate void OnBallEnableHandler(Transform ball);
    public static event OnBallEnableHandler OnBallEnabled;

    public void onBallEnabled(Transform ball) {
        if (OnBallEnabled != null)
            OnBallEnabled(ball);
    }
}
