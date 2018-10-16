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
    public bool isInGame = true;

    [Header("SFX")]
	[ReadOnly]	public SoundEffectsPlayer sepRef = null;
				public SoundEffectCollection BallHitSound;

    [Header("Status")]
    [ReadOnly] public bool isSleeping = false;
    [ReadOnly] public LevelInstance levelInstance = null;
    [ReadOnly] public Vector2 slotLeft;
    [ReadOnly] public Vector2 slotRight;
    [ReadOnly] public bool isInWindZone = false;
    [ReadOnly] public float groundAngle = 0;
    [ReadOnly] public bool completeSleep = false;

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;
    private bool m_BallCollisionStartReset = true;
    private float m_GravityScale;

    private const float VELOCITY_SLEEP_THRESHOLD = 0.25f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();


        GameObject particlePrefab = CustomizationSelected.particle.Obj;
        if (particlePrefab != null) {
            GameObject particle = Instantiate(particlePrefab);
            particle.transform.SetParent(transform, false);
            particle.transform.localPosition = Vector3.zero;
        }

        if (!isInGame) {
            m_RigidBody.isKinematic = true;
            return;
        }

        isInWindZone = false;
    }

    private void Start() {
        sepRef = SoundEffectsPlayer.Instance;

        levelInstance = LevelInstance.Instance;
        if (levelInstance == null) {
            return;
        }

        if(LevelInstance.Instance.ballSwitchCount == 2) {
            levelInstance.SetBall(this, false);
        } else {
            levelInstance.SetBall(this);
        }

        if (levelInstance.useCannon) isSleeping = true;
    }

    private void Update() {
        if(levelInstance == null || !isInGame) { return; }

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

    public void HitBall(Vector2 power, float normalizedPower) {
        StartCoroutine(HitBallRoutine(power));
        if(normalizedPower >= 0.8f) {
            normalizedPower = normalizedPower.Remap(0.8f, 1f, 0.5f, 1f);
            GameCamera.Instance.camShake += normalizedPower;
        }
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
        if (!isInGame) return;
        m_BallCollisionStart = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!isInGame) return;
        if(collision.collider.sharedMaterial == null) {
            Debug.LogError("The terrain object " + collision.collider.gameObject.name + " didn't have a physics material assigned to it. Temporarily set to default.", collision.collider.gameObject);
            collision.collider.sharedMaterial = levelInstance.levelData.defaultTerrainMaterial;
        }

        Vector2 direction = collision.contacts[0].normal;

        float velocityMagnitude = m_RigidBody.velocity.magnitude;
        if (isInWindZone && velocityMagnitude >= 0.75f) {
            m_BallCollisionStartReset = true;
            return;
        }

        if (m_BallCollisionStartReset) {
            m_BallCollisionStartReset = false;
            m_BallCollisionStart = Time.time;
        }

        groundAngle = Mathf.Abs(Vector2.SignedAngle(direction, Vector2.up));
        float maxAngle = 20;
        float lerpValue = Mathf.Clamp((maxAngle - groundAngle) / maxAngle, 0, 1);
        lerpValue = levelInstance.levelData.ballSlowdownCurve.Evaluate(lerpValue);

        float materialFriction = Mathf.Lerp(1, Mathf.Clamp01((collision.collider.sharedMaterial.friction) * 0.8f), lerpValue);
        float finalMultiplier = Mathf.Clamp01(materialFriction + 0.2f) - ((Time.time - m_BallCollisionStart) * 0.1f * lerpValue);
        m_RigidBody.velocity *= finalMultiplier;

        //Start a routine to check if the ball will stay still
        if (velocityMagnitude <= VELOCITY_SLEEP_THRESHOLD) {
            StartSleepRoutine();
        } else {
            StopSleepRoutine();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!isInGame) return;
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
        completeSleep = false;
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
                completeSleep = true;
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