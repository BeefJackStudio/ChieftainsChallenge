using UnityEngine;
using System.Collections;

public class MaskCollision : MonoBehaviour {
    void OnCollisionEnter2D(Collision2D coll) {
        SoundEffectsPlayer.Instance.onMaskCollision();
    }
}
