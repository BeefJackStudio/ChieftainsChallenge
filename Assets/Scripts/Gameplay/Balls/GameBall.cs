using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TrajectoryRenderer))]
public class GameBall : MonoBehaviour {

    [Header("Status")]
    [ReadOnly] public bool isSleeping = false;
    [ReadOnly] public bool dragging = false;
    [ReadOnly] public LevelInstance levelInstance = null;
    [ReadOnly] public Vector3 dragDelta = Vector3.zero;

    [Header("Config")]
    public bool enableTrajectoryPrediction = true;

    private Rigidbody2D m_RigidBody;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;
    private Vector3 m_MouseClickStart;
    private const float VELOCITY_SLEEP_THRESHOLD = 0.1f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
        levelInstance = GameObject.Find("LevelInstance").GetComponent<LevelInstance>();

        if(levelInstance == null) {
            Debug.LogError("GameBall could not find levelinstance!");
            return;
        }

        levelInstance.SetBall(this);
    }

    private void Update() {
        if(levelInstance == null) { return; }

        if (Input.GetMouseButtonDown(0)) {
            Vector3 touchpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
            bool allowShot = false;
            foreach(RaycastHit2D hit in hits) {
                if (hit.collider && hit.collider.gameObject == this.gameObject && 
                    levelInstance.levelState == LevelState.inGame && isSleeping) {
                    allowShot = true;
                }
            }

            if(allowShot) {
                m_MouseClickStart = Input.mousePosition;
                dragging = true;
                if(enableTrajectoryPrediction) { gameObject.GetComponent<TrajectoryRenderer>().StartRender(); }
            }
        }

        if(dragging) {
            //make additional delta calcs for trajectory renderer.
            if(Input.GetMouseButton(0)) {
                dragDelta = Input.mousePosition - m_MouseClickStart;
            }

            if (Input.GetMouseButtonUp(0)) {
                dragging = false;
                dragDelta = Input.mousePosition - m_MouseClickStart;
                if(enableTrajectoryPrediction) { gameObject.GetComponent<TrajectoryRenderer>().StopRender(); }
                HitBall(-dragDelta, dragDelta.magnitude / 10);
            }
        }

        if(!isSleeping && levelInstance != null && levelInstance.enableWind) {
            Vector2 windForce = levelInstance.windDirection.normalized * levelInstance.GetRandomWindForce();
            m_RigidBody.AddForce(windForce);
        }
    }

    public void HitBall(Vector2 direction, float power) {
        StopSleepRoutine();
        m_RigidBody.AddForce(direction.normalized * power, ForceMode2D.Impulse);

        if(power > 1 && levelInstance != null) {
            levelInstance.OnShotFired();
        }
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
        if (m_BallSleepRoutine != null) { return; }
        m_BallSleepRoutine = StartCoroutine(BallSleepRoutine());
        isSleeping = true;
    }

    private void StopSleepRoutine() {
        if (m_BallSleepRoutine == null) { return; }
        StopCoroutine(m_BallSleepRoutine);
        m_BallSleepRoutine = null;
        isSleeping = false;
    }

    private IEnumerator BallSleepRoutine() {
        float startTime = Time.time;
        float waitTime = 2;

        yield return new WaitForSeconds(waitTime);

        if(m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD) {
            while (true) {
                m_RigidBody.velocity = Vector2.zero;
                yield return null;
            }
        } 

        m_BallSleepRoutine = null;
    }

#endregion
}
