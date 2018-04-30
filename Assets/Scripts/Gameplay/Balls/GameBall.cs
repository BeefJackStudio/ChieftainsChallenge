using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TrajectoryRenderer))]
public class GameBall : MonoBehaviour {

    [Header("Status")]
    [ReadOnly] public bool isSleeping = false;
    [ReadOnly] public LevelInstance levelInstance = null;

    private Rigidbody2D m_RigidBody;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;
    private TrajectoryRenderer m_TrajectoryRenderer;

    private const float VELOCITY_SLEEP_THRESHOLD = 0.1f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_TrajectoryRenderer = GetComponent<TrajectoryRenderer>();

        levelInstance = LevelInstance.Instance;

        if(levelInstance == null) {
            Debug.LogError("GameBall could not find levelinstance!");
            return;
        }

        levelInstance.SetBall(this);
    }

    private void Update() {
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
        StopSleepRoutine();
        m_RigidBody.AddForce(power, ForceMode2D.Impulse);
        m_TrajectoryRenderer.StopRender();
        levelInstance.OnShotFired();
    }

#region Ball sleeping
    private void OnCollisionEnter2D(Collision2D collision) {
        m_BallCollisionStart = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        m_RigidBody.velocity *= (1f - (Time.time - m_BallCollisionStart) * 0.05f);

        //Start a routine to check if the ball will stay still
        if (m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD) {
            StartSleepRoutine();
        } else {
            StopSleepRoutine();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        StopSleepRoutine();
    }

    private void StartSleepRoutine() {
        isSleeping = true;
        if (m_BallSleepRoutine != null) { return; }
        m_BallSleepRoutine = StartCoroutine(BallSleepRoutine());
    }

    private void StopSleepRoutine() {
        isSleeping = false;
        if (m_BallSleepRoutine == null) { return; }
        StopCoroutine(m_BallSleepRoutine);
        m_BallSleepRoutine = null;
    }

    private IEnumerator BallSleepRoutine() {
        float startTime = Time.time;
        float waitTime = 2;

        yield return new WaitForSeconds(waitTime);

        if(m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD) {

            LevelInstance.Instance.TriggerNextTurn();
            m_TrajectoryRenderer.StartRender();

            while (isSleeping) {
                m_RigidBody.velocity = Vector2.zero;
                yield return null;
            }
        } 

        m_BallSleepRoutine = null;
    }

    #endregion

}