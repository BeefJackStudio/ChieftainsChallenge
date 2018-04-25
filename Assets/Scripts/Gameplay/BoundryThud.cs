using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryThud : MonoBehaviour {

	[Header("Music")]
	[ReadOnly]	public SoundEffectsPlayer sepRef = null;
                public SoundEffectCollection onBallCollideSound;

	void Start () {
		sepRef = SoundEffectsPlayer.Instance;
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
        if((coll.gameObject.GetComponent("GameBall") as GameBall) != null && sepRef != null && onBallCollideSound != null) {
			sepRef.PlaySFX(onBallCollideSound);
        }
    }
}
