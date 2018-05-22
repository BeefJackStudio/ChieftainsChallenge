using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone : MonoBehaviour {

    public float windForce = 10;

    private void OnTriggerStay2D(Collider2D collision) {
        GameBall ball = collision.gameObject.GetComponent<GameBall>();
        if (ball == null) return;

        collision.GetComponent<Rigidbody2D>().AddForce(transform.up * windForce * ball.windEffectMultiplier);
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        GameBall ball = collision.gameObject.GetComponent<GameBall>();
        if (ball == null) return;

        ball.allowSleeping = false;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GameBall ball = collision.gameObject.GetComponent<GameBall>();
        if (ball == null) return;

        ball.allowSleeping = true;
    }

}
