using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpeedZone : MonoBehaviour {

	public float force = 5000.0f;

	private void Awake() {
		this.GetComponent<BoxCollider2D>().isTrigger = true;
	}

	private void OnValidate() {
		this.GetComponent<BoxCollider2D>().hideFlags = HideFlags.HideInInspector;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.GetComponent<GameBall>() == null) return;

	    other.GetComponent<Rigidbody2D>().AddForce(other.GetComponent<Rigidbody2D>().velocity * force * LevelInstance.Instance.levelData.gameSpeed);

	}

}
