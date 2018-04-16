using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : ResettableObject {
    // ----------
    // PROPERTIES
    // ----------

    public NotificationsManager notificationManager;

    public enum PlayerState {
        WaitForCannon,
        WaitForPlayer,  //wait for player to press take shot
        RunToBall,      //player runs to ball
        AtBall,         //wait at ball for player to start taking shot
        WaitToHitBall,  //player is takeing shot
        PlayHitBall     //hit the ball
    }

    private Ball _ball;
    public Ball ball {
        get {
            if (_ball == null) {
                _ball = FindObjectOfType(typeof(Ball)) as Ball;
            }

            return _ball;
        }
    }

    public float playerSpeed;          // speed the player is going
    public float playerMaxSpeed;        // max speed the player can go
    public float playerMaxAccel;        // accelaration for the player
    public float maxSlopeAngle = 70f;

    public PlayerState state;           // stat of the player
    private Animation animationComp;

    public float targetFrame;           // the animation frame in the swing we want to be at

    public bool bFDown;                 //is your finger down on the screen
    public bool bPlayerSeleceted;       //have we selected the player

    public BallCamera gameCamera;           //the game camera

    public SpriteRenderer Mask;
    public SpriteRenderer MaskOutline;

    public List<SpriteRenderer> characterSprites;

    public AnimationCurve slowDownSpline;

    public AudioClip runSFX;
    public AudioClip hitBall;

    private bool ballair = false;

    public LayerMask ground;
    public float gravity = 10f;

    AudioSource audioSource;

    public float TargetFrame {
        set {
            targetFrame = value;
        }

        get {
            return targetFrame;
        }
    }

    public bool readyToTakeShot { get; set; }

    public GameObject smoke;

    //These are part of a hack to prevent the player getting stuck on slopes. Please refactor
    private Vector2 lastPos;
    private Vector2 currentPos;
    public bool playerStuck;
    public float playerStuckTimer;
    /// /////////////////////////////////////////////////////////////////////////////////////


    // ----------
    // METHODS
    // ----------

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void OnEnable() {
        animationComp = GetComponentInChildren<Animation>();
        animationComp.CrossFade("chieftain_idle", 0.25f);

        InputManager.inputStart += FingerDown;
        InputManager.inputFinished += FingerUp;

        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;
        NotificationsManager.OnResetPlay += NotificationsManager_OnResetPlay;
        NotificationsManager.UpdatePlayerTargetFrame += NotificationsManager_UpdatePlayerTargetFrame;

        bFDown = false;
        bPlayerSeleceted = false;

        notificationManager.onPlayerStateChange(state, bPlayerSeleceted);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
        NotificationsManager.OnResetPlay -= NotificationsManager_OnResetPlay;
        NotificationsManager.UpdatePlayerTargetFrame -= NotificationsManager_UpdatePlayerTargetFrame;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType thisGameType, int coursePar) {
        switch (thisGameType) {
            case GameManager.GameType.Basic:
                ChangeState(PlayerState.RunToBall);
                break;
            case GameManager.GameType.HoleInOne:
                ChangeState(PlayerState.RunToBall);
                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.FollowBall:
                ChangeState(PlayerState.PlayHitBall);
                break;
            case GameManager.GameState.FreeView:
                if (state != PlayerState.WaitForCannon)
                    ChangeState(PlayerState.WaitForPlayer);
                break;
            case GameManager.GameState.WaitForShot:
                readyToTakeShot = true;
                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_UpdatePlayerTargetFrame(float target) {
        TargetFrame = target;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnResetPlay() {
        bPlayerSeleceted = true;
        ChangeState(PlayerState.WaitToHitBall);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    public override void ResetMe() {
        base.ResetMe();

        OnEnable();
    }

    // Update is called once per frame
    void Update() {
        if (_ball == null || !_ball.gameObject.activeSelf) // find current active ball in game (as needed)
        {
            Ball[] balls = FindObjectsOfType<Ball>();
            for (int i = 0; i < balls.Length; ++i) {
                if (balls[i].gameObject.activeSelf) {
                    _ball = balls[i]; // found it
                    break; // out of loop
                }
            }
        }

        ApplyGravity();

        CheckSlope();

        //These are part of a hack to prevent the player getting stuck on slopes. Please refactor
        CheckIfStuckOnSlope();
        ////////////////////////////////////////////////////////////////////////////////////////

        switch (state) {
            case PlayerState.WaitForPlayer: {
                    //wait for the player to stop looking round the screen
                    //but if the ball move to fare away go run to the ball

                    float distToBall = (transform.position - ball.transform.position).magnitude;
                    {
                        animationComp.CrossFade("chieftain_run", 0.25f);
                        state = PlayerState.RunToBall;
                        notificationManager.onPlayerStateChange(state, bPlayerSeleceted);
                    }

                    break;
                }

            case PlayerState.RunToBall: {
                    //get to the ball
                    if (audioSource.clip != runSFX) {
                        audioSource.clip = runSFX;
                        audioSource.loop = true;
                        audioSource.Play();
                        audioSource.volume = 0;
                    }
                    if ((ball.transform.parent == null || ball.transform.parent.GetComponent<Ball>() == null)) {
                        if (ball.GetComponent<SpriteRenderer>().enabled)
                            RunToBall();
                    } else {
                        if (ball.transform.parent.GetComponent<SpriteRenderer>().enabled)
                            RunToBall();

                    }
                    break;
                }

            case PlayerState.AtBall: {
                    // yball();
                    if (audioSource.volume > 0) {
                        audioSource.volume -= Time.deltaTime;
                        if (audioSource.volume < 0) {
                            audioSource.volume = 0;
                        }
                    }

                    //****This is a hack - will need refactoring
                    if (readyToTakeShot) {
                        bPlayerSeleceted = true;
                        ChangeState(PlayerState.WaitToHitBall);
                        notificationManager.onPlayerStateChange(state, bPlayerSeleceted);
                        bFDown = false;
                        readyToTakeShot = false;
                    }
                    //*****

                    else {
                        float distToBall = Mathf.Abs(transform.position.x - ball.transform.position.x);
                        if (distToBall > 0.5f)// && ball.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.1f)
                        {
                            animationComp.CrossFade("chieftain_run", 0.25f);
                            state = PlayerState.RunToBall;
                            notificationManager.onPlayerStateChange(state, bPlayerSeleceted);
                            gameCamera.State = BallCamera.CameraStat.TrackBall;
                        }

                    }
                    gameCamera.targetFOV = 65;
                    break;
                }

            case PlayerState.WaitToHitBall: {
                    //bPlayerSeleceted = true;
                    //the target fram will be updated from the game manager and we update the animation here
                    float value = Mathf.Lerp(animationComp["chieftain_hit_ball"].normalizedTime, targetFrame, Time.deltaTime * 10);
                    animationComp["chieftain_hit_ball"].normalizedTime = value;
                    animationComp["chieftain_hit_ball"].enabled = true;
                    animationComp.Sample();
                    animationComp["chieftain_hit_ball"].enabled = false;
                    gameCamera.targetFOV = 35;
                    gameCamera.m_state = BallCamera.CameraStat.TrackBall;

                    break;
                }

            case PlayerState.PlayHitBall: {
                    //we need to hit the ball so play the animation out
                    if (!animationComp.IsPlaying("chieftain_hit_ball")) {
                        //we have finsihed hitting the ball so go back to wait for player
                        animationComp.CrossFade("chieftain_idle", 0.25f);
                        state = PlayerState.WaitForPlayer;
                        notificationManager.onPlayerStateChange(state, bPlayerSeleceted);
                    }
                    //were playing hit animation and when we get to the right point in the animation
                    else if (animationComp["chieftain_hit_ball"].normalizedTime >= 0.28 && ball.GetComponent<Rigidbody2D>().velocity.magnitude < 0.5f) {
                        //fire the ball of
                        ball.ShootBall();
                        gameCamera.targetFOV = 85;
                        audioSource.volume = SoundManager.SoundEffectVolume;
                        audioSource.clip = hitBall;
                        audioSource.Play();
                        audioSource.loop = false;
                    }

                    break;
                }
        }
    }
    private void yball() {
        var pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, ball.transform.position.y, 2.0f);
        //  transform.localScale = new Vector3(2f, 2f, 2f);
        if (pos.y > ball.transform.position.y) {

            pos.y = Mathf.Lerp(pos.y, ball.transform.position.y, 2.0f);
        }
        if (pos.y < ball.transform.position.y) {

            pos.y = Mathf.Lerp(pos.y, ball.transform.position.y, 2.0f);
        }

    }
    private void RunToBall() {
        //if (GameManager.Instance.GameFinished)
        //    return;

        //deadzone prevents player from rapidly changing directions when near ball
        var deadzone = 1f;

        float playerAccel = 0;
        var pos = transform.position;
        if (pos.x < ball.transform.position.x - deadzone * Mathf.Abs(playerSpeed)) {
            if (pos.x < ball.transform.position.x - deadzone)
                transform.localScale = new Vector3(2f, 2f, 2f);

            playerAccel = (Time.deltaTime * playerMaxAccel); // * dir;
        } else if (pos.x > ball.transform.position.x + deadzone * Mathf.Abs(playerSpeed)) {
            if (pos.x > ball.transform.position.x + deadzone)
                transform.localScale = new Vector3(-2f, 2f, 2f);

            playerAccel = -(Time.deltaTime * playerMaxAccel); // * dir;
        }


        float distToBall = Mathf.Abs(transform.position.x - ball.transform.position.x);
        float distToBally = Mathf.Abs(transform.position.y - ball.transform.position.y);

        float sval = slowDownSpline.Evaluate(distToBall);
        float newMax = (sval * playerMaxSpeed) * 2;
        //gameCamera.targetFOV = Mathf.Clamp(distToBall*10,35,75);

        //Debug.Log("Max Speed=" + playerMaxSpeed);
        playerSpeed = Mathf.Clamp(playerSpeed + playerAccel, -newMax, newMax);


        audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01(SoundManager.SoundEffectVolume * (playerSpeed * 0.1f)), Time.deltaTime);

        var vel = (playerSpeed * Time.deltaTime) + playerAccel;
        playerSpeed += playerAccel;

        if ((pos.y < ball.transform.position.y || distToBally > 3.0f) && ball.ballStill == true) {
            //pos = new Vector3(Mathf.Lerp(pos.x, ball.transform.position.x, 2.0f) - 0.4f, Mathf.Lerp(pos.y, ball.transform.position.y, 2.0f), pos.z);
            pos.x = ball.transform.position.x;
            pos.y = ball.transform.position.y;
            ball.ballair = true;

            StartCoroutine(TriggerSmoke());

        } else if (pos.y < ball.transform.position.y + 1f && ballair == false) {
            pos.x += vel;
        }

        transform.position = pos;

        //update the animation speed based on the player speed
        animationComp["chieftain_run"].speed = Mathf.Abs((vel * 10f).ClampNeg1());

        distToBall = Mathf.Abs(transform.position.x - ball.transform.position.x);
        if (distToBall < 0.5f && ball.GetComponent<Rigidbody2D>().velocity.magnitude < 0.5f && distToBally < 0.5f) {
            // reset player 
            playerSpeed = 0f;
            transform.localScale = new Vector3(2f, 2f, 2f);
            if (pos.x > ball.transform.position.x) {

                pos.x -= 1.85f;
            }



            transform.position = pos;

            animationComp.CrossFade("chieftain_idle", 0.25f);
            //  pos.y = Mathf.Lerp(pos.y, ball.transform.position.y, 2.0f);
            ballair = false;
            state = PlayerState.AtBall;
            notificationManager.onPlayerStateChange(state, bPlayerSeleceted);

        }
    }

    //change the animation base on the state we want to be in
    public void ChangeState(PlayerState ps) {
        state = ps;

        switch (state) {
            case PlayerState.RunToBall: {
                    animationComp.CrossFade("chieftain_run", 0.25f);
                    break;
                }

            case PlayerState.WaitForPlayer: {
                    animationComp.CrossFade("chieftain_idle", 0.25f);
                    break;
                }

            case PlayerState.AtBall: {
                    animationComp.CrossFade("chieftain_idle", 0.25f);
                    bPlayerSeleceted = false;
                    break;
                }

            case PlayerState.WaitToHitBall: {
                    animationComp.Play("chieftain_hit_ball", PlayMode.StopAll);
                    animationComp.Stop();
                    animationComp["chieftain_hit_ball"].normalizedTime = 0.2f;
                    animationComp["chieftain_hit_ball"].enabled = true;
                    animationComp.Sample();
                    animationComp["chieftain_hit_ball"].enabled = false;

                    break;
                }

            case PlayerState.PlayHitBall: {
                    GameManager.Instance.OnShotTaken();

                    animationComp["chieftain_hit_ball"].enabled = true;
                    break;
                }
        }
    }

    private void FingerDown() {
        bFDown = true;
    }

    private void FingerUp() {
        bFDown = false;
    }

    void ApplyGravity() {
        BoxCollider2D thisCol = transform.GetComponent<BoxCollider2D>();
        Vector2 down = transform.TransformDirection(Vector2.down);
        Vector2 pos = transform.position;

        RaycastHit2D hitInfo = Physics2D.Raycast(pos, down, 1000, ground);

        if (hitInfo) {
            if (hitInfo.distance > 0.4f)
                transform.position = new Vector2(transform.position.x, (Mathf.Lerp(transform.position.y, hitInfo.point.y + 0.1f, Time.deltaTime * gravity)));
        }
    }

    void CheckSlope() {
        bool sheerSlope = false;

        var ball = FindObjectOfType<Ball>();

        if (ball == null)
            return;

        float distanceToBallY = transform.position.y - ball.transform.position.y;

        if (distanceToBallY > 15f || distanceToBallY < -15f)
            return;

        bool facingLeft = false;

        BoxCollider2D thisCol = transform.GetComponent<BoxCollider2D>();
        Vector2 right = transform.TransformDirection(Vector2.right);
        Vector2 left = transform.TransformDirection(Vector2.left);
        Vector2 pos = transform.position;

        if (transform.localScale.x < 0)
            facingLeft = true;

        Vector2 currentFacingDirection = new Vector2();

        if (facingLeft)
            currentFacingDirection = left;
        else
            currentFacingDirection = right;

        RaycastHit2D hitInfo = Physics2D.Raycast(pos, currentFacingDirection, 1000, ground);

        if (hitInfo) {
            float angle = Mathf.Abs(Mathf.Atan2(hitInfo.normal.x, hitInfo.normal.y) * Mathf.Rad2Deg);

            if (angle > maxSlopeAngle)
                sheerSlope = true;
            else
                sheerSlope = false;
        }

        if (hitInfo) {
            if (hitInfo.distance < 0.4f && !sheerSlope) {
                //Debug.Log(hitInfo.transform.name);
                transform.position = new Vector2(transform.position.x, hitInfo.point.y + 0.1f);
            }
        }
    }

    IEnumerator TriggerSmoke() {
        smoke.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        smoke.SetActive(false);

        smoke.GetComponent<Animator>().enabled = true;
    }

    IEnumerator FadeInCharacter() {
        float currentAlpha = Mask.color.a;

        currentAlpha += (Time.deltaTime * 10f);

        Mask.color = new Color(Mask.color.r, Mask.color.g, Mask.color.b, currentAlpha);
        MaskOutline.color = new Color(MaskOutline.color.r, MaskOutline.color.g, MaskOutline.color.b, currentAlpha);

        foreach (SpriteRenderer sprite in characterSprites) {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentAlpha);
        }

        if (Mask.color.a < 1f) {
            yield return null;
        }

        Mask.color = new Color(Mask.color.r, Mask.color.g, Mask.color.b, 1);
        MaskOutline.color = new Color(MaskOutline.color.r, MaskOutline.color.g, MaskOutline.color.b, 1);

        foreach (SpriteRenderer sprite in characterSprites) {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        }
    }

    //This are part of a hack to prevent the player getting stuck on slopes. Please refactor
    void CheckIfStuckOnSlope() {
        currentPos = transform.position;

        float timeUntilStuck = 10f;

        playerStuckTimer += Time.deltaTime;

        if (currentPos == lastPos &&
           state == PlayerState.RunToBall) {
            playerStuckTimer += Time.deltaTime;

            if (playerStuckTimer > timeUntilStuck)
                playerStuck = true;
        } else {
            playerStuck = false;
            playerStuckTimer = 0f;
        }

        lastPos = transform.position;

        if (playerStuck)
            transform.position = ball.transform.position;
    }

}