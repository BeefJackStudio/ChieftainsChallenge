using UnityEngine;

public class Ball : ResettableObject {
    public NotificationsManager notificationManager;
    public GameObject TrajectoryPointPrefeb;
    float ballForceScale = 0;
    public Vector2 forceAngle = new Vector2(1, 0);
    private Rigidbody2D rb2;
    private Vector3 resetBall;
    //public float Inertia = 0.0f;                // The resistance an object has to a change in its state of motion.
    [Tooltip("How long to count before slowing the ball down.")]
    public float MinVelocityTime = 5.0f;        // How long to count before slowing the ball down.
    [Tooltip("Min Speed to start counting.")]
    public float MinVelocity = 2.0f;            // Min Speed to start counting.
    [Tooltip("How fast the ball slows down. Vel*SlowdownSpeed.")]
    public float SlowDownSpeed = 1.5f;          // How fast the ball slows down. Vel*SlowdownSpeed.
    [Tooltip("If the Velocity is lower than this number the ball stops moving.")]
    public float DeadZone = 0.1f;               // If the Velocity is lower than this number the ball stops moving.
    [Tooltip("Time.")]
    float BufferTime = 0;                       // Time

    public bool inAir = false;
    public float airCount = 0;
    public bool ballStill = false;
    public bool ballair = false;

    public Ball parent;

    AudioSource audioSource;

    public GameObject SmashHitEffectInstance;

    public bool onGround;

    //------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        rb2 = GetComponent<Rigidbody2D>();
        resetBall = transform.position;
        audioSource = GetComponent<AudioSource>();

        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;
        NotificationsManager.StartPowerShot += NotificationsManager_StartPowerShot;
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;

        notificationManager.onBallMovement(airCount, transform.position);
        notificationManager.onBallEnabled(transform);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
        NotificationsManager.StartPowerShot -= NotificationsManager_StartPowerShot;
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType thisGameType, int coursePar) {
        notificationManager.onBallMovement(airCount, transform.position);

        switch (thisGameType) {
            case GameManager.GameType.Cannon:
                gameObject.SetActive(false);
                break;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_StartPowerShot() {
        transform.GetComponent<TrailRenderer>().enabled = false;
        GameObject fire = Instantiate(SmashHitEffectInstance) as GameObject;
        fire.transform.parent = transform;
        fire.transform.localPosition = Vector3.zero;
        fire.transform.localRotation = Quaternion.identity;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.FreeView:
                if (transform.childCount > 0) {
                    GameObject obj = transform.GetChild(0).gameObject;
                    obj.transform.parent = null;
                    Destroy(obj);
                    GetComponent<TrailRenderer>().enabled = true;
                }
                break;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------

    public void Init() {
        rb2 = GetComponent<Rigidbody2D>();
        resetBall = transform.position;
    }

    public override void ResetMe() {
        base.ResetMe();
        rb2.angularVelocity = 0f;
        rb2.velocity = Vector2.zero;

    }

    public void Awake() {
        if (notificationManager == null) // needed for ball to function correctly in-game
        {
            notificationManager = FindObjectOfType<NotificationsManager>();
        }
    }

    public void Update() {
        if (ballStill == false)
            notificationManager.onBallMovement(airCount, transform.position);

        if ((rb2.velocity.magnitude) <= MinVelocity) {
            BufferTime += Time.deltaTime;
        } else {
            BufferTime = 0;
        }
        if (BufferTime >= MinVelocityTime)
            rb2.velocity -= (rb2.velocity - (rb2.velocity * SlowDownSpeed)) * Time.deltaTime;

        if ((rb2.velocity.magnitude) <= DeadZone) {
            rb2.velocity = Vector2.zero;
            ballStill = true;
        } else {
            ballStill = false;

        }

        if (inAir) {
            airCount += Time.deltaTime;
            if (parent) {
                parent.airCount += Time.deltaTime;
            }
        }
    }



    public void UpdatePower(float power) {
        ballForceScale = 3000.0f * power;
    }

    public void UpdateForceAngle(float angle) {
        forceAngle.y = Mathf.Clamp01(Mathf.Sin(Mathf.Deg2Rad * angle));
        forceAngle.x = Mathf.Clamp01(Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    public void ResetBall() {
        transform.position = resetBall;
        rb2.velocity = Vector3.zero;
        rb2.angularVelocity = 0;
    }

    public void ShootBall() {
        rb2.AddForce(forceAngle * ballForceScale, ForceMode2D.Force);
        BufferTime = 0;
        inAir = true;
    }

    void OnCollisionEnter2D(Collision2D col) {
        inAir = false;
        if (!audioSource.isPlaying) {
            SoundManager.playSFX(audioSource);
        }

        if (col.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            onGround = true;
    }

    void OnCollisionExit2D(Collision2D col) {
        inAir = true;
        onGround = false;
    }

}
