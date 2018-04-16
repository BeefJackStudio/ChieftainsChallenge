using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {
    public float ShootAngle;
    public float ShootPower;

    public enum CannonState {
        WaitForBall,
        BallIn,
        Fire
    };

    public CannonState state;
    public GameObject cannon;
    public GameObject ballAncer;
    public ParticleSystem boom;
    bool ballShot;
    float shootDelay;

    public GameObject ball;

    void OnEnable() {
        NotificationsManager.OnBallEnabled += NotificationsManager_OnBallEnabled;

        state = CannonState.WaitForBall;
    }

    void OnDisable() {
        NotificationsManager.OnBallEnabled -= NotificationsManager_OnBallEnabled;
    }

    private void NotificationsManager_OnBallEnabled(Transform thisBall) {
        ball = thisBall.gameObject;
    }

    void Update() {
        if (ball == null)
            return;

        switch (state) {
            case CannonState.WaitForBall: {
                    Vector3 diff = cannon.transform.position - ball.transform.position;
                    diff.Normalize();
                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    cannon.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 220);

                    break;
                }
            case CannonState.BallIn: {
                    ball.transform.localPosition = Vector2.zero;
                    ball.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
                    ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                    Vector3 rot = cannon.transform.eulerAngles;
                    if (!rot.z.Aprox(ShootAngle - 40.0f, 1)) {
                        rot.z = Mathf.Lerp(rot.z, ShootAngle - 40.0f, Time.deltaTime);
                        cannon.transform.eulerAngles = rot;
                    } else {
                        ballShot = false;
                        state = CannonState.Fire;
                        GameObject.Instantiate(boom, ballAncer.transform.position, Quaternion.identity);
                        shootDelay = 3;
                        SoundManager.playSFX(GetComponent<AudioSource>());
                    }
                    break;
                }
            case CannonState.Fire: {
                    shootDelay -= Time.deltaTime;
                    if (shootDelay <= 0) {
                        state = CannonState.WaitForBall;
                    } else if (shootDelay <= 2.5f && !ballShot) {
                        ball.GetComponent<SpriteRenderer>().enabled = true;
                        ball.transform.parent = null;
                        ball.GetComponent<Ball>().UpdateForceAngle(ShootAngle);
                        ball.GetComponent<Ball>().UpdatePower(ShootPower / 10.0f);
                        ball.GetComponent<Ball>().ShootBall();
                        ballShot = true;
                    } else if (shootDelay > 2.5f) {
                        ball.transform.localPosition = Vector2.zero;
                        ball.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
                    }
                    break;
                }
        }
    }
}
