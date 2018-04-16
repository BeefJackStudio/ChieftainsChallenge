using UnityEngine;
using System.Collections;

public class StopBall : MonoBehaviour {

    public float stopMagnitude = 1f;
    public Ball ball;

    public float slopeThreshold;
    private bool onSlope;

    public LayerMask ground;

    private Rigidbody2D rB;

    void OnEnable() {
        rB = transform.GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (rB.velocity.magnitude < stopMagnitude && (!ball.inAir) && !onSlope && ball.onGround) {
            rB.velocity = new Vector3(0, 0, 0);
        }

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1000, ground);

        if (hitInfo) {
            float angle = Mathf.Abs(Mathf.Atan2(hitInfo.normal.x, hitInfo.normal.y) * Mathf.Rad2Deg);

            if (angle > slopeThreshold)
                onSlope = true;
            else
                onSlope = false;
        }
    }

}
