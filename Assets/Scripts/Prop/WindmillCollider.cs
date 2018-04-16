using UnityEngine;
using System.Collections;

public class WindmillCollider : MonoBehaviour {
    Windmill parent;

    // Use this for initialization
    void Start() {
        parent = transform.parent.GetComponent<Windmill>();
    }

    void OnTriggerEnter2D(Collider2D collision) {

        if (collision.name == "Ball" && parent.state == Windmill.WindmillState.WaitForBall) {
            parent.ball = collision.gameObject;
            parent.ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //ball.GetComponent<Rigidbody2D>().gravityScale = 0;
            parent.ball.transform.parent = parent.ballAncer.transform;
            parent.ball.transform.position = Vector3.zero;
            parent.ball.GetComponent<SpriteRenderer>().enabled = false;
            parent.state = Windmill.WindmillState.SpinUp;
        }

    }

}
