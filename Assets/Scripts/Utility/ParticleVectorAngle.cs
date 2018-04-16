using UnityEngine;
using System.Collections;

public class ParticleVectorAngle : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (transform.parent != null) {
            Vector2 from = Vector2.up;
            Vector2 to = transform.parent.GetComponent<Rigidbody2D>().velocity.normalized;
            float angle = Mathf.DeltaAngle(Mathf.Atan2(from.y, from.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg);
            transform.GetChild(0).GetComponent<ParticleSystem>().startRotation = (180 - angle) * Mathf.Deg2Rad;
        }
    }
}
