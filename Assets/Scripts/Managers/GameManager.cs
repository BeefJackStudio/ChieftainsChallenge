using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[System.Serializable]
public class StarScores {
    public int OneStar = 1000;
    public int TwoStar = 2000;
    public int ThreeStar = 3000;
}

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public NotificationsManager notificationsManager;
    public MaskManager maskManager;
    public SwingInputUI swingInputUI;

    void ActivateMask() {
        if (maskManager != null) {
            notificationsManager.onBallEnabled(ball.transform);
            maskManager.ActivateMask();
        }
    }

    public enum GameState {
        preGame,
        WaitForCannon,
        FreeView,
        WaitForShot,
        FollowBall,
        GameFinished
    }

    public enum InputState {
        PlayerSelected,
        SwingBack,
        SwingForword
    }

    public enum GameType {
        Basic,
        HoleInOne,
        Cannon,
    }

    public StarScores MinimumScores;

    public GameState gameState;
    public InputState inputState;
    public GameType gameType;

    public Ball ball;

    public Score score;

    public int coursePar;
    public int shotCount { get; set; }

    public float zDepthEnvironment = 40.0f; // used when setting sorting orders for sprite layers

    private Vector2 startPos;
    private Vector2 backPos;
    private Vector2 positionDiff;

    private bool touchFinished;
    private bool gameFinished = false;
    private bool bPlayerSelected;

    private float arrowAngle;
    private float shotTime;
    private float swingAdd;

    private int currentScore;

    private Transform Hole;

    private Player.PlayerState playerState;

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        InputManager.inputMoved += TouchInput;
        InputManager.inputFinished += TouchEnd;
        NotificationsManager.OnPlayerStateChange += NotificationsManager_OnPlayerStateChange;
        NotificationsManager.OnBallMovement += NotificationsManager_OnBallMovement;

        notificationsManager.onCourseStart(gameType, coursePar);

        Time.timeScale = 1;
        positionDiff = Vector2.zero;
        score = new Score(MinimumScores);

        if (gameType == GameType.Cannon)
            shotCount = -1;

        if (Hole == null) {
            GameObject obj = GameObject.Find("Hole");

            if (obj != null) {
                Hole = obj.transform;
            } else
                Debug.LogError("CANT FIND HOLE IN SCENE!!!!");
        }

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnPlayerStateChange -= NotificationsManager_OnPlayerStateChange;
        NotificationsManager.OnBallMovement -= NotificationsManager_OnBallMovement;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnBallMovement(float airTime, Vector3 ballPosition) {
        if (Hole == null) {
            GameObject obj = GameObject.Find("Hole");

            if (obj != null) {
                Hole = obj.transform;
            } else
                Debug.LogError("CANT FIND HOLE IN SCENE!!!!");
        }

        int distanceToHole = (int)(ballPosition - Hole.position).magnitude;
        UpdateScore(airTime, distanceToHole);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnPlayerStateChange(Player.PlayerState thisPlayerState, bool thisbPlayerSelected) {
        playerState = thisPlayerState;
        bPlayerSelected = thisbPlayerSelected;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void Update() {
        switch (gameState) {
            case GameState.preGame:
                notificationsManager.onGameStateStay(gameState);
                break;
            case GameState.FreeView: {
                    notificationsManager.onGameStateStay(gameState);
                    break;
                }
            case GameState.WaitForShot: {
                    notificationsManager.onGameStateStay(gameState);
                    CheckInput();
                    break;
                }
            case GameState.FollowBall: {
                    notificationsManager.onGameStateStay(gameState);

                    if (playerState == Player.PlayerState.AtBall && ball.GetComponent<SpriteRenderer>().enabled && ball.GetComponent<Rigidbody2D>().velocity.magnitude.Aprox(0, 0.1f)) {
                        if ((gameType == GameType.HoleInOne && shotCount > 0) || gameFinished) {
                            if (gameFinished) {
                                score.InHole = true;
                            }
                            gameState = GameState.GameFinished;

                            notificationsManager.onGameStateChange(gameState);
                        } else {
                            ResetShot();
                        }
                    }
                    break;
                }
            case GameState.GameFinished: {
                    notificationsManager.onGameStateStay(gameState);

                    score.ShotCount = shotCount;
                    score.BallInAirTime = ball.airCount;
                    score.Par = coursePar;

                    notificationsManager.onEndGameStart(gameType, gameFinished, score);

                    break;
                }
        }
        touchFinished = false;
    }

    float DistanceToHole() {
        return (ball.transform.position - Hole.position).magnitude;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void OnShotTaken() {
        shotCount += 1;
        notificationsManager.onShotTaken(DistanceToHole());
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void ResetShot() {
        gameState = GameState.FreeView;
        notificationsManager.onGameStateChange(gameState);
        notificationsManager.onBallStopped(DistanceToHole());
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void CheckInput() {
        shotTime += Time.deltaTime;
        if (playerState == Player.PlayerState.WaitToHitBall && bPlayerSelected) {
            switch (inputState) {
                case InputState.PlayerSelected: {
                        if (positionDiff.x < -0.1) {
                            inputState = InputState.SwingBack;
                            startPos = InputManager.positionLast;
                            Vector2 ball2d = Camera.main.WorldToScreenPoint(ball.transform.position);
                            ball2d.y = InputManager.InputToScreenPoint(startPos).y;
                            startPos = ball2d;

                            notificationsManager.onInputStateChange(inputState, ball2d);

                            shotTime = 0;
                        } else {
                            notificationsManager.updatePlayerTargetFrame(0.28f);
                        }
                        positionDiff = Vector2.zero;
                        swingAdd = 0;
                        break;
                    }
                case InputState.SwingBack: {
                        notificationsManager.onSwingUIColourUpdate(shotTime);

                        Vector2 posLast = InputManager.InputToScreenPoint(InputManager.positionLast);
                        posLast.x = Mathf.Max(posLast.x, maskManager.startOfPowerBar.position.x);

                        if (positionDiff.x > 0) {
                            backPos = posLast;
                            inputState = InputState.SwingForword;

                            notificationsManager.onInputStateChange(inputState, backPos);

                            touchFinished = false;
                        } else {


                            Vector2 from = startPos - new Vector2(posLast.x, startPos.y);
                            Vector2 to = posLast - startPos;
                            float angle;
                            angle = Mathf.DeltaAngle(Mathf.Atan2(from.y, from.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg);

                            float len;
                            if (posLast.x < startPos.x) {
                                len = (startPos - posLast).magnitude * InputManager.screenScale.x;
                                swingAdd = (1.0f / (startPos.x - maskManager.startOfPowerBar.position.x)) * (startPos.x - posLast.x);
                                notificationsManager.updatePlayerTargetFrame((1 - Mathf.Clamp01((swingAdd / 4))) * 0.28f);
                            } else {
                                len = 0;
                                swingAdd = 0;
                                notificationsManager.updatePlayerTargetFrame((1 - Mathf.Clamp01((swingAdd / 4))) * 0.28f);
                            }

                            notificationsManager.onSwingBackAngleChange(angle, len, true);
                        }
                        if (touchFinished) {
                            ResetPlay();
                        }
                        positionDiff = Vector2.zero;
                        break;
                    }
                case InputState.SwingForword: {
                        notificationsManager.onSwingUIColourUpdate(shotTime);

                        if (InputManager.InputToScreenPoint(InputManager.positionLast).x >= startPos.x) {
                            Vector2 from = new Vector2(startPos.x, backPos.y) - backPos;
                            Vector2 to = InputManager.InputToScreenPoint(InputManager.positionLast) - backPos;
                            float angle = (Vector2.Angle(from, to));
                            angle = Mathf.DeltaAngle(Mathf.Atan2(from.y, from.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg);

                            if (angle < 0) {
                                angle = 0;
                            }
                            ball.UpdateForceAngle(angle);
                            float power;
                            if (maskManager.maskIndex != -1) {
                                power = maskManager.mask[maskManager.maskIndex].powerCurve.Evaluate(1 - Mathf.Clamp01((swingAdd)));
                            } else {
                                power = maskManager.defaultMask.powerCurve.Evaluate(1 - Mathf.Clamp01((swingAdd)));
                            }
                            float bones = swingInputUI.TimeBones.Evaluate(shotTime);

                            power *= bones;
                            ball.UpdatePower(power);

                            if (power.Aprox(2, 0.05f)) {
                                notificationsManager.startPowerShot();
                                score.SmashHitsCount++;
                            }

                            gameState = GameState.FollowBall;
                            notificationsManager.onGameStateChange(gameState);
                            inputState = InputState.PlayerSelected;
                            notificationsManager.onInputStateChange(inputState, new Vector2());

                            positionDiff = Vector2.zero;

                            notificationsManager.onPrintSwingAngleData(angle, power);
                        } else {
                            Vector2 from = new Vector2(startPos.x, backPos.y) - backPos;
                            Vector2 to = InputManager.InputToScreenPoint(InputManager.positionLast) - backPos;
                            float angle = (Vector2.Angle(from, to));
                            angle = Mathf.DeltaAngle(Mathf.Atan2(from.y, from.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg);

                            notificationsManager.onSwingBackAngleChange(angle, 0, false);

                            if (touchFinished) {
                                ResetPlay();
                            }
                            positionDiff = Vector2.zero;
                        }

                        break;
                    }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void TakeShotButtonPress() {
        gameState = GameState.WaitForShot;
        notificationsManager.onGameStateChange(gameState);
        positionDiff = Vector2.zero;
        ball.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void TakeShotReturn() {
        gameState = GameState.FreeView;
        notificationsManager.onGameStateChange(gameState);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void TouchInput() {
        positionDiff += InputManager.positionDelta * Time.deltaTime;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void TouchEnd() {
        if (gameState == GameState.WaitForShot) {
            touchFinished = true;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    void ResetPlay() {
        touchFinished = false;
        inputState = InputState.PlayerSelected;
        notificationsManager.onInputStateChange(inputState, new Vector2());
        notificationsManager.onResetPlay();
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void StartGame() {
        gameState = GameState.FollowBall;
        notificationsManager.onGameStateChange(gameState);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void UpdateScore(float airTime, int distanceToHole) {
        if (score != null) {
            score.ShotCount = shotCount;
            score.BallInAirTime = airTime;
            score.Par = coursePar;
            score.InHole = false;
            currentScore = score.CalcScore();
        }

        notificationsManager.updateInGameUI(distanceToHole, currentScore, shotCount);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------

    public void LoadLevel(string level) {
        Application.LoadLevel(level);
    }




    [ExecuteInEditMode]
    void OnValidate() {
        MinimumScores.OneStar = Mathf.Clamp(MinimumScores.OneStar, 0, MinimumScores.TwoStar - 1);
        MinimumScores.TwoStar = Mathf.Clamp(MinimumScores.TwoStar, MinimumScores.OneStar, MinimumScores.ThreeStar - 1);
        MinimumScores.ThreeStar = Mathf.Max(MinimumScores.TwoStar, MinimumScores.ThreeStar);
    }
}
