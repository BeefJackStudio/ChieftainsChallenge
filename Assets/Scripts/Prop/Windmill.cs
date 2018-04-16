using UnityEngine;
using System.Collections;

public class Windmill : MonoBehaviour {
    public float maxSpinSpeed;
    public float ShootAngle;
    public float ShootPower;

    public enum WindmillState {
        WaitForBall,
        SpinUp,
        SpinDown
    };

    public WindmillState state;
    public GameObject windmill;
    public GameObject ball;
    public GameObject ballAncer;

    float rotationSpeed;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case WindmillState.WaitForBall: {
                    break;
                }
            case WindmillState.SpinUp: {
                    ball.transform.localPosition = Vector2.zero;
                    ball.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
                    ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                    rotationSpeed = Mathf.Lerp(rotationSpeed, maxSpinSpeed, Time.deltaTime * 1);
                    Vector3 rot = windmill.transform.eulerAngles;
                    rot.z -= rotationSpeed;
                    windmill.transform.eulerAngles = rot;
                    if (rotationSpeed.Aprox(maxSpinSpeed, 0.1f)) {
                        state = WindmillState.SpinDown;
                        ball.GetComponent<SpriteRenderer>().enabled = true;
                        ball.transform.parent = null;
                        ball.GetComponent<Ball>().UpdateForceAngle(ShootAngle);
                        ball.GetComponent<Ball>().UpdatePower(ShootPower / 10.0f);
                        ball.GetComponent<Ball>().ShootBall();
                    }
                    break;
                }
            case WindmillState.SpinDown: {
                    rotationSpeed = Mathf.Lerp(rotationSpeed, 0, Time.deltaTime * 0.25f);
                    Vector3 rot = windmill.transform.eulerAngles;
                    rot.z += rotationSpeed;
                    windmill.transform.eulerAngles = rot;
                    if (rotationSpeed.Aprox(0, 0.1f)) {
                        state = WindmillState.WaitForBall;
                    }
                    break;
                }
        }
    }


}
