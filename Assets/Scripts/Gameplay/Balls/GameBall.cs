using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TrajectoryRenderer))]
public class GameBall : MonoBehaviour {

    [Header("Visualization")]
    public string displayName;
    public string description;

    [Header("Config")]
    public float windEffectMultiplier = 1;
    public float characterSlotDistance = 2f;

    [Header("SFX")]
	[ReadOnly]	public SoundEffectsPlayer sepRef = null;
				public SoundEffectCollection BallHitSound;

    [Header("Status")]
    [ReadOnly] public bool isSleeping = false;
    [ReadOnly] public LevelInstance levelInstance = null;
    [ReadOnly] public Vector2 slotLeft;
    [ReadOnly] public Vector2 slotRight;

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;
    private TrajectoryRenderer m_TrajectoryRenderer;

    private const float VELOCITY_SLEEP_THRESHOLD = 0.1f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        m_TrajectoryRenderer = GetComponent<TrajectoryRenderer>();
        sepRef = SoundEffectsPlayer.Instance;

        levelInstance = LevelInstance.Instance;

        if(levelInstance == null) {
            Debug.LogError("GameBall could not find levelinstance!");
            return;
        }

        levelInstance.SetBall(this);
    }

    private void Update() {
        Debug.DrawLine(transform.position, transform.position + new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 0), Color.yellow, 5);

        if(levelInstance == null) { return; }

        if (Input.GetKeyDown(KeyCode.D)) {
            m_RigidBody.AddForce(levelInstance.ShootPower, ForceMode2D.Impulse);
        }

        if (!isSleeping && levelInstance != null && levelInstance.enableWind) {
            m_RigidBody.AddForce(levelInstance.windForce);
        }else {
            m_TrajectoryRenderer.Plot(m_RigidBody);
        }
    }
    
    public void HitBall(Vector2 power) {
        StartCoroutine(HitBallRoutine(power));
    }

    private IEnumerator HitBallRoutine(Vector2 power) {
        StopSleepRoutine();
        if (sepRef != null && BallHitSound != null) {
            sepRef.PlaySFX(BallHitSound);
        }

        m_Collider.enabled = false;
        m_RigidBody.AddForce(power, ForceMode2D.Impulse);
        m_TrajectoryRenderer.StopRender();
        levelInstance.OnShotFired();

        yield return new WaitForFixedUpdate();
        m_Collider.enabled = true;
    }

#region Ball sleeping
    private void OnCollisionEnter2D(Collision2D collision) {
        m_BallCollisionStart = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Vector2 direction = new Vector2(transform.position.x, transform.position.y) - collision.contacts[0].point;
        float angle = Vector2.SignedAngle(direction, Vector2.up);
        float velocityMagnitude = m_RigidBody.velocity.magnitude;
        if (Mathf.Abs(angle) >= 20 && velocityMagnitude >= VELOCITY_SLEEP_THRESHOLD) return;

        float materialFriction = Mathf.Clamp01((collision.collider.sharedMaterial.friction - 1) * 0.5f);
        m_RigidBody.velocity *= ((1f - materialFriction) - (Time.time - m_BallCollisionStart) * 0.075f);

        //Start a routine to check if the ball will stay still
        if (velocityMagnitude <= VELOCITY_SLEEP_THRESHOLD) {
            StartSleepRoutine();
        } else {
            StopSleepRoutine();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        StopSleepRoutine();
    }

    public void StartSleepRoutine(bool forceSleep = false) {
        isSleeping = true;
        if (m_BallSleepRoutine != null) { return; }
        m_BallSleepRoutine = StartCoroutine(BallSleepRoutine(forceSleep));
    }

    private void StopSleepRoutine() {
        isSleeping = false;
        if (m_BallSleepRoutine == null) { return; }
        StopCoroutine(m_BallSleepRoutine);
        m_BallSleepRoutine = null;
    }

    private IEnumerator BallSleepRoutine(bool forceSleep) {
        float startTime = Time.time;
        float waitTime = 1;

        if (!forceSleep) {
            yield return new WaitForSeconds(waitTime);
        }

        if (m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD || forceSleep) {

            if(!forceSleep) LevelInstance.Instance.TriggerNextTurn();
            m_TrajectoryRenderer.StartRender();

            while (isSleeping) {
                m_RigidBody.velocity = Vector2.zero;
                yield return null;
            }
        } 

        m_BallSleepRoutine = null;
    }
#endregion

#region Character
    public void CalculateSlotLocations() {
        slotLeft = transform.position;
        slotLeft.x -= characterSlotDistance;
        RaycastHit2D hitLeft = Physics2D.Raycast(slotLeft, new Vector2(0,-1), 40);
        if(hitLeft != null) {
            slotLeft = hitLeft.point;
        }

        slotRight = transform.position;
        slotRight.x += characterSlotDistance;
        RaycastHit2D hitRight = Physics2D.Raycast(slotRight, new Vector2(0,-1), 40);
        if(hitRight != null) {
            slotRight = hitRight.point;
        }
    }

    public bool IsAimingRight() {
        if(levelInstance.shootAngle.x < 0) { return false; }
        return true;
    }

#endregion
}