using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone : MonoBehaviour {

    public float windForce = 10;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<GameBall>() == null) return;

        collision.GetComponent<Rigidbody2D>().AddForce(transform.up * windForce);
    }

}
