using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    [ReadOnly]
    public bool isInSpeedzone = false;

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;
    private float m_GravityScale;

    private const float VELOCITY_SLEEP_THRESHOLD = 0.15f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        sepRef = SoundEffectsPlayer.Instance;

        levelInstance = LevelInstance.Instance;

        if(levelInstance == null) {
            Debug.LogError("GameBall could not find levelinstance!");
            return;
        }

        isInSpeedzone = false;

        GameObject particlePrefab = CustomizationSelected.particle.Obj;
        if(particlePrefab != null) {
            GameObject particle = Instantiate(particlePrefab);
            particle.transform.SetParent(transform, false);
            particle.transform.localPosition = Vector3.zero;
        }
        levelInstance.SetBall(this);

        if (levelInstance.useCannon) isSleeping = true;
    }

    private void Update() {
        if(levelInstance == null) { return; }

        if(m_GravityScale == 0) {
            m_GravityScale = m_RigidBody.gravityScale;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            m_RigidBody.AddForce(levelInstance.ShootPower, ForceMode2D.Impulse);
        }

        if (!isSleeping && levelInstance != null && levelInstance.enableWind) {
            m_RigidBody.AddForce(levelInstance.windForce);
        }
    }

    public void SetGravityScale(float scale) {
        m_GravityScale = scale;
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
        m_RigidBody.gravityScale = m_GravityScale;
        power *= (levelInstance.levelData.gameSpeed * 0.75f);
        m_RigidBody.AddForce(power, ForceMode2D.Impulse);
        TrajectoryRenderer.Instance.gameObject.SetActive(false);
        levelInstance.OnShotFired();

        yield return new WaitForFixedUpdate();
        m_Collider.enabled = true;
    }

    public void CopySettingsTo(GameBall targetBall) {
        targetBall.displayName = displayName;
        targetBall.description = description;
        targetBall.BallHitSound = BallHitSound;
        targetBall.windEffectMultiplier = windEffectMultiplier;
        targetBall.characterSlotDistance = characterSlotDistance;
        targetBall.GetComponent<Rigidbody2D>().gravityScale = GetComponent<Rigidbody2D>().gravityScale;
        targetBall.GetComponent<CircleCollider2D>().sharedMaterial = GetComponent<Collider2D>().sharedMaterial;

        targetBall.SetGravityScale(m_GravityScale);
    }

    #region Ball sleeping
    private void OnCollisionEnter2D(Collision2D collision) {
        m_BallCollisionStart = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Vector2 direction = new Vector2(transform.position.x, transform.position.y) - collision.contacts[0].point;
        float angle = Vector2.SignedAngle(direction, Vector2.up);
        float velocityMagnitude = m_RigidBody.velocity.magnitude;
        if ((Mathf.Abs(angle) >= 20 && velocityMagnitude >= VELOCITY_SLEEP_THRESHOLD) || (isInSpeedzone && velocityMagnitude >= 0.1f)) return;

        float materialFriction = Mathf.Clamp01((collision.collider.sharedMaterial.friction - 1) * 0.5f);
        m_RigidBody.velocity *= ((1f - materialFriction) - (Time.time - m_BallCollisionStart) * 0.2f);

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
            TrajectoryRenderer.Instance.gameObject.SetActive(true);

            while (isSleeping) {
                m_RigidBody.velocity = Vector2.zero;
                m_RigidBody.gravityScale = 0;
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