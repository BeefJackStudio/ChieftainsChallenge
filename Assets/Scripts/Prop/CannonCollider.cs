using UnityEngine;
using System.Collections;

public class CannonCollider : MonoBehaviour {
    Cannon parent;
    // Use this for initialization
    void Start() {
        parent = transform.parent.parent.GetComponent<Cannon>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (parent != null) {
            if (collision.gameObject.GetComponent<Ball>() != null && parent.state == Cannon.CannonState.WaitForBall) {
                parent.ball = collision.gameObject;
                parent.ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                //ball.GetComponent<Rigidbody2D>().gravityScale = 0;
                parent.ball.transform.parent = parent.ballAncer.transform;
                parent.ball.GetComponent<SpriteRenderer>().enabled = false;
                parent.state = Cannon.CannonState.BallIn;
            }
        }

    }

}
