using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

    public Transform ball;
    public Vector3 defaultOffset = new Vector3(0, 5, -25);
    public float followSpeed = 0.1f;

    private Vector3 m_PreviousBallPosition;

    private void Awake() {
        m_PreviousBallPosition = ball.position;
    }

    private void FixedUpdate() {
        Vector3 ballPosition = ball.position;
        Vector3 ballDelta = (ballPosition - m_PreviousBallPosition) * 10;

        transform.position = Vector3.Lerp(transform.position, ball.position + (ballDelta * 2) + defaultOffset + new Vector3(0, 0, -ballDelta.magnitude), followSpeed);

        m_PreviousBallPosition = ball.position;
    }
}
